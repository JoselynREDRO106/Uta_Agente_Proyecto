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
    DateOnly? Fecha,
    decimal? PromedioCurso,
    decimal Escala,
    int NumCalificaciones,
    bool DatosInsuficientes,
    int ItemId,
    string Actividad,
    decimal? Variacion);

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
    DateTime FechaUtc,
    int? EstudianteId = null,
    string? Estudiante = null,
    DateTime? FechaEvidenciaUtc = null);

public sealed record SolicitudGenerarRecomendacionesDto(
    decimal Umbral = 6m,
    int MinimoCalificaciones = 7,
    int? EstudianteId = null);

public sealed record ResultadoGenerarRecomendacionesDto(
    int Creadas,
    IReadOnlyCollection<RecomendacionDto> Recomendaciones,
    string? Motivo = null);

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
    string Incertidumbre,
    IReadOnlyCollection<string>? Herramientas = null);

public sealed record RespuestaAsistenteCursoDto(
    string Respuesta,
    IReadOnlyCollection<string> Hechos,
    IReadOnlyCollection<string> Inferencias,
    IReadOnlyCollection<string> Recomendaciones,
    string Incertidumbre,
    string Fuente,
    DateTime? UltimaSincronizacionUtc,
    IReadOnlyCollection<string>? Herramientas = null);

public sealed record SolicitudRecomendacionesGatewayDto(
    decimal Umbral = 6m,
    int MinimoCalificaciones = 7,
    int? EstudianteId = null);

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

public sealed record ActividadCalificacionDto(
    int ItemId,
    string Nombre,
    decimal EscalaMinima,
    decimal EscalaMaxima);

public sealed record NotaLibroDto(int ItemId, decimal? Nota);

public sealed record FilaLibroCalificacionesDto(
    int MoodleId,
    string Nombre,
    string Email,
    IReadOnlyCollection<NotaLibroDto> Notas);

public sealed record LibroCalificacionesDto(
    int CursoId,
    string Fuente,
    DateTime UltimaSincronizacionUtc,
    string EstadoSincronizacion,
    IReadOnlyCollection<ActividadCalificacionDto> Actividades,
    IReadOnlyCollection<FilaLibroCalificacionesDto> Estudiantes);

public sealed record SeguimientoEstudianteDto(
    int CursoId,
    int EstudianteId,
    string Nombre,
    string Fuente,
    DateTime UltimaSincronizacionUtc,
    string EstadoSincronizacion,
    IReadOnlyCollection<SeguimientoNotaDto> Notas,
    IReadOnlyCollection<SeguimientoActividadDto> Actividades);
