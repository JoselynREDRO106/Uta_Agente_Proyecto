using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Moodle;

public sealed class AnalysisReportHttpClient(
    HttpClient httpClient,
    IOptions<AnalysisBridgeSettings> options) : IAnalysisReportClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly string _baseUrl = options.Value.BaseUrl.TrimEnd('/');

    public async Task<InformeDto> GenerateReportAsync(
        int courseId,
        GenerarInformeDto requestBody,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, $"/api/analysis/courses/{courseId}/reports", bearerToken);
        request.Content = JsonContent.Create(requestBody, options: JsonOptions);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<InformeDto>(JsonOptions, cancellationToken)
               ?? throw new JsonException("El servicio de Análisis devolvió un informe vacío.");
    }

    public async Task<IReadOnlyCollection<InformeResumenDto>> GetReportsAsync(
        int courseId,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, $"/api/analysis/courses/{courseId}/reports", bearerToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<InformeResumenDto>>(
                   JsonOptions, cancellationToken)
               ?? throw new JsonException("El servicio de Análisis devolvió una lista de informes vacía.");
    }

    public async Task<InformeDto?> GetReportAsync(
        int courseId,
        Guid reportId,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(
            HttpMethod.Get, $"/api/analysis/courses/{courseId}/reports/{reportId}", bearerToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<InformeDto>(JsonOptions, cancellationToken)
               ?? throw new JsonException("El servicio de Análisis devolvió un informe vacío.");
    }

    public async Task<ArchivoInforme?> DownloadReportAsync(
        int courseId,
        Guid reportId,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(
            HttpMethod.Get, $"/api/analysis/courses/{courseId}/reports/{reportId}/download", bearerToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        EnsureSuccess(response);
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                       ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                       ?? $"informe-{reportId}.pdf";
        return new ArchivoInforme(
            await response.Content.ReadAsByteArrayAsync(cancellationToken),
            response.Content.Headers.ContentType?.ToString() ?? "application/pdf",
            fileName);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string path, string? bearerToken)
    {
        var request = new HttpRequestMessage(method, _baseUrl + path);
        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        return request;
    }

    private static void EnsureSuccess(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"El servicio de Análisis respondió HTTP {(int)response.StatusCode}.",
                null,
                response.StatusCode);
        }
    }
}
