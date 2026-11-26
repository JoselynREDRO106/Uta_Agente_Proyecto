using System.Globalization;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.AcademicAnalysis.WebApi.Analysis;

public static class GroupAnalysisCalculator
{
    public static AnalisisGrupalDto Calculate(
        AcademicCourseSnapshot snapshot,
        decimal thresholdOutOfTen,
        int minimumGrades)
    {
        var orderedItems = snapshot.Grades.Items
            .Where(item => item.GradeMax > item.GradeMin)
            .Where(item => !IsSummaryItem(item.ItemType))
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .ToArray();
        var items = orderedItems.ToDictionary(item => item.Id);

        var grades = snapshot.Grades.Grades
            .Where(grade => items.ContainsKey(grade.ItemId))
            .GroupBy(grade => new { grade.UserId, grade.ItemId })
            .Select(group => group.First())
            .Select(grade => TryReadGrade(grade, items[grade.ItemId], out var value)
                ? new GradeValue(grade.ItemId, value, TryReadDate(grade.DateGraded, out var date) ? date : null)
                : null)
            .Where(grade => grade is not null)
            .Select(grade => grade!)
            .ToArray();

        var activitySequence = orderedItems
            .Select(item =>
            {
                var itemGrades = grades.Where(grade => grade.ItemId == item.Id).ToArray();
                var count = itemGrades.Length;
                decimal? average = count == 0
                    ? null
                    : decimal.Round(itemGrades.Average(grade => grade.Value), 2);
                var insufficient = count < minimumGrades;
                return new ResultadoActividadDto(
                    item.Id,
                    item.ItemName,
                    average,
                    10m,
                    count,
                    !insufficient && average.HasValue && average.Value < thresholdOutOfTen,
                    insufficient);
            })
            .ToArray();
        var activityResults = activitySequence
            .OrderBy(item => item.Promedio ?? decimal.MaxValue)
            .ThenBy(item => item.Nombre, StringComparer.CurrentCulture)
            .ToArray();

        decimal? previousAverage = null;
        var trend = activitySequence
            .Select(activity =>
            {
                decimal? change = activity.Promedio.HasValue && previousAverage.HasValue
                    ? decimal.Round(activity.Promedio.Value - previousAverage.Value, 2)
                    : null;
                if (activity.Promedio.HasValue)
                {
                    previousAverage = activity.Promedio;
                }

                return new PuntoTendenciaDto(
                    null,
                    activity.Promedio,
                    activity.Escala,
                    activity.NumCalificaciones,
                    activity.DatosInsuficientes,
                    activity.ItemId,
                    activity.Nombre,
                    change);
            })
            .ToArray();

        return new AnalisisGrupalDto(
            snapshot.MoodleCourseId,
            thresholdOutOfTen,
            minimumGrades,
            "Moodle",
            snapshot.UpdatedAtUtc,
            activityResults,
            trend);
    }

    private static bool IsSummaryItem(string itemType) =>
        itemType.Equals("course", StringComparison.OrdinalIgnoreCase)
        || itemType.Equals("category", StringComparison.OrdinalIgnoreCase);

    private static bool TryReadGrade(
        MoodleUserGradeDto grade,
        MoodleGradeItemDto item,
        out decimal normalizedValue)
    {
        if (!decimal.TryParse(
                grade.Grade,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var value))
        {
            normalizedValue = 0;
            return false;
        }

        normalizedValue = (value - item.GradeMin) * 10m / (item.GradeMax - item.GradeMin);
        return true;
    }

    private static bool TryReadDate(string value, out DateTime date)
    {
        date = default;
        if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var timestamp)
            || timestamp <= 0
            || timestamp > 253402300799)
        {
            return false;
        }

        date = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        return true;
    }

    private sealed record GradeValue(int ItemId, decimal Value, DateTime? GradedAtUtc);
}
