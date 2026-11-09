using System.Text.Json;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.WebApi.Controllers;

/// <summary>Gestiona la sincronización de cursos Moodle y su consulta desde EduMonitor.</summary>
[ApiController]
[Authorize]
[Route("api/cursos")]
public sealed class CursosController(
    IGestionarCursosUseCase gestionarCursos,
    IEduMonitorRepository repository,
    ILogger<CursosController> logger,
    IAnalysisQueryClient? analysisQueries = null,
    IMoodleAcademicSyncClient? integrationSync = null) : ControllerBase
{
    /// <summary>Lista los cursos sincronizados y guardados en EduMonitor.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CursoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CursoResponse>>> ObtenerCursos(
        CancellationToken cancellationToken)
    {
        var isAdmin = User.IsInRole("Administrador técnico");
        if (analysisQueries is not null
            && (isAdmin || User.HasClaim("course_assignments", "managed")))
        {
            try
            {
                var coursesFromAnalysis = await analysisQueries.GetCoursesAsync(GetBearerToken(), cancellationToken);
                return Ok(coursesFromAnalysis.Select(course =>
                    new CursoResponse(course.MoodleId, course.Name)).ToArray());
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

        IReadOnlyCollection<CursoDto> courses;
        if (!isAdmin && User.HasClaim("course_assignments", "managed"))
        {
            courses = await repository.GetCoursesForMoodleIdsAsync(
                GetMoodleCourseIdsFromClaims(), cancellationToken);
        }
        else
        {
            int? userId = isAdmin
                ? null
                : int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            courses = await gestionarCursos.ObtenerAsync(userId, cancellationToken);
        }

        return Ok(courses.Select(course => new CursoResponse(course.MoodleId, course.Name)).ToArray());
    }

    [HttpGet("sincronizaciones")]
    [Authorize(Roles = "Administrador técnico")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SincronizacionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SincronizacionDto>>> ObtenerHistorialSincronizaciones(
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        if (limit is < 1 or > 100)
        {
            return BadRequest("El límite debe estar entre 1 y 100.");
        }

        if (integrationSync is not null)
        {
            try
            {
                return Ok(await integrationSync.GetSyncHistoryAsync(limit, cancellationToken));
            }
            catch (HttpRequestException exception)
            {
                return DownstreamFailure("Integración Moodle", exception);
            }
            catch (JsonException exception)
            {
                return DownstreamInvalidResponse("Integración Moodle", exception);
            }
        }

        return Ok(await repository.GetSyncHistoryAsync(limit, cancellationToken));
    }

    [HttpGet("{courseId:int}/estudiantes")]
    [ProducesResponseType(typeof(PaginaDto<EstudianteCursoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginaDto<EstudianteCursoDto>>> ObtenerEstudiantes(
        int courseId,
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0 || page < 1 || pageSize is < 1 or > 100
            || ((long)page - 1) * pageSize > int.MaxValue)
        {
            return BadRequest("El curso debe ser positivo; page >= 1 y pageSize debe estar entre 1 y 100.");
        }

        if (!await CanAccessCourseAsync(courseId, cancellationToken))
        {
            return Forbid();
        }

        if (status is not null
            && status.Trim().ToLowerInvariant() is not ("calificado" or "pendiente" or "sin_entrega" or "sin_datos"))
        {
            return BadRequest("El filtro status debe ser calificado, pendiente, sin_entrega o sin_datos.");
        }

        if (analysisQueries is not null && User.HasClaim("course_assignments", "managed"))
        {
            try
            {
                return Ok(await analysisQueries.GetCourseStudentsAsync(
                    courseId,
                    search,
                    status?.Trim().ToLowerInvariant(),
                    page,
                    pageSize,
                    GetBearerToken(),
                    cancellationToken));
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

        return Ok(await repository.GetCourseStudentsAsync(
            courseId, search, status?.Trim().ToLowerInvariant(), page, pageSize, cancellationToken));
    }

    [HttpGet("{courseId:int}/indicadores")]
    [ProducesResponseType(typeof(IndicadoresCursoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndicadoresCursoDto>> ObtenerIndicadores(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessCourseAsync(courseId, cancellationToken))
        {
            return Forbid();
        }

        IndicadoresCursoDto? indicators;
        if (analysisQueries is not null && User.HasClaim("course_assignments", "managed"))
        {
            try
            {
                indicators = await analysisQueries.GetCourseIndicatorsAsync(
                    courseId, GetBearerToken(), cancellationToken);
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
        else
        {
            indicators = await repository.GetCourseIndicatorsAsync(courseId, cancellationToken);
        }

        return indicators is null ? NotFound() : Ok(indicators);
    }

    /// <summary>Obtiene un curso de Moodle y lo guarda o actualiza en EduMonitor.</summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    [HttpPost("sincronizar/{courseId:int}")]
    [Authorize(Roles = "Administrador técnico")]
    [ProducesResponseType(typeof(CursoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<CursoResponse>> SincronizarCurso(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (courseId <= 0)
        {
            return BadRequest("El courseId debe ser un entero positivo.");
        }

        try
        {
            var course = await gestionarCursos.SincronizarAsync(courseId, cancellationToken);
            return Ok(new CursoResponse(course.MoodleId, course.Name));
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(exception, "No se pudo obtener el curso {CourseId} desde Moodle.", courseId);
            return Problem(
                title: "No se pudo sincronizar el curso desde Moodle.",
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "Moodle devolvió una respuesta inválida para el curso {CourseId}.", courseId);
            return Problem(
                title: "Moodle devolvió una respuesta inválida.",
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
            return GetMoodleCourseIdsFromClaims().Contains(courseId);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out var parsedUserId)
            && await repository.IsCourseAssignedAsync(parsedUserId, courseId, cancellationToken);
    }

    private IReadOnlyCollection<int> GetMoodleCourseIdsFromClaims() =>
        User.FindAll("course_id")
            .Select(claim => int.TryParse(claim.Value, out var courseId) ? courseId : 0)
            .Where(courseId => courseId > 0)
            .Distinct()
            .ToArray();

    private string? GetBearerToken()
    {
        var authorization = Request.Headers.Authorization.ToString();
        return authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authorization["Bearer ".Length..].Trim()
            : null;
    }

    private ObjectResult DownstreamFailure(string service, HttpRequestException exception)
    {
        logger.LogError(exception, "El servicio {Service} no pudo completar la solicitud.", service);
        var statusCode = exception.StatusCode switch
        {
            HttpStatusCode.NotFound => StatusCodes.Status404NotFound,
            HttpStatusCode.Forbidden => StatusCodes.Status403Forbidden,
            HttpStatusCode.BadRequest => StatusCodes.Status400BadRequest,
            null => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status502BadGateway
        };
        return Problem(
            title: $"El servicio de {service} no está disponible.",
            statusCode: statusCode);
    }

    private ObjectResult DownstreamInvalidResponse(string service, JsonException exception)
    {
        logger.LogError(exception, "El servicio {Service} devolvió datos inválidos.", service);
        return Problem(
            title: $"El servicio de {service} devolvió una respuesta inválida.",
            statusCode: StatusCodes.Status502BadGateway);
    }
}

/// <summary>Datos de un curso de Moodle expuestos por la API académica.</summary>
public sealed record CursoResponse(int Id, string Name);
