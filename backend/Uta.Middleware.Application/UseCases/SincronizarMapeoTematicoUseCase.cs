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

    public SincronizarMapeoTematicoUseCase(
        IMoodleHttpClient moodleHttpClient,
        IEduMonitorRepository repository)
    {
        _moodleHttpClient = moodleHttpClient;
        _repository = repository;
    }

    public async Task<IEnumerable<MapeoTematico>> EjecutarAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var courseTask = _moodleHttpClient.GetCourseAsync(courseId, cancellationToken);
        var contentsTask = _moodleHttpClient.GetCourseContentsAsync(courseId, cancellationToken);
        await Task.WhenAll(courseTask, contentsTask);

        var course = await courseTask;
        var response = await contentsTask;

        var mapeos = new List<MapeoTematico>();

        foreach (var section in response.Sections)
        {
            // Cada sección representa una Unidad Temática
            foreach (var module in section.Modules)
            {
                mapeos.Add(new MapeoTematico
                {
                    MoodleCourseId    = courseId,
                    NombreAsignatura  = course.FullName,
                    MoodleGradeItemId = module.Instance ?? module.Id,
                    NombreActividad   = module.Name,
                    UnidadTematica    = string.IsNullOrWhiteSpace(section.Name)
                                            ? $"Unidad {section.Section}"
                                            : section.Name,
                    ContenidoMinimo   = module.Name,
                    Ponderacion       = 0m  // Será completado por el DBA con los scripts DDL
                });
            }
        }

        await _repository.SaveCourseContentsAsync(course, response, cancellationToken);

        return mapeos;
    }
}
