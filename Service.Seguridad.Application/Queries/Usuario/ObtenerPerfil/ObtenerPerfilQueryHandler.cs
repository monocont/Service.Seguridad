using MediatR;
using Service.Seguridad.Application.DTOs.Usuario;
using Service.Seguridad.Domain.Interfaces;
namespace Service.Seguridad.Application.Queries.Usuario.ObtenerPerfil;

public class ObtenerPerfilQueryHandler : IRequestHandler<ObtenerPerfilQuery, UsuarioPerfilDTO>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioRolRepository _usuarioRolRepository;

    public ObtenerPerfilQueryHandler(
        IUsuarioRepository usuarioRepository,
        IUsuarioRolRepository usuarioRolRepository)
    {
        _usuarioRepository = usuarioRepository;
        _usuarioRolRepository = usuarioRolRepository;
    }

    public async Task<UsuarioPerfilDTO> Handle(ObtenerPerfilQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.IdUsuario);

        if (usuario is null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        var roles = await _usuarioRolRepository.obtenerCodigosPorUsuarioIdAsync(usuario.IdUsuario);
        var rolesCodigos = roles.Select(r => r.Codigo).ToList();

        return new UsuarioPerfilDTO
        {
            IdUsuario = usuario.IdUsuario,
            Correo = usuario.Correo,
            Nombres = usuario.Nombres,
            Apellidos = usuario.Apellidos,
            MetodoRegistro = usuario.MetodoRegistro,
            TieneContrasena = usuario.ContrasenaHash != null,
            Roles = rolesCodigos
        };
    }
}

