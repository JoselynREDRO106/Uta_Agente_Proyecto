namespace Uta.Middleware.Infrastructure.Moodle;

public sealed class MoodleIntegrationBridgeSettings
{
    public const string SectionName = "MoodleIntegration";

    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
