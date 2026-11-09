using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Uta.Identity.WebApi.Data;
using Uta.Middleware.Application.DTOs;
using Uta.Middleware.Application.Security;

namespace Uta.Identity.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController(
    IdentityRepository accounts,
    IConfiguration configuration) : ControllerBase
{
    private const string TeacherRole = "Docente";
    private const string AdminRole = "Administrador técnico";

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDto>> Login(
        CredencialesDto credentials,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(credentials.Email)
            || string.IsNullOrWhiteSpace(credentials.Password))
        {
            return Unauthorized(new { message = "Credenciales incorrectas." });
        }

        var account = await accounts.FindUserAsync(credentials.Email, cancellationToken);
        var hash = account?.PasswordHash ?? DummyPasswordHash.Value;
        var passwordIsValid = PasswordHashing.Verify(credentials.Password, hash);
        if (account is null || !passwordIsValid)
        {
            return Unauthorized(new { message = "Credenciales incorrectas." });
        }

        var signingKey = configuration["Authentication:Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Falta configurar Authentication:Jwt:SigningKey.");
        var issuer = configuration["Authentication:Jwt:Issuer"] ?? "EduMonitor";
        var audience = configuration["Authentication:Jwt:Audience"] ?? "EduMonitor";
        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, account.Name),
            new(ClaimTypes.Email, account.Email),
            new(ClaimTypes.Role, account.Role),
            new("course_assignments", "managed")
        };
        claims.AddRange(account.MoodleCourseIds
            .Where(courseId => courseId > 0)
            .Distinct()
            .Select(courseId => new Claim("course_id", courseId.ToString(CultureInfo.InvariantCulture))));

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256));

        return Ok(new TokenDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            account.Name,
            account.Role));
    }

    [AllowAnonymous]
    [HttpPost("bootstrap-admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BootstrapAdministrator(
        [FromHeader(Name = "X-Bootstrap-Key")] string? bootstrapKey,
        CredencialesDto credentials,
        CancellationToken cancellationToken)
    {
        var configuredKey = configuration["Authentication:BootstrapKey"];
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            return NotFound();
        }

        if (!FixedTimeEquals(configuredKey, bootstrapKey))
        {
            return Unauthorized();
        }

        if (!IsValidAccount(credentials.Email, credentials.Password, out var validationError))
        {
            return BadRequest(new { message = validationError });
        }

        try
        {
            var account = await accounts.CreateBootstrapAdministratorAsync(
                credentials.Email,
                PasswordHashing.Hash(credentials.Password),
                cancellationToken);
            if (account is null)
            {
                return Conflict(new { message = "La cuenta administrativa inicial ya fue creada." });
            }

            return Created("/api/auth/login", new
            {
                account.Nombre,
                account.Email,
                account.Rol
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [Authorize(Roles = AdminRole)]
    [HttpPost("usuarios")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        UsuarioCreacionDto request,
        CancellationToken cancellationToken)
    {
        if (request.Rol != TeacherRole && request.Rol != AdminRole)
        {
            return BadRequest(new { message = "El rol debe ser Docente o Administrador técnico." });
        }

        if (request.CursosMoodle is null || request.CursosMoodle.Any(courseId => courseId <= 0))
        {
            return BadRequest(new { message = "La lista de cursos debe contener IDs positivos; usa [] si no aplica." });
        }

        if (!IsValidAccount(request.Email, request.Password, out var validationError)
            || string.IsNullOrWhiteSpace(request.Nombre)
            || request.Nombre.Trim().Length > 150)
        {
            return BadRequest(new { message = validationError ?? "El nombre es obligatorio." });
        }

        if (request.Rol == AdminRole && request.CursosMoodle.Count > 0)
        {
            return BadRequest(new { message = "Las cuentas administrativas no reciben cursos asignados." });
        }

        try
        {
            var account = await accounts.CreateUserAsync(
                request.Nombre,
                request.Email,
                PasswordHashing.Hash(request.Password),
                request.Rol,
                request.CursosMoodle,
                cancellationToken);
            return Created("/api/auth/login", new
            {
                account.Nombre,
                account.Email,
                account.Rol
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static bool IsValidAccount(string email, string password, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            error = "El correo y la contraseña son obligatorios.";
            return false;
        }

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            error = "Ingresa un correo válido.";
            return false;
        }

        if (email.Length > 254)
        {
            error = "El correo excede la longitud permitida.";
            return false;
        }

        if (password.Length < 12 || password.Length > 256)
        {
            error = "La contraseña debe tener entre 12 y 256 caracteres.";
            return false;
        }

        return true;
    }

    private static bool FixedTimeEquals(string expected, string? provided)
    {
        if (provided is null)
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        return expectedBytes.Length == providedBytes.Length
            && System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                expectedBytes, providedBytes);
    }

    private static class DummyPasswordHash
    {
        public static readonly string Value = PasswordHashing.Hash(
            Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)));
    }
}
