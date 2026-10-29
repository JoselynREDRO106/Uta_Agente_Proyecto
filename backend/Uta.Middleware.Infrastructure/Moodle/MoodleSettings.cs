namespace Uta.Middleware.Infrastructure.Moodle;

/// <summary>
/// Opciones de configuración para el cliente HTTP de Moodle.
/// Se inyectan desde appsettings.json (sección "MoodleSettings").
/// </summary>
public class MoodleSettings
{
    public const string SectionName = "MoodleSettings";

    /// <summary>URL base de la instancia Moodle, ej: http://localhost:8080/moodle</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Token REST generado en Moodle (Administración → Web Services → Tokens).</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Timeout en segundos para las solicitudes HTTP. Default: 30.</summary>
    public int TimeoutSeconds { get; set; } = 30;
}
