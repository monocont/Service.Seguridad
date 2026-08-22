using MediatR;
using Microsoft.Extensions.Options;
using Service.Seguridad.Application.Common;
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
    private readonly SesionOptions _sesionOptions;

    public RefreshSessionCommandHandler(
        ISesionRepository sesionRepository,
        ITokenService tokenService,
        IUsuarioRepository usuarioRepository,
        IUsuarioRolRepository usuarioRolRepository,
        IOptions<SesionOptions> sesionOptions)
    {
        _sesionRepository = sesionRepository;
        _tokenService = tokenService;
        _usuarioRepository = usuarioRepository;
        _usuarioRolRepository = usuarioRolRepository;
        _sesionOptions = sesionOptions.Value;
    }

    public async Task<AuthResponse> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
    {
        var sesionExistente = await _sesionRepository.ObtenerPorTokenAsync(request.RefreshToken);

        if (sesionExistente is null)
            throw new UnauthorizedAccessException("Token de refresco invÃ¡lido.");

        if (sesionExistente.EsRevocado)
        {
            // Margen de gracia: refreshes casi simultáneos (varias peticiones 401 en paralelo)
            // llegan con el token recién rotado. Dentro de la ventana se trata como refresh válido;
            // fuera de ella es reutilización real y se revocan todas las sesiones del usuario.
            var enVentanaGracia = sesionExistente.FechaRevocacion.HasValue &&
                DateTime.UtcNow <= sesionExistente.FechaRevocacion.Value.AddSeconds(30);

            if (!enVentanaGracia)
            {
                await _sesionRepository.RevocarTodasPorUsuarioAsync(sesionExistente.IdUsuario);
                throw new UnauthorizedAccessException("Intento de reutilizaciÃ³n detectado. Sesiones revocadas.");
            }
        }

        if (sesionExistente.EstaExpirado())
            throw new UnauthorizedAccessException("Token de refresco expirado.");

        // Límite absoluto: sin importar la actividad, la sesión continua no puede superar el máximo configurado.
        if (sesionExistente.ExcedioLimiteAbsoluto(_sesionOptions.LimiteAbsolutoDuracion))
        {
            await _sesionRepository.RevocarTodasPorUsuarioAsync(sesionExistente.IdUsuario);
            throw new UnauthorizedAccessException("SesiÃ³n excediÃ³ el tiempo mÃ¡ximo permitido. Inicie sesiÃ³n nuevamente.");
        }

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
            _sesionOptions.RefreshTokenDuracion,
            null,
            null,
            // La sesión rotada hereda el inicio original para que el límite absoluto se mida desde el login.
            sesionExistente.FechaInicioSesion);

        await _sesionRepository.AgregarAsync(nuevaSesion);

        return new AuthResponse
        {
            AccessToken = nuevoAccessToken,
            RefreshToken = nuevoRefreshToken,
            ExpiresIn = _tokenService.AccessTokenDuracionSegundos
        };
    }
}

