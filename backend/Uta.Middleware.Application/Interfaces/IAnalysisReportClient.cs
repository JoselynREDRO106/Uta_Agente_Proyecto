using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.Application.Interfaces;

public sealed record ArchivoInforme(byte[] Contenido, string ContentType, string NombreArchivo);

public interface IAnalysisReportClient
{
    Task<InformeDto> GenerateReportAsync(
        int courseId,
        GenerarInformeDto request,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InformeResumenDto>> GetReportsAsync(
        int courseId,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<InformeDto?> GetReportAsync(
        int courseId,
        Guid reportId,
        string? bearerToken,
        CancellationToken cancellationToken = default);

    Task<ArchivoInforme?> DownloadReportAsync(
        int courseId,
        Guid reportId,
        string? bearerToken,
        CancellationToken cancellationToken = default);
}
