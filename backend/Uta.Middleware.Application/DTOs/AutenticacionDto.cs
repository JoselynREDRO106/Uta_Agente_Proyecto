namespace Uta.Middleware.Application.DTOs;

public sealed record UsuarioAutenticadoDto(int Id, string Nombre, string Email, string Rol);

public sealed record CuentaUsuarioDto(
    int Id,
    string Nombre,
    string Email,
    string Rol,
    string HashPassword);

public sealed record CredencialesDto(string Email, string Password);

public sealed record UsuarioCreacionDto(
    string Nombre,
    string Email,
    string Password,
    string Rol,
    IReadOnlyCollection<int> CursosMoodle);

public sealed record TokenDto(string AccessToken, DateTime ExpiresAtUtc, string Nombre, string Rol);

public sealed record EstudianteCursoDto(
    int MoodleId,
    string Nombre,
    string Email,
    decimal? Promedio,
    string Estado);

public sealed record PaginaDto<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);

public sealed record IndicadoresCursoDto(
    int CursoId,
    int Estudiantes,
    decimal? Promedio,
    decimal? NotaMaxima,
    int ActividadesEvaluadas,
    int ActividadesTotales,
    decimal? ProgresoPorcentaje,
    DateTime? UltimaSincronizacionUtc,
    string? EstadoSincronizacion,
    bool DatosDisponibles,
    IReadOnlyCollection<DistribucionCalificacionDto> Distribucion);

public sealed record DistribucionCalificacionDto(string Rango, int Cantidad);
