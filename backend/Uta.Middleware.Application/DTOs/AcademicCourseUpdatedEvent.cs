using Uta.Middleware.Application.DTOs.Moodle;

namespace Uta.Middleware.Application.DTOs;

public sealed record AcademicCourseUpdatedEvent(
    Guid EventId,
    string EventType,
    DateTime OccurredAtUtc,
    AcademicCourseSnapshot Snapshot);

public sealed record AcademicCourseSnapshot(
    int MoodleCourseId,
    string CourseName,
    DateTime UpdatedAtUtc,
    DateTime? CourseStartsAtUtc,
    DateTime? CourseEndsAtUtc,
    IReadOnlyCollection<MoodleEnrolledUserDto> EnrolledUsers,
    MoodleGradesResponseDto Grades,
    MoodleCourseModulesResponseDto Contents,
    string SyncStatus,
    string? ErrorSummary);
