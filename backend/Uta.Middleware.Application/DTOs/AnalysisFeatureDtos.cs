namespace Uta.Middleware.Application.DTOs;

public sealed record ResultadoActividadDto(
    int ItemId,
    string Nombre,
    decimal? Promedio,
    decimal Escala,
    int NumCalificaciones,
    bool BajoUmbral,
    bool DatosInsuficientes);

public sealed record PuntoTendenciaDto(
    DateOnly Fecha,
    decimal? PromedioCurso,
    decimal Escala,
    int NumCalificaciones,
    bool DatosInsuficientes);

public sealed record AnalisisGrupalDto(
    int CursoId,
    decimal UmbralBajo,
    int MinimoCalificaciones,
    string Fuente,
    DateTime UltimaSincronizacionUtc,
    IReadOnlyCollection<ResultadoActividadDto> Actividades,
    IReadOnlyCollection<PuntoTendenciaDto> Tendencia);

public sealed record RecomendacionDto(
    Guid Id,
    int CursoId,
    int ActividadId,
    string Actividad,
    decimal Promedio,
    decimal Escala,
    int NumCalificaciones,
    decimal Umbral,
    string Texto,
    string Justificacion,
    string Estado,
    DateTime FechaUtc);

public sealed record SolicitudGenerarRecomendacionesDto(
    decimal Umbral = 6m,
    int MinimoCalificaciones = 7);

public sealed record ResultadoGenerarRecomendacionesDto(
    int Creadas,
    IReadOnlyCollection<RecomendacionDto> Recomendaciones);

public sealed record ActualizarEstadoRecomendacionDto(
    string Estado,
    string? Texto = null);

public sealed record RegistroAuditoriaDto(
    long Id,
    DateTime FechaUtc,
    string Usuario,
    string Rol,
    string Tipo,
    int? CursoId,
    string Resultado,
    string Detalle);

public sealed record FiltroAuditoriaDto(
    string? Tipo,
    DateTimeOffset? DesdeUtc,
    DateTimeOffset? HastaUtc,
    int Pagina,
    int TamanoPagina);

public sealed record SolicitudAsistenteCursoDto(string Pregunta);

public sealed record SolicitudAgenteCursoDto(
    string Pregunta,
    IndicadoresCursoDto Indicadores,
    AnalisisGrupalDto AnalisisGrupal);

public sealed record RespuestaAgenteCursoDto(
    string Respuesta,
    IReadOnlyCollection<string> Hechos,
    IReadOnlyCollection<string> Inferencias,
    IReadOnlyCollection<string> Recomendaciones,
    string Incertidumbre);

public sealed record RespuestaAsistenteCursoDto(
    string Respuesta,
    IReadOnlyCollection<string> Hechos,
    IReadOnlyCollection<string> Inferencias,
    IReadOnlyCollection<string> Recomendaciones,
    string Incertidumbre,
    string Fuente,
    DateTime? UltimaSincronizacionUtc);

public sealed record SolicitudRecomendacionesGatewayDto(
    decimal Umbral = 6m,
    int MinimoCalificaciones = 7);

public sealed record SeguimientoNotaDto(
    int ItemId,
    string Item,
    DateTime? FechaUtc,
    decimal? Nota,
    decimal EscalaMinima,
    decimal EscalaMaxima);

public sealed record SeguimientoActividadDto(
    int ItemId,
    string Actividad,
    string Estado);

public sealed record SeguimientoEstudianteDto(
    int CursoId,
    int EstudianteId,
    string Nombre,
    string Fuente,
    DateTime UltimaSincronizacionUtc,
    string EstadoSincronizacion,
    IReadOnlyCollection<SeguimientoNotaDto> Notas,
    IReadOnlyCollection<SeguimientoActividadDto> Actividades);
