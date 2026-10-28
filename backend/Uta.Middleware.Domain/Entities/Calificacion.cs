namespace Uta.Middleware.Domain.Entities;

/// <summary>
/// Representa la calificación de un estudiante en una actividad de Moodle.
/// </summary>
public class Calificacion
{
    public int Id { get; set; }

    /// <summary>ID del usuario en Moodle.</summary>
    public int MoodleUserId { get; set; }

    /// <summary>ID de la asignatura (course) en Moodle.</summary>
    public int MoodleCourseId { get; set; }

    /// <summary>ID del ítem de calificación en Moodle.</summary>
    public int MoodleGradeItemId { get; set; }

    /// <summary>Nombre del ítem de calificación.</summary>
    public string NombreActividad { get; set; } = string.Empty;

    /// <summary>Calificación numérica obtenida.</summary>
    public decimal? NotaObtenida { get; set; }

    /// <summary>Calificación máxima posible.</summary>
    public decimal NotaMaxima { get; set; }

    /// <summary>Fecha en que se registró/actualizó la calificación.</summary>
    public DateTime FechaRegistro { get; set; }
}
