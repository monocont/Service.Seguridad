using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;
namespace Service.Seguridad.Application.Commands.Usuario.ActualizarDatosBasicos;

public class ActualizarDatosBasicosCommandHandler : IRequestHandler<ActualizarDatosBasicosCommand, Unit>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ActualizarDatosBasicosCommandHandler(IUsuarioRepository usuarioRepository)
        => _usuarioRepository = usuarioRepository;

    public async Task<Unit> Handle(ActualizarDatosBasicosCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.IdUsuario);

        if (usuario is null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        if (string.IsNullOrWhiteSpace(request.Nombres))
            throw new ArgumentException("Los nombres no pueden estar vacÃ­os.");

        if (string.IsNullOrWhiteSpace(request.Apellidos))
            throw new ArgumentException("Los apellidos no pueden estar vacÃ­os.");

        usuario.ActualizarDatosBasicos(request.Nombres, request.Apellidos, request.IdUsuario.ToString());

        await _usuarioRepository.ActualizarAsync(usuario);

        return Unit.Value;
    }
}

