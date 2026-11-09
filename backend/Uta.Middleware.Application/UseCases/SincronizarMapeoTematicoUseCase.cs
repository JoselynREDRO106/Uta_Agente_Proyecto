using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Application.Interfaces;
using Uta.Middleware.Domain.Entities;

namespace Uta.Middleware.Application.UseCases;

/// <summary>
/// Implementación del caso de uso para sincronizar el mapeo temático desde Moodle.
/// Usa core_course_get_contents para obtener la organización de secciones y módulos.
/// </summary>
public class SincronizarMapeoTematicoUseCase : ISincronizarMapeoTematicoUseCase
{
    private readonly IMoodleHttpClient _moodleHttpClient;
    private readonly IEduMonitorRepository _repository;
    private readonly IMoodleAcademicSyncClient? _integrationSyncClient;

    public SincronizarMapeoTematicoUseCase(
        IMoodleHttpClient moodleHttpClient,
        IEduMonitorRepository repository,
        IMoodleAcademicSyncClient? integrationSyncClient = null)
    {
        _moodleHttpClient = moodleHttpClient;
        _repository = repository;
        _integrationSyncClient = integrationSyncClient;
    }

    public async Task<IEnumerable<MapeoTematico>> EjecutarAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        string courseName;
        MoodleCourseModulesResponseDto response;
        if (_integrationSyncClient is not null)
        {
            var snapshot = await _integrationSyncClient.SynchronizeCourseAndGetSnapshotAsync(
                courseId, cancellationToken);
            courseName = snapshot.CourseName;
            response = snapshot.Contents;
        }
        else
        {
            var courseTask = _moodleHttpClient.GetCourseAsync(courseId, cancellationToken);
            var contentsTask = _moodleHttpClient.GetCourseContentsAsync(courseId, cancellationToken);
            await Task.WhenAll(courseTask, contentsTask);
            var course = await courseTask;
            courseName = course.FullName;
            response = await contentsTask;

            var mapeos = MapContents(courseId, courseName, response);
            await _repository.SaveCourseContentsAsync(course, response, cancellationToken);
            return mapeos;
        }

        return MapContents(courseId, courseName, response);
    }

    private static List<MapeoTematico> MapContents(
        int courseId,
        string courseName,
        MoodleCourseModulesResponseDto response)
    {
        var mapeos = new List<MapeoTematico>();

        foreach (var section in response.Sections)
        {
            // Cada sección representa una Unidad Temática
            foreach (var module in section.Modules)
            {
                mapeos.Add(new MapeoTematico
                {
                    MoodleCourseId    = courseId,
                    NombreAsignatura  = courseName,
                    MoodleActivityId  = module.Id,
                    NombreActividad   = module.Name,
                    UnidadTematica    = string.IsNullOrWhiteSpace(section.Name)
                                            ? $"Unidad {section.Section}"
                                            : section.Name,
                    ContenidoMinimo   = module.Name,
                    Ponderacion       = 0m  // Será completado por el DBA con los scripts DDL
                });
            }
        }

        return mapeos;
    }
}
