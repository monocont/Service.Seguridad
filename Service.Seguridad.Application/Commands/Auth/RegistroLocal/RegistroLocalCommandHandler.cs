using MediatR;
using Microsoft.Extensions.Options;
using Service.Seguridad.Application.Common;
using Service.Seguridad.Application.DTOs.Auth;
using Service.Seguridad.Application.Interfaces;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;

namespace Service.Seguridad.Application.Commands.Auth.RegistroLocal;

public class RegistroLocalCommandHandler : IRequestHandler<RegistroLocalCommand, AuthResponse>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISesionRepository _sesionRepository;
    private readonly ITokenService _tokenService;
    private readonly IUsuarioRolRepository _usuarioRolRepository;
    private readonly IRolRepository _rolRepository;

    private readonly SesionOptions _sesionOptions;

    public RegistroLocalCommandHandler(
        IUsuarioRepository usuarioRepository,
        ISesionRepository sesionRepository,
        ITokenService tokenService,
        IUsuarioRolRepository usuarioRolRepository,
        IRolRepository rolRepository,
        IOptions<SesionOptions> sesionOptions)
    {
        _usuarioRepository = usuarioRepository;
        _sesionRepository = sesionRepository;
        _tokenService = tokenService;
        _usuarioRolRepository = usuarioRolRepository;
        _rolRepository = rolRepository;
        _sesionOptions = sesionOptions.Value;
    }

    public async Task<AuthResponse> Handle(RegistroLocalCommand request, CancellationToken cancellationToken)
    {
        var usuarioExistente = await _usuarioRepository.ObtenerPorCorreoAsync(request.Correo);
        if (usuarioExistente is not null)
            throw new ArgumentException("El usuario ya se encuentra registrado en el sistema. Por favor, inicie sesi\xc3\xb3n en lugar de registrarse nuevamente.");

        var contrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena);

        var usuarioNuevo = Service.Seguridad.Domain.Entities.Usuario.CrearLocal(
            request.Correo,
            request.Nombres,
            request.Apellidos,
            contrasenaHash);

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
            _sesionOptions.RefreshTokenDuracion,
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