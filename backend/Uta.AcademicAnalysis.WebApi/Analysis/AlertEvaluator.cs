using System.Globalization;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.AcademicAnalysis.WebApi.Analysis;

public sealed record AlertRuleSetting(string Tipo, decimal Umbral, bool Activa);

public sealed record DetectedAlert(
    int StudentId,
    string StudentName,
    string Tipo,
    DateTime DateUtc,
    string Text,
    IReadOnlyCollection<EvidenciaAlertaDto> Evidence);

public static class AlertEvaluator
{
    public const string Descent = "Descenso";
    public const string Delay = "Atraso";
    public const string LowCompletion = "BajaFinalizacion";

    public static readonly IReadOnlyList<string> RuleTypes = [Descent, Delay, LowCompletion];

    public static AlertRuleSetting DefaultRule(string tipo) => tipo switch
    {
        Descent => new(Descent, 20m, true),
        Delay => new(Delay, 3m, true),
        LowCompletion => new(LowCompletion, 50m, true),
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };

    public static string RuleName(string tipo) => tipo switch
    {
        Descent => "Descenso en evaluaciones",
        Delay => "Atraso en entregas",
        LowCompletion => "Baja finalización de actividades",
        _ => tipo
    };

    public static string RuleUnit(string tipo) => tipo == Delay ? "días" : "%";

