using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Docente,Administrador técnico")]
[Route("api/cursos")]
public sealed class AsistenteCursoController(
    IEduMonitorRepository repository,
    IAuditLogRepository auditLog,
    ILogger<AsistenteCursoController> logger,
    IAnalysisQueryClient? analysisQueries = null,
    IAcademicAgentClient? academicAgent = null) : ControllerBase
{
    [HttpPost("{courseId:int}/consultar")]
    [ProducesResponseType(typeof(RespuestaAsistenteCursoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<RespuestaAsistenteCursoDto>> Ask(
        int courseId,
        SolicitudAsistenteCursoDto request,
        CancellationToken cancellationToken)
    {
        var question = request.Pregunta?.Trim();
        if (courseId <= 0 || string.IsNullOrWhiteSpace(question) || question.Length > 1000)
        {
            return BadRequest("Indique un curso válido y una pregunta de 1 a 1000 caracteres.");
        }

        if (!await CanAccessCourseAsync(courseId, cancellationToken))
        {
            await RecordAuditAsync(courseId, "Denegado", cancellationToken);
            return Forbid();
        }

        if (academicAgent is null || analysisQueries is null)
        {
            return Problem(
                title: "El agente académico no está configurado.",
                detail: "Configure AcademicAgent__BaseUrl, AcademicAgent__ApiKey y AcademicAnalysis__BaseUrl.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        try
        {
            var indicators = await analysisQueries.GetCourseIndicatorsAsync(
                courseId, GetBearerToken(), cancellationToken);
            if (indicators is null)
            {
                return NotFound();
            }

            var groupAnalysis = await analysisQueries.GetGroupAnalysisAsync(
                courseId, 6m, 7, GetBearerToken(), cancellationToken);
            var answer = await academicAgent.AskAsync(
                question, indicators, groupAnalysis, cancellationToken);

            await RecordAuditAsync(courseId, "Exito", cancellationToken);
            return Ok(new RespuestaAsistenteCursoDto(
                answer.Respuesta,
                answer.Hechos,
                answer.Inferencias,
                answer.Recomendaciones,
                answer.Incertidumbre,
                "Moodle",
                indicators.UltimaSincronizacionUtc));
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(exception, "El agente académico no pudo responder para el curso {CourseId}.", courseId);
            await RecordAuditAsync(courseId, "Error", cancellationToken);
            return Problem(
                title: "El agente académico no está disponible.",
                statusCode: exception.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                    ? StatusCodes.Status503ServiceUnavailable
                    : StatusCodes.Status502BadGateway);
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "El agente académico devolvió una respuesta inválida para el curso {CourseId}.", courseId);
            await RecordAuditAsync(courseId, "Error", cancellationToken);
            return Problem(
                title: "El agente académico devolvió una respuesta inválida.",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }

    private async Task<bool> CanAccessCourseAsync(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Administrador técnico"))
        {
            return true;
        }

        if (User.HasClaim("course_assignments", "managed"))
        {
            return User.FindAll("course_id")
                .Any(claim => int.TryParse(claim.Value, out var assignedCourseId)
                    && assignedCourseId == courseId);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out var parsedUserId)
            && await repository.IsCourseAssignedAsync(parsedUserId, courseId, cancellationToken);
    }

    private string? GetBearerToken()
    {
        var authorization = Request.Headers.Authorization.ToString();
        return authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authorization["Bearer ".Length..].Trim()
            : null;
    }

    private Task RecordAuditAsync(int courseId, string result, CancellationToken cancellationToken) =>
        auditLog.RecordAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            User.Identity?.Name ?? "Usuario",
            User.FindFirstValue(ClaimTypes.Role) ?? "Desconocido",
            "ConsultaAgente",
            courseId,
            result,
            "Consulta agregada al agente; no se guardó el texto de la pregunta.",
            cancellationToken);
}
