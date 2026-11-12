using Microsoft.EntityFrameworkCore;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Data;

internal sealed class AuditLogRepository(EduMonitorDbContext dbContext) : IAuditLogRepository
{
    public async Task RecordAsync(
        string? userId,
        string userName,
        string role,
        string eventType,
        int? courseId,
        string result,
        string detail,
        CancellationToken cancellationToken = default)
    {
        dbContext.AuditLogs.Add(new AuditLogRow
        {
            OccurredAtUtc = DateTime.UtcNow,
            UserId = Limit(userId, 200),
            UserName = Limit(userName, 150) ?? "Usuario",
            Role = Limit(role, 40) ?? "Desconocido",
            EventType = Limit(eventType, 80) ?? "Evento",
            CourseId = courseId,
            Result = Limit(result, 20) ?? "Desconocido",
            Detail = Limit(detail, 500) ?? string.Empty
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PaginaDto<RegistroAuditoriaDto>> SearchAsync(
        FiltroAuditoriaDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.AuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(filter.Tipo))
        {
            query = query.Where(item => item.EventType == filter.Tipo);
        }

        if (filter.DesdeUtc.HasValue)
        {
            query = query.Where(item => item.OccurredAtUtc >= filter.DesdeUtc.Value.UtcDateTime);
        }

        if (filter.HastaUtc.HasValue)
        {
            query = query.Where(item => item.OccurredAtUtc <= filter.HastaUtc.Value.UtcDateTime);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(item => item.OccurredAtUtc)
            .ThenByDescending(item => item.Id)
            .Skip((filter.Pagina - 1) * filter.TamanoPagina)
            .Take(filter.TamanoPagina)
            .Select(item => new RegistroAuditoriaDto(
                item.Id,
                item.OccurredAtUtc,
                item.UserName,
                item.Role,
                item.EventType,
                item.CourseId,
                item.Result,
                item.Detail))
            .ToListAsync(cancellationToken);

        return new PaginaDto<RegistroAuditoriaDto>(
            items,
            filter.Pagina,
            filter.TamanoPagina,
            total,
            (int)Math.Ceiling(total / (double)filter.TamanoPagina));
    }

    private static string? Limit(string? value, int length) =>
        value is null || value.Length <= length ? value : value[..length];
}
