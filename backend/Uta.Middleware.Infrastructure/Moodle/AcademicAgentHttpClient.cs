using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Interfaces;

namespace Uta.Middleware.Infrastructure.Moodle;

public sealed class AcademicAgentHttpClient(
    HttpClient httpClient,
    IOptions<AcademicAgentSettings> options) : IAcademicAgentClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly string _apiKey = options.Value.ApiKey;

    public async Task<RespuestaAgenteCursoDto> AskAsync(
        string question,
        IndicadoresCursoDto indicators,
        AnalisisGrupalDto groupAnalysis,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/agent/ask")
        {
            Content = JsonContent.Create(
                new SolicitudAgenteCursoDto(question, indicators, groupAnalysis),
                options: JsonOptions)
        };
        request.Headers.Add("X-Agent-Api-Key", _apiKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"El servicio del agente respondió HTTP {(int)response.StatusCode}.",
                null,
                response.StatusCode);
        }

        var answer = await response.Content.ReadFromJsonAsync<RespuestaAgenteCursoDto>(
            JsonOptions, cancellationToken);
        if (answer is null
            || string.IsNullOrWhiteSpace(answer.Respuesta)
            || answer.Hechos is null
            || answer.Inferencias is null
            || answer.Recomendaciones is null
            || string.IsNullOrWhiteSpace(answer.Incertidumbre))
        {
            throw new JsonException("El servicio del agente devolvió una respuesta incompleta.");
        }

        return answer;
    }
}
