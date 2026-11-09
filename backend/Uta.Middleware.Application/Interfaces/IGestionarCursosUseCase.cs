using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.Application.Interfaces;

public interface IGestionarCursosUseCase
{
    Task<IReadOnlyCollection<CursoDto>> ObtenerAsync(
        int? userId,
        CancellationToken cancellationToken = default);

    Task<CursoDto> SincronizarAsync(
        int courseId,
        CancellationToken cancellationToken = default);
}
