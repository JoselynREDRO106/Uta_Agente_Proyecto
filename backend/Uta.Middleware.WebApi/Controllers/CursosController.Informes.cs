using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.WebApi.Controllers;

public sealed partial class CursosController
{
    [HttpPost("{courseId:int}/informes")]
    [Authorize(Roles = "Docente,Administrador técnico")]
    public async Task<ActionResult<InformeDto>> GenerarInforme(
        int courseId,
        GenerarInformeDto request,
        CancellationToken cancellationToken)
    {
        var (client, failure) = await ValidateReportAccessAsync(courseId, "InformeGenerado", cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        try
        {
            var report = await client!.GenerateReportAsync(courseId, request, GetBearerToken(), cancellationToken);
            await RecordAuditAsync(
                "InformeGenerado", courseId, "Exito", $"Informe {report.Id} generado.", cancellationToken);
            return Ok(report);
        }
        catch (HttpRequestException exception)
        {
            await RecordAuditAsync(
                "InformeGenerado", courseId, "Error", "No se pudo generar el informe.", cancellationToken);
            return DownstreamFailure("Análisis", exception);
        }
        catch (JsonException exception)
        {
            return DownstreamInvalidResponse("Análisis", exception);
        }
    }

    [HttpGet("{courseId:int}/informes")]
    [Authorize(Roles = "Docente,Administrador técnico")]
    public async Task<ActionResult<IReadOnlyCollection<InformeResumenDto>>> ListarInformes(
        int courseId,
        CancellationToken cancellationToken)
    {
        var (client, failure) = await ValidateReportAccessAsync(courseId, null, cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        try
        {
            return Ok(await client!.GetReportsAsync(courseId, GetBearerToken(), cancellationToken));
        }
        catch (HttpRequestException exception)
        {
            return DownstreamFailure("Análisis", exception);
        }
        catch (JsonException exception)
        {
            return DownstreamInvalidResponse("Análisis", exception);
        }
    }

    [HttpGet("{courseId:int}/informes/{reportId:guid}")]
    [Authorize(Roles = "Docente,Administrador técnico")]
    public async Task<ActionResult<InformeDto>> ObtenerInforme(
        int courseId,
        Guid reportId,
        CancellationToken cancellationToken)
    {
        var (client, failure) = await ValidateReportAccessAsync(courseId, null, cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        try
        {
            var report = await client!.GetReportAsync(courseId, reportId, GetBearerToken(), cancellationToken);
            return report is null ? NotFound() : Ok(report);
        }
        catch (HttpRequestException exception)
        {
            return DownstreamFailure("Análisis", exception);
        }
        catch (JsonException exception)
        {
            return DownstreamInvalidResponse("Análisis", exception);
        }
    }

    [HttpGet("{courseId:int}/informes/{reportId:guid}/descargar")]
    [Authorize(Roles = "Docente,Administrador técnico")]
    public async Task<IActionResult> DescargarInforme(
        int courseId,
        Guid reportId,
        CancellationToken cancellationToken)
    {
        var (client, failure) = await ValidateReportAccessAsync(courseId, "InformeDescargado", cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        try
        {
            var file = await client!.DownloadReportAsync(courseId, reportId, GetBearerToken(), cancellationToken);
            if (file is null)
            {
                return NotFound();
            }

            await RecordAuditAsync(
                "InformeDescargado", courseId, "Exito", $"Informe {reportId} descargado.", cancellationToken);
            return File(file.Contenido, file.ContentType, file.NombreArchivo);
        }
        catch (HttpRequestException exception)
        {
            return DownstreamFailure("Análisis", exception);
        }
    }

    private async Task<(IAnalysisReportClient? Client, ActionResult? Failure)> ValidateReportAccessAsync(
        int courseId,
        string? auditEventOnDenied,
        CancellationToken cancellationToken)
    {
        if (courseId <= 0)
        {
            return (null, BadRequest("El curso debe ser positivo."));
        }

        if (!await CanAccessCourseAsync(courseId, cancellationToken))
        {
            if (auditEventOnDenied is not null)
            {
                await RecordAuditAsync(
                    auditEventOnDenied, courseId, "Denegado", "Acceso al curso denegado.", cancellationToken);
            }

            return (null, Forbid());
        }

        var client = HttpContext.RequestServices.GetService<IAnalysisReportClient>();
        return client is null
            ? (null, Problem(
                title: "El servicio de Análisis no está configurado.",
                statusCode: StatusCodes.Status503ServiceUnavailable))
            : (client, null);
    }
}
