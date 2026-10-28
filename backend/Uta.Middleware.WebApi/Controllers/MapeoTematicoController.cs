using Microsoft.AspNetCore.Mvc;
using Uta.Middleware.Application.Interfaces;
using Uta.Middleware.Domain.Entities;

namespace Uta.Middleware.WebApi.Controllers;

/// <summary>
/// Controlador REST para el mapeo temático de cursos desde Moodle.
/// Usa core_course_get_contents para obtener secciones y módulos (unidades temáticas).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MapeoTematicoController : ControllerBase
{
    private readonly ISincronizarMapeoTematicoUseCase _sincronizarMapeoTematico;
    private readonly ILogger<MapeoTematicoController> _logger;

    public MapeoTematicoController(
        ISincronizarMapeoTematicoUseCase sincronizarMapeoTematico,
        ILogger<MapeoTematicoController> logger)
    {
        _sincronizarMapeoTematico = sincronizarMapeoTematico;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene y retorna el mapeo temático (secciones → módulos) de un curso desde Moodle.
    /// Usa core_course_get_contents para recuperar la organización del contenido académico.
    /// </summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet("{courseId:int}")]
    [ProducesResponseType(typeof(IEnumerable<MapeoTematico>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> ObtenerMapeoTematico(
        [FromRoute] int courseId,
        CancellationToken cancellationToken)
    {
        if (courseId <= 0)
            return BadRequest("El courseId debe ser un entero positivo.");

        try
        {
            _logger.LogInformation(
                "Iniciando sincronización del mapeo temático para el curso {CourseId}.", courseId);

            var mapeos = await _sincronizarMapeoTematico
                .EjecutarAsync(courseId, cancellationToken);

            _logger.LogInformation(
                "Mapeo temático completado: {Count} items obtenidos del curso {CourseId}.",
                mapeos.Count(), courseId);

            return Ok(mapeos);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "Error al conectarse a Moodle para el mapeo del curso {CourseId}.", courseId);
            return StatusCode(StatusCodes.Status502BadGateway,
                "No se pudo conectar con el servidor Moodle. Verifique la configuración.");
        }
    }
}
