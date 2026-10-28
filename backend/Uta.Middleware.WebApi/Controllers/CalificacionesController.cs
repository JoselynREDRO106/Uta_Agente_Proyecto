using Microsoft.AspNetCore.Mvc;
using Uta.Middleware.Application.Interfaces;
using Uta.Middleware.Domain.Entities;

namespace Uta.Middleware.WebApi.Controllers;

/// <summary>
/// Controlador REST para la sincronización de calificaciones desde Moodle.
/// Endpoint principal: HU-001 - Ingesta de datos desde core_grades_get_grades.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalificacionesController : ControllerBase
{
    private readonly ISincronizarCalificacionesUseCase _sincronizarCalificaciones;
    private readonly ILogger<CalificacionesController> _logger;

    public CalificacionesController(
        ISincronizarCalificacionesUseCase sincronizarCalificaciones,
        ILogger<CalificacionesController> logger)
    {
        _sincronizarCalificaciones = sincronizarCalificaciones;
        _logger = logger;
    }

    /// <summary>
    /// Sincroniza y retorna todas las calificaciones de un curso desde Moodle.
    /// Consume internamente core_grades_get_grades del Web Service REST de Moodle.
    /// </summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet("sincronizar/{courseId:int}")]
    [ProducesResponseType(typeof(IEnumerable<Calificacion>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> SincronizarCalificaciones(
        [FromRoute] int courseId,
        CancellationToken cancellationToken)
    {
        if (courseId <= 0)
            return BadRequest("El courseId debe ser un entero positivo.");

        try
        {
            _logger.LogInformation(
                "Iniciando sincronización de calificaciones para el curso {CourseId}.", courseId);

            var calificaciones = await _sincronizarCalificaciones
                .EjecutarAsync(courseId, cancellationToken);

            _logger.LogInformation(
                "Sincronización completada: {Count} calificaciones obtenidas del curso {CourseId}.",
                calificaciones.Count(), courseId);

            return Ok(calificaciones);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "Error al conectarse a Moodle para el curso {CourseId}.", courseId);
            return StatusCode(StatusCodes.Status502BadGateway,
                "No se pudo conectar con el servidor Moodle. Verifique la configuración.");
        }
    }
}
