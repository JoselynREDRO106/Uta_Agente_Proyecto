namespace Uta.Middleware.Application.DTOs.Moodle;

public class MoodleCourseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
}

public class MoodleEnrolledUserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<MoodleRoleDto> Roles { get; set; } = [];
}

public class MoodleRoleDto
{
    public string ShortName { get; set; } = string.Empty;
}
