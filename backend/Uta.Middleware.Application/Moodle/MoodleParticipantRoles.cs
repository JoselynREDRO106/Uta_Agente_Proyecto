using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.Middleware.Application.Moodle;

public static class MoodleParticipantRoles
{
    public static bool IsTeacher(MoodleEnrolledUserDto user) =>
        user.Roles.Any(role => IsTeacher(role.ShortName));

    public static bool IsStudent(MoodleEnrolledUserDto user) =>
        !IsTeacher(user)
        && user.Roles.Any(role => !IsIgnored(role.ShortName));

    public static bool IsTeacher(string? shortName)
    {
        var role = Normalize(shortName);
        return role is "editingteacher" or "teacher" or "noneditingteacher"
            or "docente" or "profesor";
    }

    private static bool IsIgnored(string? shortName)
    {
        var role = Normalize(shortName);
        return role is "" or "manager" or "gestor" or "coursecreator"
            or "guest" or "invitado" or "user" or "usuario";
    }

    private static string Normalize(string? shortName) =>
        (shortName ?? string.Empty).Trim().ToLowerInvariant();
}
