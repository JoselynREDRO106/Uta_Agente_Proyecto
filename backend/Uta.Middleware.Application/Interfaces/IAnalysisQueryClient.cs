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

    Task<IReadOnlyCollection<ReglaAlertaDto>> GetAlertRulesAsync(
        int courseId,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ReglaAlertaDto>> SaveAlertRulesAsync(
        int courseId,
        GuardarReglasAlertaDto request,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<PanelAlertasDto?> GetAlertsAsync(
        int courseId,
        string? status,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<ActualizarEstadoAlertaDto?> UpdateAlertStateAsync(
        int courseId,
        int studentId,
        string alertType,
        ActualizarEstadoAlertaDto request,
        string? bearerToken,
        CancellationToken cancellationToken = default);
}
