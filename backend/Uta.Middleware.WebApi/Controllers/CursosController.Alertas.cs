using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.WebApi.Controllers;

public sealed partial class CursosController
{
    [HttpGet("{courseId:int}/alertas/reglas")]
    [Authorize(Roles = "Docente,Administrador técnico")]
    public async Task<ActionResult<IReadOnlyCollection<ReglaAlertaDto>>> ObtenerReglasAlerta(
        int courseId,
        CancellationToken cancellationToken)
    {
        var failure = await ValidateAnalysisAccessAsync(courseId, null, cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        try
        {
            return Ok(await analysisQueries!.GetAlertRulesAsync(
                courseId, GetBearerToken(), cancellationToken));
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

    [HttpPut("{courseId:int}/alertas/reglas")]
    [Authorize(Roles = "Docente,Administrador técnico")]
    public async Task<ActionResult<IReadOnlyCollection<ReglaAlertaDto>>> GuardarReglasAlerta(
        int courseId,
        GuardarReglasAlertaDto request,
        CancellationToken cancellationToken)
    {
        var failure = await ValidateAnalysisAccessAsync(
            courseId, "ReglasAlertaActualizadas", cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        try
        {
            var rules = await analysisQueries!.SaveAlertRulesAsync(
                courseId, request, GetBearerToken(), cancellationToken);
            await RecordAuditAsync(
                "ReglasAlertaActualizadas", courseId, "Exito", "Reglas de alerta actualizadas.", cancellationToken);
            return Ok(rules);
        }
        catch (HttpRequestException exception)
        {
            await RecordAuditAsync(
                "ReglasAlertaActualizadas", courseId, "Error", "No se pudieron guardar las reglas.", cancellationToken);
            return DownstreamFailure("Análisis", exception);
        }
        catch (JsonException exception)
        {
            return DownstreamInvalidResponse("Análisis", exception);
        }
    }

    [HttpGet("{courseId:int}/alertas")]
    [Authorize(Roles = "Docente,Administrador técnico")]
    public async Task<ActionResult<PanelAlertasDto>> ObtenerAlertas(
        int courseId,
        [FromQuery] string? estado,
        CancellationToken cancellationToken)
    {
        var failure = await ValidateAnalysisAccessAsync(courseId, null, cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        try
        {
            var panel = await analysisQueries!.GetAlertsAsync(
                courseId, estado, GetBearerToken(), cancellationToken);
            return panel is null ? NotFound() : Ok(panel);
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

    [HttpPatch("{courseId:int}/alertas/{studentId:int}/{tipo}/estado")]
    [Authorize(Roles = "Docente,Administrador técnico")]
    public async Task<ActionResult<ActualizarEstadoAlertaDto>> ActualizarEstadoAlerta(
        int courseId,
        int studentId,
        string tipo,
        ActualizarEstadoAlertaDto request,
        CancellationToken cancellationToken)
    {
        var failure = await ValidateAnalysisAccessAsync(
            courseId, "EstadoAlertaActualizado", cancellationToken);
        if (failure is not null)
        {
            return failure;
        }

        try
        {
            var state = await analysisQueries!.UpdateAlertStateAsync(
                courseId, studentId, tipo, request, GetBearerToken(), cancellationToken);
            if (state is null)
            {
                return NotFound();
            }

            await RecordAuditAsync(
                "EstadoAlertaActualizado",
                courseId,
                "Exito",
                $"Alerta {tipo} actualizada a {state.Estado}.",
                cancellationToken);
            return Ok(state);
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

    private async Task<ActionResult?> ValidateAnalysisAccessAsync(
        int courseId,
        string? auditEventOnDenied,
        CancellationToken cancellationToken)
    {
        if (courseId <= 0)
        {
            return BadRequest("El curso debe ser positivo.");
        }

        if (!await CanAccessCourseAsync(courseId, cancellationToken))
        {
            if (auditEventOnDenied is not null)
            {
                await RecordAuditAsync(
                    auditEventOnDenied, courseId, "Denegado", "Acceso al curso denegado.", cancellationToken);
            }

            return Forbid();
        }

        return analysisQueries is null
            ? Problem(
                title: "El servicio de Análisis no está configurado.",
                statusCode: StatusCodes.Status503ServiceUnavailable)
            : null;
    }
}
