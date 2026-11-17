using System.Diagnostics.CodeAnalysis;

namespace Uta.AcademicAnalysis.WebApi.Analysis;

internal static class RecommendationText
{
    public static string Action(string activity) =>
        $"Revisar la estrategia de enseñanza de «{activity}» y considerar "
        + "una actividad de refuerzo con retroalimentación antes de la siguiente evaluación.";

    public static string Justification(
        string activity,
        decimal average,
        int gradeCount,
        decimal threshold,
        DateTime? evidenceAtUtc)
    {
        var evidenceText = evidenceAtUtc.HasValue
            ? $" Evidencia más reciente el {evidenceAtUtc.Value:yyyy-MM-dd} UTC."
            : " Moodle no proporcionó fecha para esta evidencia.";
        return $"Promedio {average:0.00}/10 en «{activity}» con {gradeCount} calificaciones; "
            + $"el umbral acordado es {threshold:0.##}/10.{evidenceText}";
    }

    public static bool IsReviewState([NotNullWhen(true)] string? state) =>
        state?.Trim() is "Aceptada" or "Ajustada" or "Descartada";
}
