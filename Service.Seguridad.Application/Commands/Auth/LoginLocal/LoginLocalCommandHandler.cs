using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
using Service.Seguridad.Application.Interfaces;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;
namespace Service.Seguridad.Application.Commands.Auth.LoginLocal;

public class LoginLocalCommandHandler : IRequestHandler<LoginLocalCommand, AuthResponse>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISesionRepository _sesionRepository;
    private readonly ITokenService _tokenService;
    private readonly IUsuarioRolRepository _usuarioRolRepository;
    private readonly IRolRepository _rolRepository;

    public LoginLocalCommandHandler(
        IUsuarioRepository usuarioRepository,
        ISesionRepository sesionRepository,
        ITokenService tokenService,
        IUsuarioRolRepository usuarioRolRepository,
        IRolRepository rolRepository)
    {
        _usuarioRepository = usuarioRepository;
        _sesionRepository = sesionRepository;
        _tokenService = tokenService;
        _usuarioRolRepository = usuarioRolRepository;
        _rolRepository = rolRepository;
    }

    public async Task<AuthResponse> Handle(LoginLocalCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(request.Email);

        if (usuario is null || usuario.ContrasenaHash is null)
            throw new UnauthorizedAccessException("Credenciales invÃ¡lidas.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.ContrasenaHash))
            throw new UnauthorizedAccessException("Credenciales invÃ¡lidas.");

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("Cuenta desactivada.");

        var roles = await _usuarioRolRepository.obtenerCodigosPorUsuarioIdAsync(usuario.IdUsuario);
        var rolesCodigos = roles.Select(r => r.Codigo);

        var accessToken = _tokenService.GenerarAccessToken(usuario.IdUsuario, usuario.Correo, rolesCodigos);
        var refreshTokenStr = _tokenService.GenerarRefreshToken();

        var sesion = TokenRefresco.Crear(
            usuario.IdUsuario,
            refreshTokenStr,
            TimeSpan.FromDays(7),
            request.IpOrigen,
            request.AgenteUsuario);

        await _sesionRepository.AgregarAsync(sesion);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenStr,
            ExpiresIn = _tokenService.AccessTokenDuracionSegundos
        };
    }
}

