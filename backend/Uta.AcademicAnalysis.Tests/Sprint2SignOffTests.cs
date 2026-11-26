extern alias middleware;

using System.Globalization;
using Xunit;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Uta.AcademicAnalysis.WebApi.Analysis;
using Uta.AcademicAnalysis.WebApi.Controllers;
using Uta.Middleware.Application.Auditing;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.Interfaces;

namespace Uta.AcademicAnalysis.Tests;

public sealed class GroupAnalysisSignOffTests
{
    private static readonly DateTime SyncedAt = new(2026, 11, 12, 15, 30, 0, DateTimeKind.Utc);

    [Fact]
    public void Cp10_orders_by_average_marks_threshold_and_keeps_moodle_source()
    {
        var analysis = GroupAnalysisCalculator.Calculate(SampleSnapshot(), thresholdOutOfTen: 6m, minimumGrades: 2);

        Assert.Equal(["Practica C", "Tarea B", "Quiz A"], analysis.Actividades.Select(item => item.Nombre).ToArray());
        Assert.Equal(3.00m, analysis.Actividades.Single(item => item.Nombre == "Tarea B").Promedio);
        Assert.True(analysis.Actividades.Single(item => item.Nombre == "Tarea B").BajoUmbral);
        Assert.False(analysis.Actividades.Single(item => item.Nombre == "Quiz A").BajoUmbral);
        Assert.Equal("Moodle", analysis.Fuente);
        Assert.Equal(SyncedAt, analysis.UltimaSincronizacionUtc);

        Assert.Equal(["Quiz A", "Tarea B", "Practica C"], analysis.Tendencia.Select(item => item.Actividad).ToArray());
        var quiz = analysis.Tendencia.Single(item => item.Actividad == "Quiz A");
        Assert.Null(quiz.Fecha);
        Assert.Null(quiz.Variacion);
        Assert.Equal(10m, quiz.Escala);
        Assert.Equal(6.00m, quiz.PromedioCurso);
        Assert.False(quiz.DatosInsuficientes);
        Assert.Equal(-3.00m, analysis.Tendencia.Single(item => item.Actividad == "Tarea B").Variacion);
        Assert.True(analysis.Tendencia.Single(item => item.Actividad == "Practica C").DatosInsuficientes);
    }

    [Fact]
    public void Cp10_raises_the_minimum_and_stops_marking_a_small_sample_as_low()
    {
        var analysis = GroupAnalysisCalculator.Calculate(SampleSnapshot(), thresholdOutOfTen: 6m, minimumGrades: 3);
        var small = analysis.Actividades.Single(item => item.Nombre == "Tarea B");

        Assert.True(small.DatosInsuficientes);
        Assert.False(small.BajoUmbral);
        Assert.False(analysis.Tendencia.Single(item => item.Actividad == "Quiz A").DatosInsuficientes);
        Assert.True(analysis.Tendencia.Single(item => item.Actividad == "Tarea B").DatosInsuficientes);
    }

    [Fact]
    public void Cp12_names_the_activity_and_keeps_the_evidence_date()
    {
        var evidence = new DateTime(2026, 11, 10, 0, 0, 0, DateTimeKind.Utc);
        var action = RecommendationText.Action("Tarea B");
        var justification = RecommendationText.Justification("Tarea B", 3.00m, 2, 6m, evidence);
        var withoutDate = RecommendationText.Justification("Tarea B", 3.00m, 2, 6m, null);

        Assert.Contains("Tarea B", action, StringComparison.Ordinal);
        Assert.Contains("Revisar", action, StringComparison.Ordinal);
        Assert.Contains("Promedio 3.00/10", justification, StringComparison.Ordinal);
        Assert.Contains("«Tarea B»", justification, StringComparison.Ordinal);
        Assert.Contains("2026-11-10", justification, StringComparison.Ordinal);
        Assert.Contains("Moodle no proporcionó fecha", withoutDate, StringComparison.Ordinal);
    }

