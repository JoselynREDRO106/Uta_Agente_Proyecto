namespace Uta.Middleware.Domain.Entities;

/// <summary>
/// Representa el mapeo entre un contenido mínimo de Moodle y su unidad temática.
/// </summary>
public class MapeoTematico
{
    public int Id { get; set; }

    /// <summary>ID de la asignatura (course) en Moodle.</summary>
    public int MoodleCourseId { get; set; }

    /// <summary>Nombre de la asignatura en Moodle.</summary>
    public string NombreAsignatura { get; set; } = string.Empty;

    /// <summary>ID del ítem de calificación en Moodle.</summary>
    public int MoodleGradeItemId { get; set; }

    /// <summary>Nombre del ítem/actividad en Moodle.</summary>
    public string NombreActividad { get; set; } = string.Empty;

    /// <summary>Nombre de la unidad temática a la que pertenece.</summary>
    public string UnidadTematica { get; set; } = string.Empty;

    /// <summary>Nombre del contenido mínimo asociado.</summary>
    public string ContenidoMinimo { get; set; } = string.Empty;

    /// <summary>Peso ponderado de este ítem en la calificación final.</summary>
    public decimal Ponderacion { get; set; }
}
