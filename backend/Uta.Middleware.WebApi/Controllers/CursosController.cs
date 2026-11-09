using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.WebApi.Controllers;

/// <summary>Gestiona la sincronización de cursos Moodle y su consulta desde EduMonitor.</summary>
[ApiController]
[Route("api/cursos")]
public sealed class CursosController(
    IGestionarCursosUseCase gestionarCursos,
    ILogger<CursosController> logger) : ControllerBase
{
    /// <summary>Lista los cursos sincronizados y guardados en EduMonitor.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CursoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CursoResponse>>> ObtenerCursos(
        CancellationToken cancellationToken)
    {
        var courses = await gestionarCursos.ObtenerAsync(cancellationToken);
        return Ok(courses.Select(course => new CursoResponse(course.MoodleId, course.Name)).ToArray());
    }

    /// <summary>Obtiene un curso de Moodle y lo guarda o actualiza en EduMonitor.</summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    [HttpPost("sincronizar/{courseId:int}")]
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
}

/// <summary>Datos de un curso de Moodle expuestos por la API académica.</summary>
public sealed record CursoResponse(int Id, string Name);
