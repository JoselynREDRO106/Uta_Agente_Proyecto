namespace Uta.Middleware.Application.DTOs.Moodle;

/// <summary>
/// DTO que representa la respuesta del endpoint core_grades_get_grades de Moodle.
/// </summary>
public class MoodleGradesResponseDto
{
    public List<MoodleGradeItemDto> Items { get; set; } = [];
    public List<MoodleUserGradeDto> Grades { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}

/// <summary>
/// Ítem de calificación devuelto por Moodle (columna del libro de calificaciones).
/// </summary>
public class MoodleGradeItemDto
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string ItemModule { get; set; } = string.Empty;
    public int? ItemInstance { get; set; }
    public decimal GradeMin { get; set; }
    public decimal GradeMax { get; set; }
}

/// <summary>
/// Calificación individual de un usuario en un ítem de Moodle.
/// </summary>
public class MoodleUserGradeDto
{
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string DateSubmitted { get; set; } = string.Empty;
    public string DateGraded { get; set; } = string.Empty;
}
