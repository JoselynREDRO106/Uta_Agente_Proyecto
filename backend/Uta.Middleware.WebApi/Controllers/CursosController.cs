using System.Text.Json;
using System.Security.Claims;
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
    ILogger<CursosController> logger) : ControllerBase
{
    /// <summary>Lista los cursos sincronizados y guardados en EduMonitor.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CursoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CursoResponse>>> ObtenerCursos(
        CancellationToken cancellationToken)
    {
        int? userId = User.IsInRole("Administrador técnico")
            ? null
            : int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var courses = await gestionarCursos.ObtenerAsync(userId, cancellationToken);
        return Ok(courses.Select(course => new CursoResponse(course.MoodleId, course.Name)).ToArray());
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

        var indicators = await repository.GetCourseIndicatorsAsync(courseId, cancellationToken);
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

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out var parsedUserId)
            && await repository.IsCourseAssignedAsync(parsedUserId, courseId, cancellationToken);
    }
}

/// <summary>Datos de un curso de Moodle expuestos por la API académica.</summary>
public sealed record CursoResponse(int Id, string Name);
