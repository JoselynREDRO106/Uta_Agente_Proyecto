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

        var report = ToReportDto(record, ReadReportContent(record));
        return File(
            AcademicReportPdf.Render(report),
            "application/pdf",
            $"informe-curso-{courseId}-{record.GeneratedAtUtc:yyyyMMdd-HHmm}.pdf");
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

}