    [Fact]
    public void Cp12_accepts_only_the_three_review_states()
    {
        Assert.True(RecommendationText.IsReviewState("Aceptada"));
        Assert.True(RecommendationText.IsReviewState(" Ajustada "));
        Assert.True(RecommendationText.IsReviewState("Descartada"));
        Assert.False(RecommendationText.IsReviewState("Propuesta"));
        Assert.False(RecommendationText.IsReviewState("Rechazada"));
    }

    [Fact]
    public void Cp12_explains_insufficient_data_without_creating_recommendations()
    {
        var analysis = new AnalisisGrupalDto(
            1001, 6m, 7, "Moodle", SyncedAt,
            [new ResultadoActividadDto(1, "Tarea B", 3m, 10m, 2, false, true)],
            []);

        var reason = AcademicAnalysisController.ExplainGroupGap(analysis, created: 0, alreadyPresent: 0, insufficient: 1, belowWithSample: 0);

        Assert.NotNull(reason);
        Assert.Contains("datos insuficientes", reason, StringComparison.Ordinal);
        Assert.Contains("no se concluye", reason, StringComparison.Ordinal);
    }

    private static AcademicCourseSnapshot SampleSnapshot()
    {
        var graded = DateTimeOffset.Parse("2026-11-10T12:00:00Z", CultureInfo.InvariantCulture).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        return new AcademicCourseSnapshot(
            1001,
            "Curso de prueba",
            SyncedAt,
            null,
            null,
            [],
            new MoodleGradesResponseDto
            {
                Items =
                [
                    Item(10, "Quiz A", "mod"),
                    Item(20, "Tarea B", "mod"),
                    Item(30, "Practica C", "mod"),
                    Item(40, "Total", "category")
                ],
                Grades =
                [
                    Grade(1, 10, "8", graded),
                    Grade(2, 10, "6", graded),
                    Grade(3, 10, "4", graded),
                    Grade(1, 20, "2", graded),
                    Grade(2, 20, "4", graded),
                    Grade(1, 30, "1", graded),
                    Grade(1, 40, "9", graded)
                ]
            },
            new MoodleCourseModulesResponseDto(),
            "Completa",
            null);
    }

    private static MoodleGradeItemDto Item(int id, string name, string type) => new()
    {
        Id = id,
        ItemName = name,
        ItemType = type,
        GradeMin = 0,
        GradeMax = 10
    };

    private static MoodleUserGradeDto Grade(int userId, int itemId, string value, string gradedAt) => new()
    {
        UserId = userId,
        ItemId = itemId,
        Grade = value,
        DateGraded = gradedAt
    };
}

public sealed class MiddlewareSignOffTests : IClassFixture<MiddlewareSignOffFactory>
{
    private const string SecretQuestion = "clave-secreta-no-guardar";
    private readonly MiddlewareSignOffFactory _factory;

    public MiddlewareSignOffTests(MiddlewareSignOffFactory factory) => _factory = factory;

    [Fact]
    public async Task Cp11_and_cp14_record_the_agent_query_without_the_question()
    {
        _factory.Audit.Events.Clear();
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token("Administrador técnico", "admin.prueba"));

