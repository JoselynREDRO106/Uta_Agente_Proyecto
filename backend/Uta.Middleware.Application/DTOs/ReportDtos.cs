namespace Uta.Middleware.Application.DTOs;

public sealed record GenerarInformeDto(string? Periodo);

public sealed record ContenidoInformeDto(
    string Curso,
    string Periodo,
    IndicadoresCursoDto Indicadores,
    IReadOnlyCollection<ResultadoActividadDto> ActividadesBajoUmbral,
    IReadOnlyCollection<string> Hallazgos,
    IReadOnlyCollection<string> Recomendaciones,
    string? Aviso);

public sealed record InformeResumenDto(
    Guid Id,
    int CursoId,
    string Periodo,
    DateTime FechaGeneracionUtc,
    DateTime FechaActualizacionDatosUtc,
    bool DatosIncompletos);

public sealed record InformeDto(
    Guid Id,
    int CursoId,
    string Periodo,
    DateTime FechaGeneracionUtc,
    DateTime FechaActualizacionDatosUtc,
    bool DatosIncompletos,
    ContenidoInformeDto Contenido);
