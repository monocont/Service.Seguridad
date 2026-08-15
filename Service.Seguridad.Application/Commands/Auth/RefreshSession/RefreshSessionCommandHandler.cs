using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
using Service.Seguridad.Application.Interfaces;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;
namespace Service.Seguridad.Application.Commands.Auth.RefreshSession;

public class RefreshSessionCommandHandler : IRequestHandler<RefreshSessionCommand, AuthResponse>
{
    private readonly ISesionRepository _sesionRepository;
    private readonly ITokenService _tokenService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioRolRepository _usuarioRolRepository;

    public RefreshSessionCommandHandler(
        ISesionRepository sesionRepository,
        ITokenService tokenService,
        IUsuarioRepository usuarioRepository,
        IUsuarioRolRepository usuarioRolRepository)
    {
        _sesionRepository = sesionRepository;
        _tokenService = tokenService;
        _usuarioRepository = usuarioRepository;
        _usuarioRolRepository = usuarioRolRepository;
    }

    public async Task<AuthResponse> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
    {
        var sesionExistente = await _sesionRepository.ObtenerPorTokenAsync(request.RefreshToken);

        if (sesionExistente is null)
            throw new UnauthorizedAccessException("Token de refresco invÃ¡lido.");

        if (sesionExistente.EsRevocado)
        {
            await _sesionRepository.RevocarTodasPorUsuarioAsync(sesionExistente.IdUsuario);
            throw new UnauthorizedAccessException("Intento de reutilizaciÃ³n detectado. Sesiones revocadas.");
        }

        if (sesionExistente.EstaExpirado())
            throw new UnauthorizedAccessException("Token de refresco expirado.");

        sesionExistente.Revocar();
        await _sesionRepository.ActualizarAsync(sesionExistente);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(sesionExistente.IdUsuario);

        if (usuario is null || !usuario.Activo)
            throw new UnauthorizedAccessException("Cuenta no disponible.");

        var roles = await _usuarioRolRepository.obtenerCodigosPorUsuarioIdAsync(usuario.IdUsuario);
        var rolesCodigos = roles.Select(r => r.Codigo);

        var nuevoAccessToken = _tokenService.GenerarAccessToken(usuario.IdUsuario, usuario.Correo, rolesCodigos);
        var nuevoRefreshToken = _tokenService.GenerarRefreshToken();

        var nuevaSesion = TokenRefresco.Crear(
            usuario.IdUsuario,
            nuevoRefreshToken,
            TimeSpan.FromDays(7),
            null,
            null);

        await _sesionRepository.AgregarAsync(nuevaSesion);

        return new AuthResponse
        {
            AccessToken = nuevoAccessToken,
            RefreshToken = nuevoRefreshToken,
            ExpiresIn = _tokenService.AccessTokenDuracionSegundos
        };
    }
}

