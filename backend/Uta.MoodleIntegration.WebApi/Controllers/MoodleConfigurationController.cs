using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Uta.MoodleIntegration.WebApi.Services;

namespace Uta.MoodleIntegration.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Administrador técnico")]
[Route("api/moodle")]
public sealed class MoodleConfigurationController(
    MoodleConfigurationService configuration,
    MoodleConnectionTester connectionTester,
    ILogger<MoodleConfigurationController> logger) : ControllerBase
{
    [HttpGet("config")]
    [ProducesResponseType(typeof(MoodleConfigurationStatus), StatusCodes.Status200OK)]
    public async Task<ActionResult<MoodleConfigurationStatus>> GetConfiguration(
        CancellationToken cancellationToken)
    {
        return Ok(await configuration.GetStatusAsync(cancellationToken));
    }

    [HttpPut("config")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveConfiguration(
        MoodleConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await configuration.SaveAsync(request, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { detail = exception.Message });
        }
    }

    [HttpPost("connection-test")]
    [ProducesResponseType(typeof(MoodleConnectionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<MoodleConnectionResult>> TestConnection(
        CancellationToken cancellationToken)
    {
        var settings = await configuration.GetSettingsAsync(cancellationToken);
        if (settings is null)
        {
            return Conflict(new { detail = "Configura Moodle antes de verificar la conexión." });
        }

        try
        {
            return Ok(await connectionTester.TestAsync(settings, cancellationToken));
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning("Moodle rechazó o no completó la verificación: {Message}", exception.Message);
            return Problem(
                title: "No se pudo verificar la conexión con Moodle.",
                detail: exception.Message,
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Timeout al verificar la conexión con Moodle.");
            return Problem(
                title: "Moodle no respondió dentro del tiempo configurado.",
                statusCode: StatusCodes.Status504GatewayTimeout);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "Moodle devolvió datos inválidos durante la verificación.");
            return Problem(
                title: "Moodle devolvió una respuesta inválida.",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }
}
