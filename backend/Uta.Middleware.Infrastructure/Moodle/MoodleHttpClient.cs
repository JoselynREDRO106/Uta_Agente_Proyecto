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

    public async Task<MoodleCourseDto> GetCourseAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var courses = await GetAsync<List<MoodleCourseDto>>(
            BuildUrl("core_course_get_courses", new Dictionary<string, string>
            {
                ["options[ids][0]"] = courseId.ToString()
            }),
            cancellationToken);

        return courses?.SingleOrDefault(course => course.Id == courseId)
            ?? throw new HttpRequestException($"Moodle no devolvió el curso {courseId}.");
    }

    public async Task<IReadOnlyCollection<MoodleEnrolledUserDto>> GetEnrolledUsersAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<MoodleEnrolledUserDto>>(
                   BuildUrl("core_enrol_get_enrolled_users", new Dictionary<string, string>
                   {
                       ["courseid"] = courseId.ToString()
                   }),
                   cancellationToken)
               ?? [];
    }

    /// <inheritdoc/>
    public async Task<MoodleGradesResponseDto> GetGradesByCourseAsync(
        int courseId,
        IEnumerable<int> userIds,
        CancellationToken cancellationToken = default)
    {
        return await GetGradesByUserAsync(courseId, userIds, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<MoodleGradesResponseDto> GetGradesByUserAsync(
        int courseId,
        IEnumerable<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.Where(id => id > 0).Distinct().ToArray();
        if (ids.Length == 0)
        {
            return new MoodleGradesResponseDto();
        }

        using var concurrencyLimit = new SemaphoreSlim(4);
        var responses = await Task.WhenAll(ids.Select(async userId =>
        {
            await concurrencyLimit.WaitAsync(cancellationToken);
            try
            {
                var response = await GetAsync<MoodleUserGradeItemsResponseDto>(
                    BuildUrl("gradereport_user_get_grade_items", new Dictionary<string, string>
                    {
                        ["courseid"] = courseId.ToString(),
                        ["userid"] = userId.ToString()
                    }),
                    cancellationToken);
                return (UserId: userId, Response: response ?? new MoodleUserGradeItemsResponseDto());
            }
            finally
            {
                concurrencyLimit.Release();
            }
        }));

        var items = responses
            .SelectMany(result => result.Response.UserGrades.SelectMany(user => user.GradeItems))
            .Where(item => item.ItemType.Equals("mod", StringComparison.OrdinalIgnoreCase))
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .Select(item => new MoodleGradeItemDto
            {
                Id = item.Id,
                ItemName = item.ItemName,
                ItemType = item.ItemType,
                ItemModule = item.ItemModule,
                ItemInstance = item.ItemInstance,
                CourseModuleId = item.CourseModuleId,
                GradeMin = item.GradeMin ?? 0,
                GradeMax = item.GradeMax ?? 0
            })
            .ToList();
        var grades = responses.SelectMany(result =>
            result.Response.UserGrades
                .Where(user => user.UserId > 0)
                .SelectMany(user => user.GradeItems
                    .Where(item => item.ItemType.Equals("mod", StringComparison.OrdinalIgnoreCase))
                    .Select(item => new MoodleUserGradeDto
                {
                    UserId = user.UserId,
                    ItemId = item.Id,
                    Grade = item.GradeRaw?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                    DateSubmitted = item.GradeDateSubmitted?.ToString(
                        System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                    DateGraded = item.GradeDateGraded?.ToString(
                        System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty
                })))
            .ToList();
        var warnings = responses.SelectMany(result => result.Response.Warnings)
            .Select(warning => warning.ValueKind == JsonValueKind.Object
                && warning.TryGetProperty("message", out var message)
                ? message.GetString() ?? warning.GetRawText()
                : warning.GetRawText())
            .ToList();

        return new MoodleGradesResponseDto { Items = items, Grades = grades, Warnings = warnings };
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
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(content);
        if (document.RootElement.ValueKind == JsonValueKind.Object
            && document.RootElement.TryGetProperty("exception", out _))
        {
            var errorCode = document.RootElement.TryGetProperty("errorcode", out var code)
                ? code.GetString()
                : "unknown";
            var message = document.RootElement.TryGetProperty("message", out var details)
                ? details.GetString()
                : "Moodle devolvió un error en la respuesta.";
            throw new HttpRequestException(
                $"Moodle Web Service error ({errorCode}): {message}");
        }

        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }
}
