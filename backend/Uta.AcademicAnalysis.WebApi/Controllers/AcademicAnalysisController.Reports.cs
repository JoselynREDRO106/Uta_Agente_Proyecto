using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uta.AcademicAnalysis.WebApi.Analysis;
using Uta.AcademicAnalysis.WebApi.Data;
using Uta.Middleware.Application.DTOs;

namespace Uta.AcademicAnalysis.WebApi.Controllers;

public sealed partial class AcademicAnalysisController
{
    private const decimal ReportThreshold = 6m;
    private const int ReportMinimumGrades = 7;

    [HttpPost("courses/{courseId:int}/reports")]
    public async Task<ActionResult<InformeDto>> GenerateReport(
        int courseId,
        GenerarInformeDto request,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        var period = await GetActivePeriodAsync(snapshot, cancellationToken);
        var periodName = string.IsNullOrWhiteSpace(request.Periodo)
            ? period.Name ?? "Período no definido"
            : request.Periodo.Trim();
        if (periodName.Length > 150)
        {
            return BadRequest("El período no puede superar 150 caracteres.");
        }

        var indicators = BuildIndicators(snapshot, period);
        var group = GroupAnalysisCalculator.Calculate(snapshot, ReportThreshold, ReportMinimumGrades);
        var below = group.Actividades.Where(activity => activity.BajoUmbral).ToArray();
        var recommendations = await dbContext.Recommendations.AsNoTracking()
            .Where(item => item.MoodleCourseId == courseId && item.State != "Descartada")
            .OrderByDescending(item => item.CreatedAtUtc)
            .Select(item => item.RecommendationText)
            .Take(20)
            .ToListAsync(cancellationToken);

        var incomplete = !string.Equals(snapshot.SyncStatus, "Completa", StringComparison.OrdinalIgnoreCase);
        var findings = new List<string>();
        if (indicators.Promedio is { } average)
        {
            findings.Add($"El promedio del curso es {average:0.00} sobre {indicators.NotaMaxima:0.##}.");
        }

        findings.Add(below.Length == 0
            ? "No se identificaron actividades por debajo del umbral con datos suficientes."
            : $"{below.Length} actividad(es) con promedio inferior a {ReportThreshold:0.##}.");
        findings.Add($"{indicators.ActividadesEvaluadas} de {indicators.ActividadesTotales} actividades tienen calificaciones.");

        var content = new ContenidoInformeDto(
            snapshot.CourseName,
            periodName,
            indicators,
            below,
            findings,
            recommendations,
            incomplete
                ? "Datos incompletos: la última sincronización no terminó correctamente; las métricas pueden no reflejar todo el curso."
                : null);

        var actor = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "docente";
        var record = new AcademicReportRecord
        {
            Id = Guid.NewGuid(),
            MoodleCourseId = courseId,
            Period = periodName,
            GeneratedAtUtc = DateTime.UtcNow,
            DataAtUtc = snapshot.UpdatedAtUtc,
            IncompleteData = incomplete,
            ContentJson = JsonSerializer.Serialize(content, SnapshotJsonOptions),
            CreatedBy = actor.Length <= 200 ? actor : actor[..200]
        };
        dbContext.Reports.Add(record);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToReportDto(record, content));
    }

    [HttpGet("courses/{courseId:int}/reports")]
    public async Task<ActionResult<IReadOnlyCollection<InformeResumenDto>>> GetReports(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var records = await dbContext.Reports.AsNoTracking()
            .Where(item => item.MoodleCourseId == courseId)
            .OrderByDescending(item => item.GeneratedAtUtc)
            .Select(item => new InformeResumenDto(
                item.Id, item.MoodleCourseId, item.Period, item.GeneratedAtUtc, item.DataAtUtc, item.IncompleteData))
            .ToListAsync(cancellationToken);
        return Ok(records);
    }

    [HttpGet("courses/{courseId:int}/reports/{reportId:guid}")]
    public async Task<ActionResult<InformeDto>> GetReport(
        int courseId,
        Guid reportId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var record = await FindReportAsync(courseId, reportId, cancellationToken);
        return record is null ? NotFound() : Ok(ToReportDto(record, ReadReportContent(record)));
    }

    [HttpGet("courses/{courseId:int}/reports/{reportId:guid}/download")]
    public async Task<IActionResult> DownloadReport(
        int courseId,
        Guid reportId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var record = await FindReportAsync(courseId, reportId, cancellationToken);
        if (record is null)
        {
            return NotFound();
        }

        var html = RenderReportHtml(record, ReadReportContent(record));
        return File(
            System.Text.Encoding.UTF8.GetBytes(html),
            "text/html; charset=utf-8",
            $"informe-curso-{courseId}-{record.GeneratedAtUtc:yyyyMMdd-HHmm}.html");
    }

    private Task<AcademicReportRecord?> FindReportAsync(
        int courseId,
        Guid reportId,
        CancellationToken cancellationToken) =>
        dbContext.Reports.AsNoTracking().SingleOrDefaultAsync(
            item => item.Id == reportId && item.MoodleCourseId == courseId,
            cancellationToken);

    private static ContenidoInformeDto ReadReportContent(AcademicReportRecord record) =>
        JsonSerializer.Deserialize<ContenidoInformeDto>(record.ContentJson, SnapshotJsonOptions)
        ?? throw new JsonException($"El informe {record.Id} es inválido.");

    private static InformeDto ToReportDto(AcademicReportRecord record, ContenidoInformeDto content) =>
        new(record.Id, record.MoodleCourseId, record.Period, record.GeneratedAtUtc,
            record.DataAtUtc, record.IncompleteData, content);

    private static string RenderReportHtml(AcademicReportRecord record, ContenidoInformeDto content)
    {
        static string E(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
        static string Local(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, EcuadorTimeZone).ToString("yyyy-MM-dd HH:mm");
        static string List(IEnumerable<string> items) =>
            "<ul>" + string.Join(string.Empty, items.Select(item => $"<li>{E(item)}</li>")) + "</ul>";

        var indicators = content.Indicadores;
        var builder = new System.Text.StringBuilder();
        builder.Append("<!DOCTYPE html><html lang=\"es\"><head><meta charset=\"utf-8\">");
        builder.Append($"<title>Informe - {E(content.Curso)}</title>");
        builder.Append("<style>body{font-family:Arial,sans-serif;max-width:800px;margin:2rem auto;color:#1f2937}");
        builder.Append("table{border-collapse:collapse;width:100%}td,th{border:1px solid #cbd5e1;padding:6px;text-align:left}");
        builder.Append(".warn{background:#fef3c7;border:1px solid #f59e0b;padding:8px}</style></head><body>");
        builder.Append($"<h1>Informe académico: {E(content.Curso)}</h1>");
        builder.Append($"<p><strong>Período:</strong> {E(content.Periodo)}<br>");
        builder.Append($"<strong>Generado:</strong> {Local(record.GeneratedAtUtc)}<br>");
        builder.Append($"<strong>Datos actualizados al:</strong> {Local(record.DataAtUtc)} · Fuente: Moodle</p>");
        if (content.Aviso is not null)
        {
            builder.Append($"<p class=\"warn\">{E(content.Aviso)}</p>");
        }

        builder.Append("<h2>Métricas</h2><table>");
        builder.Append($"<tr><th>Estudiantes</th><td>{indicators.Estudiantes}</td></tr>");
        builder.Append($"<tr><th>Promedio</th><td>{(indicators.Promedio is { } p ? p.ToString("0.00") : "Sin datos")}</td></tr>");
        builder.Append($"<tr><th>Actividades evaluadas</th><td>{indicators.ActividadesEvaluadas} de {indicators.ActividadesTotales}</td></tr></table>");
        builder.Append("<h2>Hallazgos</h2>").Append(List(content.Hallazgos));
        builder.Append("<h2>Actividades bajo el umbral</h2>");
        builder.Append(content.ActividadesBajoUmbral.Count == 0
            ? "<p>Ninguna.</p>"
            : "<table><tr><th>Actividad</th><th>Promedio</th><th>Calificaciones</th></tr>"
              + string.Join(string.Empty, content.ActividadesBajoUmbral.Select(a =>
                  $"<tr><td>{E(a.Nombre)}</td><td>{a.Promedio:0.00}</td><td>{a.NumCalificaciones}</td></tr>"))
              + "</table>");
        builder.Append("<h2>Recomendaciones</h2>");
        builder.Append(content.Recomendaciones.Count == 0 ? "<p>Sin recomendaciones registradas.</p>" : List(content.Recomendaciones));
        builder.Append("</body></html>");
        return builder.ToString();
    }
}