        var response = await client.PostAsJsonAsync("/api/cursos/1001/consultar", new { pregunta = SecretQuestion });
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.TryGetProperty("hechos", out var facts) && facts.GetArrayLength() == 1);
        Assert.True(json.RootElement.TryGetProperty("inferencias", out _));
        Assert.True(json.RootElement.TryGetProperty("recomendaciones", out _));
        Assert.Equal("La muestra no alcanza para concluir.", json.RootElement.GetProperty("incertidumbre").GetString());

        Assert.Equal(
            ["UsoHerramienta", "UsoHerramienta", "ConsultaAgente"],
            _factory.Audit.Events.Select(item => item.EventType).ToArray());
        Assert.All(_factory.Audit.Events, item =>
        {
            Assert.Equal("admin.prueba", item.UserName);
            Assert.Equal("Exito", item.Result);
            Assert.DoesNotContain(SecretQuestion, item.Detail, StringComparison.Ordinal);
            Assert.DoesNotContain("token", item.Detail, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("contraseña", item.Detail, StringComparison.OrdinalIgnoreCase);
        });
        Assert.Contains("obtener_indicadores_curso", _factory.Audit.Events[0].Detail, StringComparison.Ordinal);
        Assert.Contains("obtener_analisis_grupal", _factory.Audit.Events[1].Detail, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Cp11_denies_a_teacher_without_the_course()
    {
        _factory.Audit.Events.Clear();
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Token("Docente", "docente.prueba", managedCourses: true, courseId: 2));

        var response = await client.PostAsJsonAsync("/api/cursos/1001/consultar", new { pregunta = SecretQuestion });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var denied = Assert.Single(_factory.Audit.Events);
        Assert.Equal("ConsultaAgente", denied.EventType);
        Assert.Equal("Denegado", denied.Result);
        Assert.Equal("docente.prueba", denied.UserName);
        Assert.DoesNotContain(SecretQuestion, denied.Detail, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Cp14_filters_tool_use_and_rejects_a_teacher()
    {
        Assert.Contains("UsoHerramienta", AuditEventTypes.Allowed);
        _factory.Audit.Events.Clear();
        _factory.Audit.SearchCalls = 0;
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token("Docente", "docente.prueba"));
        var denied = await client.GetAsync("/api/auditoria");
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
        Assert.Equal(0, _factory.Audit.SearchCalls);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token("Administrador técnico", "admin.prueba"));
        var page = await client.GetAsync("/api/auditoria?tipo=UsoHerramienta&desdeUtc=2026-11-01T00:00:00Z&hastaUtc=2026-11-17T23:59:59Z&pagina=1&tamanoPagina=20");

        Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        Assert.Equal(1, _factory.Audit.SearchCalls);
        Assert.Equal("UsoHerramienta", _factory.Audit.LastFilter!.Tipo);
        Assert.Equal(20, _factory.Audit.LastFilter.TamanoPagina);
        Assert.True(_factory.Audit.LastFilter.DesdeUtc < _factory.Audit.LastFilter.HastaUtc);
    }

    private static string Token(string role, string name, bool managedCourses = false, int? courseId = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "9"),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Role, role)
        };
        if (managedCourses)
        {
            claims.Add(new Claim("course_assignments", "managed"));
        }

        if (courseId.HasValue)
        {
            claims.Add(new Claim("course_id", courseId.Value.ToString(CultureInfo.InvariantCulture)));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(MiddlewareSignOffFactory.SigningKey));
        var jwt = new JwtSecurityToken(
            issuer: "EduMonitor",
            audience: "EduMonitor",
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public sealed class MiddlewareSignOffFactory : WebApplicationFactory<middleware::Program>
{
    public const string SigningKey = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    public CapturingAudit Audit { get; } = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseSetting("Authentication:Jwt:SigningKey", SigningKey);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IAuditLogRepository>();
            services.AddSingleton<IAuditLogRepository>(Audit);
            services.RemoveAll<IAnalysisQueryClient>();
            services.AddSingleton<IAnalysisQueryClient, StubAnalysis>();
            services.RemoveAll<IAcademicAgentClient>();
            services.AddSingleton<IAcademicAgentClient, StubAgent>();
        });
    }
}

public sealed class CapturingAudit : IAuditLogRepository
{
    public List<AuditCall> Events { get; } = [];
    public FiltroAuditoriaDto? LastFilter { get; private set; }
    public int SearchCalls { get; set; }

