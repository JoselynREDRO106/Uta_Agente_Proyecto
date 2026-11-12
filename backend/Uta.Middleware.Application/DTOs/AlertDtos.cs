namespace Uta.Middleware.Application.DTOs;

public sealed record ReglaAlertaDto(
    string Tipo,
    string Nombre,
    decimal Umbral,
    string Unidad,
    bool Activa);

public sealed record CambioReglaAlertaDto(
    string Tipo,
    decimal Umbral,
    bool Activa);

public sealed record GuardarReglasAlertaDto(
    IReadOnlyCollection<CambioReglaAlertaDto> Reglas);

public sealed record EvidenciaAlertaDto(
    string Item,
    string Valor,
    DateTime? FechaUtc,
    string Descripcion);

public sealed record AlertaDto(
    int EstudianteId,
    string Estudiante,
    string Tipo,
    string Regla,
    DateTime FechaUtc,
    string Estado,
    string Texto,
    IReadOnlyCollection<EvidenciaAlertaDto> Evidencias);

public sealed record PanelAlertasDto(
    int CursoId,
    string Fuente,
    DateTime UltimaSincronizacionUtc,
    string EstadoSincronizacion,
    bool DatosIncompletos,
    string? Aviso,
    IReadOnlyCollection<AlertaDto> Alertas);

public sealed record ActualizarEstadoAlertaDto(string Estado);
