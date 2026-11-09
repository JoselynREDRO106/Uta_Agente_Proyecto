using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using Uta.Middleware.Infrastructure.Moodle;

namespace Uta.MoodleIntegration.WebApi.Services;

public sealed class MoodleConnectionTester(IHttpClientFactory httpClientFactory)
{
    public async Task<MoodleConnectionResult> TestAsync(
        MoodleSettings settings,
        CancellationToken cancellationToken)
    {
        var endpoint = new Uri(new Uri(settings.BaseUrl.TrimEnd('/') + "/"), "webservice/rest/server.php");
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["wstoken"] = settings.Token,
                ["wsfunction"] = "core_webservice_get_site_info",
                ["moodlewsrestformat"] = "json"
            })
        };

        var client = httpClientFactory.CreateClient("MoodleConnectionTest");
        client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
        var stopwatch = Stopwatch.StartNew();
        using var response = await client.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException("Moodle devolvió una respuesta de conexión inválida.");
        }

        if (root.TryGetProperty("exception", out _))
        {
            var errorCode = root.TryGetProperty("errorcode", out var errorCodeElement)
                ? errorCodeElement.GetString()
                : null;
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(errorCode)
                    ? "Moodle rechazó la verificación de conexión."
                    : $"Moodle rechazó la verificación de conexión (código {errorCode}).");
        }

        var siteName = root.TryGetProperty("sitename", out var siteNameElement)
            ? siteNameElement.GetString()
            : null;
        if (string.IsNullOrWhiteSpace(siteName))
        {
            throw new JsonException("Moodle no devolvió el nombre del sitio.");
        }

        stopwatch.Stop();
        return new MoodleConnectionResult(siteName, stopwatch.ElapsedMilliseconds, DateTimeOffset.UtcNow);
    }
}

public sealed record MoodleConnectionResult(
    string SiteName,
    long ElapsedMilliseconds,
    DateTimeOffset CheckedAtUtc);