    public static IReadOnlyCollection<DetectedAlert> Evaluate(
        AcademicCourseSnapshot snapshot,
        IReadOnlyCollection<AlertRuleSetting> rules)
    {
        if (!snapshot.SyncStatus.Equals("Completa", StringComparison.OrdinalIgnoreCase))
        {
            return [];
        }

        var active = rules.Where(rule => rule.Activa).ToDictionary(rule => rule.Tipo);
        if (active.Count == 0)
        {
            return [];
        }

        var items = snapshot.Grades.Items
            .Where(item => item.GradeMax > item.GradeMin
                && !item.ItemType.Equals("course", StringComparison.OrdinalIgnoreCase)
                && !item.ItemType.Equals("category", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(item => item.Id);
        var students = snapshot.EnrolledUsers
            .Where(user => user.Roles.Any(role =>
                role.ShortName.Equals("student", StringComparison.OrdinalIgnoreCase)
                || role.ShortName.Equals("estudiante", StringComparison.OrdinalIgnoreCase)))
            .GroupBy(user => user.Id)
            .Select(group => group.First())
            .ToArray();
        var studentIds = students.Select(user => user.Id).ToHashSet();
        var grades = snapshot.Grades.Grades
            .Where(grade => items.ContainsKey(grade.ItemId) && studentIds.Contains(grade.UserId))
            .GroupBy(grade => (grade.UserId, grade.ItemId))
            .ToDictionary(group => group.Key, group => group.First());

        var referenceDates = new Dictionary<int, DateTime>();
        foreach (var grade in grades.Values)
        {
            var earliest = new[] { ReadDate(grade.DateSubmitted), ReadDate(grade.DateGraded) }
                .Where(date => date.HasValue)
                .Select(date => date!.Value)
                .DefaultIfEmpty()
                .Min();
            if (earliest != default
                && (!referenceDates.TryGetValue(grade.ItemId, out var current) || earliest < current))
            {
                referenceDates[grade.ItemId] = earliest;
            }
        }

        var result = new List<DetectedAlert>();
        foreach (var student in students)
        {
            if (active.TryGetValue(Descent, out var descent))
            {
                AddIfNotNull(result, EvaluateDescent(student, items, grades, descent.Umbral));
            }

            if (active.TryGetValue(Delay, out var delay))
            {
                AddIfNotNull(result, EvaluateDelay(
                    student, items, grades, referenceDates, snapshot.UpdatedAtUtc, delay.Umbral));
            }

            if (active.TryGetValue(LowCompletion, out var completion))
            {
                AddIfNotNull(result, EvaluateCompletion(
                    student, items, grades, referenceDates, snapshot.UpdatedAtUtc, completion.Umbral));
            }
        }

        return result
            .OrderByDescending(alert => alert.DateUtc)
            .ThenBy(alert => alert.StudentName, StringComparer.CurrentCulture)
            .ToArray();
    }

    private static DetectedAlert? EvaluateDescent(
        MoodleEnrolledUserDto student,
        IReadOnlyDictionary<int, MoodleGradeItemDto> items,
        IReadOnlyDictionary<(int, int), MoodleUserGradeDto> grades,
        decimal threshold)
    {
        var graded = items.Values
            .Select(item => grades.TryGetValue((student.Id, item.Id), out var grade)
                ? new { Item = item, Percent = ReadPercent(grade, item), Date = ReadDate(grade.DateGraded) }
                : null)
            .Where(entry => entry is not null && entry.Percent.HasValue && entry.Date.HasValue)
            .Select(entry => entry!)
            .OrderBy(entry => entry.Date)
            .ToArray();
        if (graded.Length < 2)
        {
            return null;
        }

        var last = graded[^1];
        var previous = graded[..^1];
        var baseline = previous.Average(entry => entry.Percent!.Value);
        var drop = baseline - last.Percent!.Value;
        if (drop < threshold)
        {
            return null;
        }

        var evidence = new List<EvidenciaAlertaDto>
        {
            new(last.Item.ItemName, FormatPercent(last.Percent.Value), last.Date,
                "Última evaluación registrada.")
        };
        evidence.AddRange(previous.TakeLast(3).Select(entry => new EvidenciaAlertaDto(
            entry.Item.ItemName,
            FormatPercent(entry.Percent!.Value),
            entry.Date,
            "Evaluación anterior considerada en el promedio de referencia.")));
        return new DetectedAlert(
            student.Id,
            student.FullName,
            Descent,
            last.Date!.Value,
            $"La última evaluación está {drop:0.#} puntos porcentuales por debajo del promedio de las "
            + $"{previous.Length} evaluaciones anteriores ({baseline:0.#}%); el umbral de la regla es {threshold:0.##}. "
            + "Se recomienda revisar el contexto antes de actuar.",
            evidence);
    }

    private static DetectedAlert? EvaluateDelay(
        MoodleEnrolledUserDto student,
        IReadOnlyDictionary<int, MoodleGradeItemDto> items,
        IReadOnlyDictionary<(int, int), MoodleUserGradeDto> grades,
        IReadOnlyDictionary<int, DateTime> referenceDates,
        DateTime evaluatedAt,
        decimal thresholdDays)
    {
        var overdue = items.Values
            .Where(item => referenceDates.TryGetValue(item.Id, out var reference)
                && (evaluatedAt - reference).TotalDays >= (double)thresholdDays
                && !HasSubmission(grades, student.Id, item))
            .Select(item => new { Item = item, Reference = referenceDates[item.Id] })
            .OrderBy(entry => entry.Reference)
            .ToArray();
        if (overdue.Length == 0)
        {
            return null;
        }

        return new DetectedAlert(
            student.Id,
            student.FullName,
            Delay,
            evaluatedAt,
            $"No se registra entrega en {overdue.Length} actividad(es) que otros estudiantes ya entregaron "
            + $"hace {thresholdDays:0.##} días o más. Puede tratarse de una entrega pendiente de registro en Moodle.",
            overdue.Take(5).Select(entry => new EvidenciaAlertaDto(
                entry.Item.ItemName,
                "Sin entrega",
                entry.Reference,
                $"Primera entrega del grupo el {entry.Reference:dd/MM/yyyy}; "
                + $"{(evaluatedAt - entry.Reference).TotalDays:0} días de referencia.")).ToArray());
    }

    private static DetectedAlert? EvaluateCompletion(
        MoodleEnrolledUserDto student,
        IReadOnlyDictionary<int, MoodleGradeItemDto> items,
        IReadOnlyDictionary<(int, int), MoodleUserGradeDto> grades,
        IReadOnlyDictionary<int, DateTime> referenceDates,
        DateTime evaluatedAt,
        decimal thresholdPercent)
    {
        var expected = items.Values.Where(item => referenceDates.ContainsKey(item.Id)).ToArray();
        if (expected.Length == 0)
        {
            return null;
        }

        var completed = expected.Count(item => HasSubmission(grades, student.Id, item));
        var percent = completed * 100m / expected.Length;
        if (percent >= thresholdPercent)
        {
            return null;
        }

        var missing = expected.Where(item => !HasSubmission(grades, student.Id, item)).Take(5).ToArray();
        return new DetectedAlert(
            student.Id,
            student.FullName,
            LowCompletion,
            evaluatedAt,
            $"Se registra {completed} de {expected.Length} actividades con entrega o calificación "
            + $"({percent:0.#}%); el umbral de la regla es {thresholdPercent:0.##}%. "
            + "Los datos reflejan lo sincronizado desde Moodle.",
            missing.Select(item => new EvidenciaAlertaDto(
                item.ItemName,
                "Sin registro",
                referenceDates[item.Id],
                "Actividad con actividad registrada en el grupo y sin registro del estudiante.")).ToArray());
    }

    private static bool HasSubmission(
        IReadOnlyDictionary<(int, int), MoodleUserGradeDto> grades,
        int studentId,
        MoodleGradeItemDto item) =>
        grades.TryGetValue((studentId, item.Id), out var grade)
        && (ReadPercent(grade, item).HasValue || ReadDate(grade.DateSubmitted).HasValue);

    private static decimal? ReadPercent(MoodleUserGradeDto grade, MoodleGradeItemDto item) =>
        decimal.TryParse(grade.Grade, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value / item.GradeMax * 100m
            : null;

    private static DateTime? ReadDate(string value) =>
        long.TryParse(value, out var timestamp) && timestamp > 0
            ? DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime
            : null;

    private static string FormatPercent(decimal value) =>
        value.ToString("0.#", CultureInfo.InvariantCulture) + "%";

    private static void AddIfNotNull(List<DetectedAlert> list, DetectedAlert? alert)
    {
        if (alert is not null)
        {
            list.Add(alert);
        }
    }
}
