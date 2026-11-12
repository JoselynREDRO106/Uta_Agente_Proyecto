using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.Application.Interfaces;

public interface IAcademicAgentClient
{
    Task<RespuestaAgenteCursoDto> AskAsync(
        string question,
        IndicadoresCursoDto indicators,
        AnalisisGrupalDto groupAnalysis,
        CancellationToken cancellationToken = default);
}
