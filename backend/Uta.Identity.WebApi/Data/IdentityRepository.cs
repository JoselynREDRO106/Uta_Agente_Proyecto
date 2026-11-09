using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Uta.Middleware.Application.DTOs;

namespace Uta.Identity.WebApi.Data;

public sealed class IdentityRepository(IdentityDbContext dbContext)
{
    public async Task<IdentityAccount?> FindUserAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.AsNoTracking()
            .SingleOrDefaultAsync(
                account => account.Email == normalizedEmail && account.IsActive,
                cancellationToken);
        if (user is null)
        {
            return null;
        }

        var courseIds = await dbContext.CourseAssignments.AsNoTracking()
            .Where(assignment => assignment.UserId == user.Id)
            .Select(assignment => assignment.MoodleCourseId)
            .ToArrayAsync(cancellationToken);

        return new IdentityAccount(
            user.Id, user.Name, user.Email, user.Role, user.PasswordHash, courseIds);
    }

    public async Task<UsuarioAutenticadoDto?> CreateBootstrapAdministratorAsync(
        string email,
        string passwordHash,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        if (await dbContext.Users.AnyAsync(
                user => user.IsActive && user.Role == "Administrador técnico",
                cancellationToken))
        {
            return null;
        }

        var account = new IdentityUserRow
        {
            Name = "Administrador inicial",
            Email = NormalizeEmail(email),
            PasswordHash = passwordHash,
            Role = "Administrador técnico",
            IsActive = true
        };
        dbContext.Users.Add(account);
        await SaveAccountAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new UsuarioAutenticadoDto(account.Id, account.Name, account.Email, account.Role);
    }

    public async Task<UsuarioAutenticadoDto> CreateUserAsync(
        string name,
        string email,
        string passwordHash,
        string role,
        IReadOnlyCollection<int> moodleCourseIds,
        CancellationToken cancellationToken)
    {
        var account = new IdentityUserRow
        {
            Name = name.Trim(),
            Email = NormalizeEmail(email),
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true
        };
        var courseIds = moodleCourseIds.Distinct().ToArray();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.Users.Add(account);
        await SaveAccountAsync(cancellationToken);

        if (role == "Docente" && courseIds.Length > 0)
        {
            dbContext.CourseAssignments.AddRange(courseIds.Select(courseId =>
                new IdentityCourseAssignmentRow
                {
                    UserId = account.Id,
                    MoodleCourseId = courseId
                }));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return new UsuarioAutenticadoDto(account.Id, account.Name, account.Email, account.Role);
    }

    private async Task SaveAccountAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueEmailViolation(exception))
        {
            throw new InvalidOperationException("Ya existe una cuenta con ese correo.", exception);
        }
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static bool IsUniqueEmailViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };
}

public sealed record IdentityAccount(
    int Id,
    string Name,
    string Email,
    string Role,
    string PasswordHash,
    IReadOnlyCollection<int> MoodleCourseIds);
