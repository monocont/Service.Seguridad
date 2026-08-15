using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
using Service.Seguridad.Application.Interfaces;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;

namespace Service.Seguridad.Application.Commands.Auth.RegistroGoogle;

public class RegistroGoogleCommandHandler : IRequestHandler<RegistroGoogleCommand, AuthResponse>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISesionRepository _sesionRepository;
    private readonly ITokenService _tokenService;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IUsuarioRolRepository _usuarioRolRepository;
    private readonly IRolRepository _rolRepository;

    public RegistroGoogleCommandHandler(
        IUsuarioRepository usuarioRepository,
        ISesionRepository sesionRepository,
        ITokenService tokenService,
        IGoogleAuthService googleAuthService,
        IUsuarioRolRepository usuarioRolRepository,
        IRolRepository rolRepository)
    {
        _usuarioRepository = usuarioRepository;
        _sesionRepository = sesionRepository;
        _tokenService = tokenService;
        _googleAuthService = googleAuthService;
        _usuarioRolRepository = usuarioRolRepository;
        _rolRepository = rolRepository;
    }

    public async Task<AuthResponse> Handle(RegistroGoogleCommand request, CancellationToken cancellationToken)
    {
        var payload = await _googleAuthService.ValidarTokenAsync(request.IdentityToken);

        if (payload is null)
            throw new UnauthorizedAccessException("Token de identidad inv\xc3\xa1lido.");

        var usuario = await _usuarioRepository.ObtenerPorGoogleIdAsync(payload.Subject);

        if (usuario is null)
            usuario = await _usuarioRepository.ObtenerPorCorreoAsync(payload.Email);

        if (usuario is not null)
            throw new ArgumentException("El usuario ya se encuentra registrado en el sistema. Por favor, inicie sesi\xc3\xb3n en lugar de registrarse nuevamente.");

        var usuarioNuevo = Service.Seguridad.Domain.Entities.Usuario.CrearGoogle(
            payload.Email,
            payload.Nombres,
            payload.Apellidos,
            payload.Subject);

        await _usuarioRepository.AgregarAsync(usuarioNuevo);

        var rolUser = await _rolRepository.obtenerPorCodigoAsync("USER");

        if (rolUser != null)
        {
            await _usuarioRolRepository.asignarAsync(usuarioNuevo.IdUsuario, rolUser.IdRol);
        }

        var roles = await _usuarioRolRepository.obtenerCodigosPorUsuarioIdAsync(usuarioNuevo.IdUsuario);
        var rolesCodigos = roles.Select(r => r.Codigo);

        var accessToken = _tokenService.GenerarAccessToken(usuarioNuevo.IdUsuario, usuarioNuevo.Correo, rolesCodigos);
        var refreshTokenStr = _tokenService.GenerarRefreshToken();

        var sesion = TokenRefresco.Crear(
            usuarioNuevo.IdUsuario,
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