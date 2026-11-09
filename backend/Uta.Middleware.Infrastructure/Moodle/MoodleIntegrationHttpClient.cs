using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Moodle;

public sealed class MoodleIntegrationHttpClient(
    HttpClient httpClient,
    IOptions<MoodleIntegrationBridgeSettings> options) : IMoodleHttpClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _apiKey = options.Value.ApiKey;

    public Task<MoodleCourseDto> GetCourseAsync(int courseId, CancellationToken cancellationToken = default) =>
        SendAsync<MoodleCourseDto>(
            HttpMethod.Get, $"internal/moodle/courses/{courseId}", null, cancellationToken);

    public Task<IReadOnlyCollection<MoodleEnrolledUserDto>> GetEnrolledUsersAsync(
        int courseId,
        CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyCollection<MoodleEnrolledUserDto>>(
            HttpMethod.Get, $"internal/moodle/courses/{courseId}/enrolled-users", null, cancellationToken);

    public Task<MoodleGradesResponseDto> GetGradesByCourseAsync(
        int courseId,
        IEnumerable<int> userIds,
        CancellationToken cancellationToken = default) =>
        GetGradesByUserAsync(courseId, userIds, cancellationToken);

    public Task<MoodleGradesResponseDto> GetGradesByUserAsync(
        int courseId,
        IEnumerable<int> userIds,
        CancellationToken cancellationToken = default) =>
        SendAsync<MoodleGradesResponseDto>(
            HttpMethod.Post,
            $"internal/moodle/courses/{courseId}/grades",
            new GradeRequest(userIds.Where(id => id > 0).Distinct().ToArray()),
            cancellationToken);

    public Task<MoodleCourseModulesResponseDto> GetCourseContentsAsync(
        int courseId,
        CancellationToken cancellationToken = default) =>
        SendAsync<MoodleCourseModulesResponseDto>(
            HttpMethod.Get, $"internal/moodle/courses/{courseId}/contents", null, cancellationToken);

    private async Task<T> SendAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.TryAddWithoutValidation("X-Integration-Key", _apiKey);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var statusCode = (int)response.StatusCode;
            throw new HttpRequestException(
                $"El servicio de Integración Moodle respondió HTTP {statusCode}.",
                null,
                response.StatusCode);
        }

        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return result ?? throw new JsonException(
            "El servicio de Integración Moodle devolvió una respuesta vacía o inválida.");
    }

    private sealed record GradeRequest(IReadOnlyCollection<int> UserIds);
}
