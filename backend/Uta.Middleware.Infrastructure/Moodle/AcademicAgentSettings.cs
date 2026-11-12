namespace Uta.Middleware.Infrastructure.Moodle;

public sealed class AcademicAgentSettings
{
    public const string SectionName = "AcademicAgent";
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
