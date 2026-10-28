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

    public SincronizarMapeoTematicoUseCase(IMoodleHttpClient moodleHttpClient)
    {
        _moodleHttpClient = moodleHttpClient;
    }

    public async Task<IEnumerable<MapeoTematico>> EjecutarAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var response = await _moodleHttpClient.GetCourseContentsAsync(courseId, cancellationToken);

        var mapeos = new List<MapeoTematico>();

        foreach (var section in response.Sections)
        {
            // Cada sección representa una Unidad Temática
            foreach (var module in section.Modules)
            {
                mapeos.Add(new MapeoTematico
                {
                    MoodleCourseId    = courseId,
                    NombreAsignatura  = $"Curso-{courseId}",
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

        return mapeos;
    }
}
