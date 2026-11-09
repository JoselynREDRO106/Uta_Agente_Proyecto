using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.Interfaces;
using Uta.Middleware.Infrastructure.Moodle;
using Uta.MoodleIntegration.WebApi.Data;
using Uta.MoodleIntegration.WebApi.Services;

namespace Uta.MoodleIntegration.WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("internal/moodle")]
public sealed class InternalMoodleController(
    MoodleConfigurationService configuration,
    IHttpClientFactory httpClientFactory,
    MoodleIntegrationDbContext dbContext) : ControllerBase
{
    private static readonly JsonSerializerOptions EventJsonOptions = new(JsonSerializerDefaults.Web);

    [HttpPost("courses/{courseId:int}/synchronize")]
    public async Task<ActionResult<CursoResponse>> SynchronizeCourse(
        int courseId,
        CancellationToken cancellationToken)
    {
        if (courseId <= 0)
        {
            return BadRequest("El courseId debe ser un entero positivo.");
        }

        try
        {
            var client = await CreateMoodleClientAsync(cancellationToken);
            var course = await client.GetCourseAsync(courseId, cancellationToken);
            var enrolledUsers = (await client.GetEnrolledUsersAsync(courseId, cancellationToken)).ToArray();
            var studentIds = enrolledUsers
                .Where(user => user.Roles.Any(role =>
                    role.ShortName.Equals("student", StringComparison.OrdinalIgnoreCase)
                    || role.ShortName.Equals("estudiante", StringComparison.OrdinalIgnoreCase)))
                .Select(user => user.Id);
            var grades = await client.GetGradesByCourseAsync(courseId, studentIds, cancellationToken);
            var contents = await client.GetCourseContentsAsync(courseId, cancellationToken);
            var now = DateTime.UtcNow;
            var snapshot = new AcademicCourseSnapshot(
                course.Id,
                string.IsNullOrWhiteSpace(course.FullName) ? course.ShortName : course.FullName,
                now,
                ToUtcDate(course.StartDate),
                ToUtcDate(course.EndDate),
                enrolledUsers,
                grades,
                contents,
                grades.Warnings.Count == 0 ? "Completa" : "Incompleta",
                grades.Warnings.Count == 0 ? null : string.Join("; ", grades.Warnings));
            var @event = new AcademicCourseUpdatedEvent(
                Guid.NewGuid(),
                "academic.course.updated.v1",
                now,
                snapshot);

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var courseRecord = await dbContext.AcademicCourses
                .SingleOrDefaultAsync(item => item.MoodleCourseId == courseId, cancellationToken);
            if (courseRecord is null)
            {
                courseRecord = new AcademicCourseRecord { MoodleCourseId = courseId };
                dbContext.AcademicCourses.Add(courseRecord);
            }

            courseRecord.CourseName = snapshot.CourseName;
            courseRecord.SnapshotJson = JsonSerializer.Serialize(snapshot, EventJsonOptions);
            courseRecord.UpdatedAtUtc = now;
            dbContext.AcademicSyncs.Add(new AcademicSyncRecord
            {
                MoodleCourseId = courseId,
                CourseName = snapshot.CourseName,
                CompletedAtUtc = now,
                Status = snapshot.SyncStatus,
                ErrorSummary = Limit(snapshot.ErrorSummary, 1000)
            });
            dbContext.Outbox.Add(new IntegrationOutboxRecord
            {
                Id = @event.EventId,
                EventType = @event.EventType,
                PayloadJson = JsonSerializer.Serialize(@event, EventJsonOptions),
                CreatedAtUtc = now,
                NextAttemptAtUtc = now
            });
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Ok(new CursoResponse(courseId, snapshot.CourseName, snapshot.SyncStatus));
        }
        catch (HttpRequestException exception)
        {
            await RecordFailedSyncAsync(courseId, exception.Message, cancellationToken);
            return Problem(
                title: "No se pudo completar la sincronización con Moodle.",
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (JsonException exception)
        {
            await RecordFailedSyncAsync(courseId, exception.Message, cancellationToken);
            return Problem(
                title: "Moodle devolvió datos con formato inválido.",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }

    [HttpGet("sync-history")]
    public async Task<ActionResult<IReadOnlyCollection<SincronizacionDto>>> GetSyncHistory(
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        if (limit is < 1 or > 100)
        {
            return BadRequest("El límite debe estar entre 1 y 100.");
        }

        var records = await dbContext.AcademicSyncs.AsNoTracking()
            .OrderByDescending(item => item.CompletedAtUtc)
            .ThenByDescending(item => item.Id)
            .Take(limit)
            .Select(item => new SincronizacionDto(
                item.MoodleCourseId,
                item.CourseName,
                item.CompletedAtUtc,
                item.Status,
                item.ErrorSummary))
            .ToListAsync(cancellationToken);
        return Ok(records);
    }

    [HttpGet("courses/{courseId:int}/snapshot")]
    public async Task<ActionResult<AcademicCourseSnapshot>> GetAcademicSnapshot(
        int courseId,
        CancellationToken cancellationToken)
    {
        var snapshotJson = await dbContext.AcademicCourses.AsNoTracking()
            .Where(item => item.MoodleCourseId == courseId)
            .Select(item => item.SnapshotJson)
            .SingleOrDefaultAsync(cancellationToken);
        if (snapshotJson is null)
        {
            return NotFound();
        }

        var snapshot = JsonSerializer.Deserialize<AcademicCourseSnapshot>(
            snapshotJson, EventJsonOptions);
        return snapshot is null
            ? throw new JsonException($"El snapshot del curso {courseId} está vacío.")
            : Ok(snapshot);
    }

    [HttpGet("courses/{courseId:int}")]
    public async Task<ActionResult<MoodleCourseDto>> GetCourse(
        int courseId,
        CancellationToken cancellationToken)
    {
        var client = await CreateMoodleClientAsync(cancellationToken);
        return Ok(await client.GetCourseAsync(courseId, cancellationToken));
    }

    [HttpGet("courses/{courseId:int}/enrolled-users")]
    public async Task<ActionResult<IReadOnlyCollection<MoodleEnrolledUserDto>>> GetEnrolledUsers(
        int courseId,
        CancellationToken cancellationToken)
    {
        var client = await CreateMoodleClientAsync(cancellationToken);
        return Ok(await client.GetEnrolledUsersAsync(courseId, cancellationToken));
    }

    [HttpPost("courses/{courseId:int}/grades")]
    public async Task<ActionResult<MoodleGradesResponseDto>> GetGrades(
        int courseId,
        GradeRequest request,
        CancellationToken cancellationToken)
    {
        var client = await CreateMoodleClientAsync(cancellationToken);
        return Ok(await client.GetGradesByCourseAsync(courseId, request.UserIds, cancellationToken));
    }

    [HttpGet("courses/{courseId:int}/contents")]
    public async Task<ActionResult<MoodleCourseModulesResponseDto>> GetCourseContents(
        int courseId,
        CancellationToken cancellationToken)
    {
        var client = await CreateMoodleClientAsync(cancellationToken);
        return Ok(await client.GetCourseContentsAsync(courseId, cancellationToken));
    }

    private async Task<IMoodleHttpClient> CreateMoodleClientAsync(CancellationToken cancellationToken)
    {
        var settings = await configuration.GetSettingsAsync(cancellationToken)
            ?? throw new InvalidOperationException("La integración Moodle no está configurada.");
        var httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
        return new MoodleHttpClient(httpClient, Options.Create(settings));
    }

    private async Task RecordFailedSyncAsync(
        int courseId,
        string error,
        CancellationToken cancellationToken)
    {
        var existingCourse = await dbContext.AcademicCourses.AsNoTracking()
            .SingleOrDefaultAsync(course => course.MoodleCourseId == courseId, cancellationToken);
        dbContext.AcademicSyncs.Add(new AcademicSyncRecord
        {
            MoodleCourseId = courseId,
            CourseName = existingCourse?.CourseName ?? $"Curso Moodle {courseId}",
            CompletedAtUtc = DateTime.UtcNow,
            Status = "Incompleta",
            ErrorSummary = Limit(error, 1000)
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string? Limit(string? value, int maxLength) =>
        value is null || value.Length <= maxLength ? value : value[..maxLength];

    private static DateTime? ToUtcDate(long unixSeconds) =>
        unixSeconds > 0
            ? DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime
            : null;

    public sealed record CursoResponse(int Id, string Name, string Status);
    public sealed record GradeRequest(IReadOnlyCollection<int> UserIds);
}
