using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.Application.Interfaces;

public interface IAnalysisQueryClient
{
    Task<IReadOnlyCollection<CursoDto>> GetCoursesAsync(
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<PaginaDto<EstudianteCursoDto>> GetCourseStudentsAsync(
        int courseId,
        string? search,
        string? status,
        int page,
        int pageSize,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<IndicadoresCursoDto?> GetCourseIndicatorsAsync(
        int courseId,
        string? bearerToken,
        CancellationToken cancellationToken = default);
}
