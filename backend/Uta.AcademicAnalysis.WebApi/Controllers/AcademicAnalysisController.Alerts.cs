using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uta.AcademicAnalysis.WebApi.Analysis;
using Uta.AcademicAnalysis.WebApi.Data;
using Uta.Middleware.Application.DTOs;

namespace Uta.AcademicAnalysis.WebApi.Controllers;

public sealed partial class AcademicAnalysisController
{
    private static readonly string[] AlertStates = ["Nueva", "Revisada", "Descartada"];

    [HttpGet("courses/{courseId:int}/alerts/rules")]
    public async Task<ActionResult<IReadOnlyCollection<ReglaAlertaDto>>> GetAlertRules(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        return Ok(ToRuleDtos(await GetAlertRulesAsync(courseId, cancellationToken)));
    }

    [HttpPut("courses/{courseId:int}/alerts/rules")]
    public async Task<ActionResult<IReadOnlyCollection<ReglaAlertaDto>>> SaveAlertRules(
        int courseId,
        GuardarReglasAlertaDto request,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        var changes = request.Reglas ?? [];
        if (changes.Count == 0
            || changes.Select(change => change.Tipo).Distinct().Count() != changes.Count
            || changes.Any(change => !AlertEvaluator.RuleTypes.Contains(change.Tipo)))
        {
            return BadRequest("Indique cada regla (Descenso, Atraso o BajaFinalizacion) una sola vez.");
        }

        foreach (var change in changes)
        {
            var maximum = change.Tipo == AlertEvaluator.Delay ? 365m : 100m;
            if (change.Umbral <= 0 || change.Umbral > maximum)
            {
                return BadRequest(
                    $"El umbral de {AlertEvaluator.RuleName(change.Tipo)} debe ser mayor que 0 y no superar {maximum:0}.");
            }
        }

        if (!await dbContext.Courses.AnyAsync(
                course => course.MoodleCourseId == courseId, cancellationToken))
        {
            return NotFound();
        }

        var actor = GetActor();
        var stored = await dbContext.AlertRules
            .Where(rule => rule.MoodleCourseId == courseId)
            .ToListAsync(cancellationToken);
        foreach (var change in changes)
        {
            var record = stored.SingleOrDefault(rule => rule.Tipo == change.Tipo);
            if (record is null)
            {
                record = new AlertRuleRecord { MoodleCourseId = courseId, Tipo = change.Tipo };
                dbContext.AlertRules.Add(record);
            }

            record.Threshold = change.Umbral;
            record.Active = change.Activa;
            record.UpdatedBy = actor;
            record.UpdatedAtUtc = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToRuleDtos(await GetAlertRulesAsync(courseId, cancellationToken)));
    }

    [HttpGet("courses/{courseId:int}/alerts")]
    public async Task<ActionResult<PanelAlertasDto>> GetAlerts(
        int courseId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        if (status is not null && !AlertStates.Contains(status))
        {
            return BadRequest("El filtro debe ser Nueva, Revisada o Descartada.");
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        var panel = await BuildAlertPanelAsync(snapshot, cancellationToken);
        return Ok(status is null
            ? panel
            : panel with { Alertas = panel.Alertas.Where(alert => alert.Estado == status).ToArray() });
    }

    [HttpPatch("courses/{courseId:int}/alerts/{studentId:int}/{tipo}/state")]
    public async Task<ActionResult<ActualizarEstadoAlertaDto>> UpdateAlertState(
        int courseId,
        int studentId,
        string tipo,
        ActualizarEstadoAlertaDto request,
        CancellationToken cancellationToken)
    {
        if (!CanAccessCourse(courseId))
        {
            return Forbid();
        }

        if (!AlertEvaluator.RuleTypes.Contains(tipo) || !AlertStates.Contains(request.Estado))
        {
            return BadRequest("Indique un tipo de alerta válido y un estado Nueva, Revisada o Descartada.");
        }

        var snapshot = await GetSnapshotAsync(courseId, cancellationToken);
        if (snapshot is null || snapshot.EnrolledUsers.All(user => user.Id != studentId))
        {
            return NotFound();
        }

        var record = await dbContext.AlertStates.SingleOrDefaultAsync(
            item => item.MoodleCourseId == courseId
                && item.StudentId == studentId
                && item.Tipo == tipo,
            cancellationToken);
        if (record is null)
        {
            record = new AlertStateRecord
            {
                MoodleCourseId = courseId,
                StudentId = studentId,
                Tipo = tipo
            };
            dbContext.AlertStates.Add(record);
        }

        record.State = request.Estado;
        record.UpdatedBy = GetActor();
        record.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new ActualizarEstadoAlertaDto(record.State));
    }

    private async Task<List<AlertRuleSetting>> GetAlertRulesAsync(
        int courseId,
        CancellationToken cancellationToken)
    {
        var stored = await dbContext.AlertRules.AsNoTracking()
            .Where(rule => rule.MoodleCourseId == courseId)
            .ToListAsync(cancellationToken);
        return AlertEvaluator.RuleTypes
            .Select(tipo => stored.SingleOrDefault(rule => rule.Tipo == tipo) is { } rule
                ? new AlertRuleSetting(tipo, rule.Threshold, rule.Active)
                : AlertEvaluator.DefaultRule(tipo))
            .ToList();
    }

    private async Task<PanelAlertasDto> BuildAlertPanelAsync(
        AcademicCourseSnapshot snapshot,
        CancellationToken cancellationToken)
    {
        var rules = await GetAlertRulesAsync(snapshot.MoodleCourseId, cancellationToken);
        var detected = AlertEvaluator.Evaluate(snapshot, rules);
        var states = await dbContext.AlertStates.AsNoTracking()
            .Where(item => item.MoodleCourseId == snapshot.MoodleCourseId)
            .ToListAsync(cancellationToken);
        var incomplete = !snapshot.SyncStatus.Equals("Completa", StringComparison.OrdinalIgnoreCase);
        var alerts = detected.Select(alert => new AlertaDto(
            alert.StudentId,
            alert.StudentName,
            alert.Tipo,
            AlertEvaluator.RuleName(alert.Tipo),
            alert.DateUtc,
            states.SingleOrDefault(item => item.StudentId == alert.StudentId && item.Tipo == alert.Tipo)?.State
                ?? "Nueva",
            alert.Text,
            alert.Evidence)).ToArray();
        return new PanelAlertasDto(
            snapshot.MoodleCourseId,
            "Moodle",
            snapshot.UpdatedAtUtc,
            snapshot.SyncStatus,
            incomplete,
            incomplete
                ? "La última sincronización quedó incompleta; no se generan alertas con datos faltantes."
                : null,
            alerts);
    }

    private static ReglaAlertaDto[] ToRuleDtos(IEnumerable<AlertRuleSetting> rules) =>
        rules.Select(rule => new ReglaAlertaDto(
            rule.Tipo,
            AlertEvaluator.RuleName(rule.Tipo),
            rule.Umbral,
            AlertEvaluator.RuleUnit(rule.Tipo),
            rule.Activa)).ToArray();

    private string GetActor()
    {
        var actor = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "docente";
        return actor.Length <= 200 ? actor : actor[..200];
    }
}