    public Task RecordAsync(
        string? userId,
        string userName,
        string role,
        string eventType,
        int? courseId,
        string result,
        string detail,
        CancellationToken cancellationToken = default)
    {
        Events.Add(new AuditCall(userId, userName, role, eventType, courseId, result, detail, DateTime.UtcNow));
        return Task.CompletedTask;
    }

    public Task<PaginaDto<RegistroAuditoriaDto>> SearchAsync(
        FiltroAuditoriaDto filter,
        CancellationToken cancellationToken = default)
    {
        SearchCalls++;
        LastFilter = filter;
        var items = Events
            .Select((item, index) => new RegistroAuditoriaDto(
                index + 1, item.At, item.UserName, item.Role, item.EventType, item.CourseId, item.Result, item.Detail))
            .ToArray();
        return Task.FromResult(new PaginaDto<RegistroAuditoriaDto>(items, filter.Pagina, filter.TamanoPagina, items.Length, 1));
    }
}

public sealed record AuditCall(
    string? UserId,
    string UserName,
    string Role,
    string EventType,
    int? CourseId,
    string Result,
    string Detail,
    DateTime At);

public sealed class StubAnalysis : IAnalysisQueryClient
{
    public Task<IndicadoresCursoDto?> GetCourseIndicatorsAsync(int courseId, string? bearerToken, CancellationToken cancellationToken = default) =>
        Task.FromResult<IndicadoresCursoDto?>(new IndicadoresCursoDto(
            courseId, 3, 6.5m, 8m, 2, 3, 40m, new DateTime(2026, 11, 12, 15, 30, 0, DateTimeKind.Utc),
            "Completa", true, []));

    public Task<AnalisisGrupalDto> GetGroupAnalysisAsync(int courseId, decimal threshold, int minimumGrades, string? bearerToken, CancellationToken cancellationToken = default) =>
        Task.FromResult(new AnalisisGrupalDto(courseId, threshold, minimumGrades, "Moodle", DateTime.UtcNow, [], []));

    public Task<IReadOnlyCollection<CursoDto>> GetCoursesAsync(string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<PaginaDto<EstudianteCursoDto>> GetCourseStudentsAsync(int courseId, string? search, string? status, int page, int pageSize, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<IReadOnlyCollection<DocenteCursoDto>> GetCourseTeachersAsync(int courseId, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<LibroCalificacionesDto?> GetCourseGradebookAsync(int courseId, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<SeguimientoEstudianteDto?> GetStudentTrackingAsync(int courseId, int studentId, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<IReadOnlyCollection<RecomendacionDto>> GetRecommendationsAsync(int courseId, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<ResultadoGenerarRecomendacionesDto> GenerateRecommendationsAsync(int courseId, SolicitudRecomendacionesGatewayDto request, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<RecomendacionDto?> UpdateRecommendationAsync(int courseId, Guid recommendationId, ActualizarEstadoRecomendacionDto request, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<IReadOnlyCollection<ReglaAlertaDto>> GetAlertRulesAsync(int courseId, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<IReadOnlyCollection<ReglaAlertaDto>> SaveAlertRulesAsync(int courseId, GuardarReglasAlertaDto request, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<PanelAlertasDto?> GetAlertsAsync(int courseId, string? status, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<ActualizarEstadoAlertaDto?> UpdateAlertStateAsync(int courseId, int studentId, string alertType, ActualizarEstadoAlertaDto request, string? bearerToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
}

public sealed class StubAgent : IAcademicAgentClient
{
    public Task<RespuestaAgenteCursoDto> AskAsync(
        string question,
        IndicadoresCursoDto indicators,
        AnalisisGrupalDto groupAnalysis,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(new RespuestaAgenteCursoDto(
            "Hay una actividad por revisar.",
            ["Tarea B está bajo el umbral."],
            ["Conviene reforzar antes de la siguiente evaluación."],
            ["Revisar Tarea B."],
            "La muestra no alcanza para concluir.",
            ["obtener_analisis_grupal"]));
}
