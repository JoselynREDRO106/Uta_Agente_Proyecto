using Uta.Middleware.Application.DTOs;

namespace Uta.Middleware.Application.Interfaces;

public interface IAuthenticationRepository
{
    Task<CuentaUsuarioDto?> FindUserAsync(string email, CancellationToken cancellationToken = default);

    Task<UsuarioAutenticadoDto?> CreateBootstrapAdministratorAsync(
        string email,
        string passwordHash,
        CancellationToken cancellationToken = default);

    Task<UsuarioAutenticadoDto> CreateUserAsync(
        string name,
        string email,
        string passwordHash,
        string role,
        IReadOnlyCollection<int> moodleCourseIds,
        CancellationToken cancellationToken = default);
}
