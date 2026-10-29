using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.Middleware.Application.Interfaces;

/// <summary>
/// Contrato del cliente HTTP que consume los Web Services REST de Moodle.
/// </summary>
public interface IMoodleHttpClient
{
    Task<MoodleCourseDto> GetCourseAsync(
        int courseId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MoodleEnrolledUserDto>> GetEnrolledUsersAsync(
        int courseId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las calificaciones de todos los usuarios de una asignatura
    /// usando la función core_grades_get_grades de Moodle REST.
    /// </summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con ítems y calificaciones del curso.</returns>
    Task<MoodleGradesResponseDto> GetGradesByCourseAsync(
        int courseId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las calificaciones de un usuario específico en una asignatura.
    /// </summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    /// <param name="userIds">Lista de IDs de usuarios.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task<MoodleGradesResponseDto> GetGradesByUserAsync(
        int courseId,
        IEnumerable<int> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la estructura de secciones y módulos de un curso (mapeo temático).
    /// Usa core_course_get_contents para recuperar la organización temática del curso.
    /// </summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task<MoodleCourseModulesResponseDto> GetCourseContentsAsync(
        int courseId,
        CancellationToken cancellationToken = default);
}
