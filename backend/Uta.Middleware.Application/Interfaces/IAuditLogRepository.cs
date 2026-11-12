using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.Application.Interfaces;

public interface IAuditLogRepository
{
    Task RecordAsync(
        string? userId,
        string userName,
        string role,
        string eventType,
        int? courseId,
        string result,
        string detail,
        CancellationToken cancellationToken = default);

    Task<PaginaDto<RegistroAuditoriaDto>> SearchAsync(
        FiltroAuditoriaDto filter,
        CancellationToken cancellationToken = default);
}
