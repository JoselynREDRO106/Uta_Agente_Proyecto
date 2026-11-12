using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Moodle;

public sealed class AnalysisHttpClient(
    HttpClient httpClient,
    IOptions<AnalysisBridgeSettings> options) : IAnalysisQueryClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly string _baseUrl = options.Value.BaseUrl.TrimEnd('/');

    public Task<IReadOnlyCollection<CursoDto>> GetCoursesAsync(
        string? bearerToken,
        CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyCollection<CursoDto>>(
            HttpMethod.Get, "/api/analysis/courses", null, bearerToken, cancellationToken);

    public Task<PaginaDto<EstudianteCursoDto>> GetCourseStudentsAsync(
        int courseId,
        string? search,
        string? status,
        int page,
        int pageSize,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"page={page}",
            $"pageSize={pageSize}"
        };
        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query.Add($"status={Uri.EscapeDataString(status.Trim())}");
        }

        return SendAsync<PaginaDto<EstudianteCursoDto>>(
            HttpMethod.Get,
            $"/api/analysis/courses/{courseId}/students?{string.Join("&", query)}",
            null,
            bearerToken,
            cancellationToken);
    }

    public async Task<IndicadoresCursoDto?> GetCourseIndicatorsAsync(
        int courseId,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, _baseUrl + $"/api/analysis/courses/{courseId}/indicators");
        AddBearerToken(request, bearerToken);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<IndicadoresCursoDto>(
                   JsonOptions, cancellationToken)
               ?? throw new JsonException("El servicio de Análisis devolvió indicadores vacíos.");
    }

    public async Task<SeguimientoEstudianteDto?> GetStudentTrackingAsync(
        int courseId,
        int studentId,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            _baseUrl + $"/api/analysis/courses/{courseId}/students/{studentId}/tracking");
        AddBearerToken(request, bearerToken);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<SeguimientoEstudianteDto>(
                   JsonOptions, cancellationToken)
               ?? throw new JsonException("El servicio de Análisis devolvió un seguimiento vacío.");
    }

    public Task<AnalisisGrupalDto> GetGroupAnalysisAsync(
        int courseId,
        decimal threshold,
        int minimumGrades,
        string? bearerToken,
        CancellationToken cancellationToken = default) =>
        SendAsync<AnalisisGrupalDto>(
            HttpMethod.Get,
            $"/api/analysis/courses/{courseId}/group-analysis?threshold={threshold.ToString(System.Globalization.CultureInfo.InvariantCulture)}&minimumGrades={minimumGrades}",
            null,
            bearerToken,
            cancellationToken);

    public Task<IReadOnlyCollection<RecomendacionDto>> GetRecommendationsAsync(
        int courseId,
        string? bearerToken,
        CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyCollection<RecomendacionDto>>(
            HttpMethod.Get,
            $"/api/analysis/courses/{courseId}/recommendations",
            null,
            bearerToken,
            cancellationToken);

    public Task<ResultadoGenerarRecomendacionesDto> GenerateRecommendationsAsync(
        int courseId,
        SolicitudRecomendacionesGatewayDto requestBody,
        string? bearerToken,
        CancellationToken cancellationToken = default) =>
        SendAsync<ResultadoGenerarRecomendacionesDto>(
            HttpMethod.Post,
            $"/api/analysis/courses/{courseId}/recommendations/generate",
            new SolicitudGenerarRecomendacionesDto(
                requestBody.Umbral, requestBody.MinimoCalificaciones),
            bearerToken,
            cancellationToken);

    public async Task<RecomendacionDto?> UpdateRecommendationAsync(
        int courseId,
        Guid recommendationId,
        ActualizarEstadoRecomendacionDto requestBody,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Patch,
            _baseUrl + $"/api/analysis/courses/{courseId}/recommendations/{recommendationId}/status")
        {
            Content = JsonContent.Create(requestBody, options: JsonOptions)
        };
        AddBearerToken(request, bearerToken);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<RecomendacionDto>(
                   JsonOptions, cancellationToken)
               ?? throw new JsonException("El servicio de Análisis devolvió una recomendación vacía.");
    }

    private async Task<T> SendAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        string? bearerToken,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, _baseUrl + path);
        AddBearerToken(request, bearerToken);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response);

        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return result ?? throw new JsonException("El servicio de Análisis devolvió una respuesta vacía.");
    }

    private static void AddBearerToken(HttpRequestMessage request, string? bearerToken)
    {
        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
    }

    private static Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return Task.CompletedTask;
        }

        return Task.FromException(new HttpRequestException(
            $"El servicio de Análisis respondió HTTP {(int)response.StatusCode}.",
            null,
            response.StatusCode));
    }
}
