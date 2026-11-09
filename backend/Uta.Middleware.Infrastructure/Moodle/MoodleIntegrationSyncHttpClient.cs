using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Moodle;

public sealed class MoodleIntegrationSyncHttpClient(
    HttpClient httpClient,
    IOptions<MoodleIntegrationBridgeSettings> options) : IMoodleAcademicSyncClient
{
    private readonly string _apiKey = options.Value.ApiKey;

    public async Task<CursoDto> SynchronizeCourseAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await SynchronizeCourseAndGetSnapshotAsync(courseId, cancellationToken);
        return new CursoDto(snapshot.MoodleCourseId, snapshot.CourseName);
    }

    public async Task<AcademicCourseSnapshot> SynchronizeCourseAndGetSnapshotAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post, $"internal/moodle/courses/{courseId}/synchronize");
        request.Headers.TryAddWithoutValidation("X-Integration-Key", _apiKey);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response);

        var syncResult = await response.Content.ReadFromJsonAsync<SyncCourseResponse>(
            cancellationToken: cancellationToken);
        if (syncResult is null || syncResult.Id != courseId)
        {
            throw new JsonException("Integración Moodle devolvió una respuesta de sincronización inválida.");
        }

        using var snapshotRequest = new HttpRequestMessage(
            HttpMethod.Get, $"internal/moodle/courses/{courseId}/snapshot");
        snapshotRequest.Headers.TryAddWithoutValidation("X-Integration-Key", _apiKey);
        using var snapshotResponse = await httpClient.SendAsync(snapshotRequest, cancellationToken);
        await EnsureSuccessAsync(snapshotResponse);
        return await snapshotResponse.Content.ReadFromJsonAsync<AcademicCourseSnapshot>(
                   cancellationToken: cancellationToken)
               ?? throw new JsonException("Integración Moodle devolvió un snapshot vacío.");
    }

    public async Task<IReadOnlyCollection<SincronizacionDto>> GetSyncHistoryAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, $"internal/moodle/sync-history?limit={limit}");
        request.Headers.TryAddWithoutValidation("X-Integration-Key", _apiKey);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<SincronizacionDto>>(
                   cancellationToken: cancellationToken)
               ?? throw new JsonException("Integración Moodle devolvió un historial vacío o inválido.");
    }

    private static Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return Task.CompletedTask;
        }

        return Task.FromException(new HttpRequestException(
            $"El servicio de Integración Moodle respondió HTTP {(int)response.StatusCode}.",
            null,
            response.StatusCode));
    }

    private sealed record SyncCourseResponse(int Id, string Name, string Status);
}
