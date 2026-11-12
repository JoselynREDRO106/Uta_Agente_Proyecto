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

    Task<SeguimientoEstudianteDto?> GetStudentTrackingAsync(
        int courseId,
        int studentId,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<AnalisisGrupalDto> GetGroupAnalysisAsync(
        int courseId,
        decimal threshold,
        int minimumGrades,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RecomendacionDto>> GetRecommendationsAsync(
        int courseId,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<ResultadoGenerarRecomendacionesDto> GenerateRecommendationsAsync(
        int courseId,
        SolicitudRecomendacionesGatewayDto request,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<RecomendacionDto?> UpdateRecommendationAsync(
        int courseId,
        Guid recommendationId,
        ActualizarEstadoRecomendacionDto request,
        string? bearerToken,
        CancellationToken cancellationToken = default);
}
