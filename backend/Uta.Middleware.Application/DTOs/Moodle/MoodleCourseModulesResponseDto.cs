namespace Uta.Middleware.Application.DTOs.Moodle;

/// <summary>
/// DTO que representa la respuesta del endpoint de mapeo temático de Moodle.
/// </summary>
public class MoodleCourseModulesResponseDto
{
    public List<MoodleSectionDto> Sections { get; set; } = [];
}

/// <summary>
/// Sección de un curso en Moodle (equivalente a unidad temática).
/// </summary>
public class MoodleSectionDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Section { get; set; }
    public List<MoodleModuleDto> Modules { get; set; } = [];
}

/// <summary>
/// Módulo/Actividad dentro de una sección de Moodle.
/// </summary>
public class MoodleModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ModName { get; set; } = string.Empty;
    public int? Instance { get; set; }
}
