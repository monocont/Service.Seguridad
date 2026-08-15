using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
using Service.Seguridad.Application.Interfaces;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;
namespace Service.Seguridad.Application.Commands.Auth.LoginFederado;

public class LoginFederadoCommandHandler : IRequestHandler<LoginFederadoCommand, AuthResponse>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISesionRepository _sesionRepository;
    private readonly ITokenService _tokenService;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IUsuarioRolRepository _usuarioRolRepository;

    public LoginFederadoCommandHandler(
        IUsuarioRepository usuarioRepository,
        ISesionRepository sesionRepository,
        ITokenService tokenService,
        IGoogleAuthService googleAuthService,
        IUsuarioRolRepository usuarioRolRepository)
    {
        _usuarioRepository = usuarioRepository;
        _sesionRepository = sesionRepository;
        _tokenService = tokenService;
        _googleAuthService = googleAuthService;
        _usuarioRolRepository = usuarioRolRepository;
    }

    public async Task<AuthResponse> Handle(LoginFederadoCommand request, CancellationToken cancellationToken)
    {
        var payload = await _googleAuthService.ValidarTokenAsync(request.IdentityToken);

        if (payload is null)
            throw new UnauthorizedAccessException("Token de identidad invÃ¡lido.");

        var usuario = await _usuarioRepository.ObtenerPorGoogleIdAsync(payload.Subject);

        if (usuario is null)
            usuario = await _usuarioRepository.ObtenerPorCorreoAsync(payload.Email);

        if (usuario is null)
            throw new UnauthorizedAccessException("La cuenta no se encuentra registrada.");

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

