using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.Interfaces;
using Uta.Middleware.Infrastructure.Moodle;
using Uta.MoodleIntegration.WebApi.Services;

namespace Uta.MoodleIntegration.WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("internal/moodle")]
public sealed class InternalMoodleController(
    MoodleConfigurationService configuration,
    IHttpClientFactory httpClientFactory) : ControllerBase
{
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

    public sealed record GradeRequest(IReadOnlyCollection<int> UserIds);
}
