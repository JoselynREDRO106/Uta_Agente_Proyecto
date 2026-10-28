using Uta.Middleware.Application.DTOs.Moodle;
using Uta.Middleware.Domain.Entities;

namespace Uta.Middleware.Application.Interfaces;

/// <summary>
/// Caso de uso: Ingesta de calificaciones desde Moodle al sistema EduMonitor.
/// </summary>
public interface ISincronizarCalificacionesUseCase
{
    /// <summary>
    /// Sincroniza las calificaciones de un curso desde Moodle.
    /// </summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de calificaciones sincronizadas.</returns>
    Task<IEnumerable<Calificacion>> EjecutarAsync(
        int courseId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Caso de uso: Sincronización del mapeo temático desde Moodle.
/// </summary>
public interface ISincronizarMapeoTematicoUseCase
{
    /// <summary>
    /// Sincroniza la estructura temática de un curso desde Moodle.
    /// </summary>
    /// <param name="courseId">ID del curso en Moodle.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de mapeos temáticos sincronizados.</returns>
    Task<IEnumerable<MapeoTematico>> EjecutarAsync(
        int courseId,
        CancellationToken cancellationToken = default);
}
