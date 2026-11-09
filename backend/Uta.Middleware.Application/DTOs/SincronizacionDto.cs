namespace Uta.Middleware.Application.DTOs;

public sealed record SincronizacionDto(
    int CourseId,
    string CourseName,
    DateTime CompletedAtUtc,
    string Status,
    string? ErrorSummary);
