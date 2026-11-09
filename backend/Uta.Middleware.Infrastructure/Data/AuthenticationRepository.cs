using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Data;

internal sealed class AuthenticationRepository(EduMonitorDbContext dbContext) : IAuthenticationRepository
{
    public async Task<CuentaUsuarioDto?> FindUserAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await dbContext.UsuariosSistema.AsNoTracking()
            .Where(user => user.Email == normalizedEmail && user.IsActive)
            .Select(user => new CuentaUsuarioDto(
                user.Id, user.Name, user.Email, user.Role, user.PasswordHash))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<UsuarioAutenticadoDto?> CreateBootstrapAdministratorAsync(
        string email,
        string passwordHash,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        if (await dbContext.UsuariosSistema.AnyAsync(
                user => user.IsActive && user.Role == "Administrador técnico",
                cancellationToken))
        {
            return null;
        }

        var account = new UsuarioSistemaRow
        {
            Name = "Administrador inicial",
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = "Administrador técnico",
            IsActive = true
        };
        dbContext.UsuariosSistema.Add(account);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueEmailViolation(exception))
        {
            throw new InvalidOperationException("Ya existe una cuenta con ese correo.", exception);
        }

        return new UsuarioAutenticadoDto(account.Id, account.Name, account.Email, account.Role);
    }

    public async Task<UsuarioAutenticadoDto> CreateUserAsync(
        string name,
        string email,
        string passwordHash,
        string role,
        IReadOnlyCollection<int> moodleCourseIds,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await dbContext.UsuariosSistema.AnyAsync(
                user => user.Email == normalizedEmail, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe una cuenta con ese correo.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var account = new UsuarioSistemaRow
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true
        };
        dbContext.UsuariosSistema.Add(account);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueEmailViolation(exception))
        {
            throw new InvalidOperationException("Ya existe una cuenta con ese correo.", exception);
        }

        if (role == "Docente")
        {
            var courseIds = moodleCourseIds.Distinct().ToArray();
            var courses = await dbContext.Cursos
                .Where(course => courseIds.Contains(course.MoodleId))
                .ToListAsync(cancellationToken);
            if (courses.Count != courseIds.Length)
            {
                throw new InvalidOperationException(
                    "Uno o más cursos indicados no existen en la base de EduMonitor.");
            }

            dbContext.AsignacionesCurso.AddRange(courses.Select(course =>
                new AsignacionCursoRow { UserId = account.Id, CourseId = course.Id }));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return new UsuarioAutenticadoDto(account.Id, account.Name, account.Email, account.Role);
    }

    private static bool IsUniqueEmailViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };
}
