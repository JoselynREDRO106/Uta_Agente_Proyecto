using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Administrador técnico")]
[Route("api/auditoria")]
public sealed class AuditoriaController(IAuditLogRepository auditLog) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginaDto<RegistroAuditoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaDto<RegistroAuditoriaDto>>> Search(
        [FromQuery] string? tipo,
        [FromQuery] DateTimeOffset? desdeUtc,
        [FromQuery] DateTimeOffset? hastaUtc,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (pagina < 1 || tamanoPagina is < 1 or > 100
            || ((long)pagina - 1) * tamanoPagina > int.MaxValue)
        {
            return BadRequest("pagina debe ser positivo y tamanoPagina debe estar entre 1 y 100.");
        }

        if (desdeUtc.HasValue && hastaUtc.HasValue && desdeUtc > hastaUtc)
        {
            return BadRequest("desdeUtc debe ser anterior o igual a hastaUtc.");
        }

        if (tipo is not null
            && tipo.Trim() is not ("Sincronizacion" or "ConsultaAgente"
                or "AnalisisGrupal" or "RecomendacionGenerada"
                or "EstadoRecomendacionActualizado" or "ReglasAlertaActualizadas"
                or "EstadoAlertaActualizado" or "InformeGenerado" or "InformeDescargado"))
        {
            return BadRequest("El tipo de evento no es válido.");
        }

        var result = await auditLog.SearchAsync(new FiltroAuditoriaDto(
            tipo?.Trim(),
            desdeUtc,
            hastaUtc,
            pagina,
            tamanoPagina), cancellationToken);
        return Ok(result);
    }
}
