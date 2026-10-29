using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Moodle;

/// <summary>
/// Implementación del cliente HTTP para los Web Services REST de Moodle.
/// Consume core_grades_get_grades (calificaciones) y core_course_get_contents (mapeo temático).
/// </summary>
public class MoodleHttpClient : IMoodleHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly MoodleSettings _settings;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public MoodleHttpClient(HttpClient httpClient, IOptions<MoodleSettings> options)
    {
        _httpClient = httpClient;
        _settings   = options.Value;
    }

    /// <inheritdoc/>
    public async Task<MoodleGradesResponseDto> GetGradesByCourseAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl("core_grades_get_grades", new Dictionary<string, string>
        {
            ["courseid"] = courseId.ToString()
        });

        return await GetAsync<MoodleGradesResponseDto>(url, cancellationToken)
               ?? new MoodleGradesResponseDto();
    }

    /// <inheritdoc/>
    public async Task<MoodleGradesResponseDto> GetGradesByUserAsync(
        int courseId,
        IEnumerable<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var extraParams = new Dictionary<string, string>
        {
            ["courseid"] = courseId.ToString()
        };

        // Moodle REST acepta userids[0], userids[1], ...
        int index = 0;
        foreach (var userId in userIds)
        {
            extraParams[$"userids[{index}]"] = userId.ToString();
            index++;
        }

        var url = BuildUrl("core_grades_get_grades", extraParams);
        return await GetAsync<MoodleGradesResponseDto>(url, cancellationToken)
               ?? new MoodleGradesResponseDto();
    }

    /// <inheritdoc/>
    public async Task<MoodleCourseModulesResponseDto> GetCourseContentsAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl("core_course_get_contents", new Dictionary<string, string>
        {
            ["courseid"] = courseId.ToString()
        });

        // core_course_get_contents devuelve un array JSON de secciones directamente
        var sections = await GetAsync<List<MoodleSectionDto>>(url, cancellationToken)
                       ?? [];

        return new MoodleCourseModulesResponseDto { Sections = sections };
    }

    // -------------------------------------------------------------------------
    // Helpers privados
    // -------------------------------------------------------------------------

    /// <summary>
    /// Construye la URL del endpoint REST de Moodle.
    /// Formato: {BaseUrl}/webservice/rest/server.php?wstoken={Token}&wsfunction={function}&moodlewsrestformat=json&{params}
    /// </summary>
    private string BuildUrl(string wsFunction, Dictionary<string, string>? extraParams = null)
    {
        var query = new Dictionary<string, string>
        {
            ["wstoken"]          = _settings.Token,
            ["wsfunction"]       = wsFunction,
            ["moodlewsrestformat"] = "json"
        };

        if (extraParams is not null)
        {
            foreach (var (key, value) in extraParams)
                query[key] = value;
        }

        var queryString = string.Join("&", query.Select(kv =>
            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

        return $"{_settings.BaseUrl.TrimEnd('/')}/webservice/rest/server.php?{queryString}";
    }

    /// <summary>
    /// Ejecuta GET y deserializa el JSON de respuesta.
    /// </summary>
    private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }
}
