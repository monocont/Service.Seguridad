using MediatR;
using Service.Seguridad.Application.DTOs.Usuario;
using Service.Seguridad.Domain.Interfaces;

namespace Service.Seguridad.Application.Queries.Usuario.ListarUsuarios;

public class ListarUsuariosQueryHandler : IRequestHandler<ListarUsuariosQuery, List<UsuarioPerfilDTO>>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ListarUsuariosQueryHandler(IUsuarioRepository usuarioRepository)
        => _usuarioRepository = usuarioRepository;

    public async Task<List<UsuarioPerfilDTO>> Handle(ListarUsuariosQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioRepository.ObtenerTodosAsync();

        return usuarios.Select(u => new UsuarioPerfilDTO
        {
            IdUsuario = u.IdUsuario,
            Correo = u.Correo,
            Nombres = u.Nombres,
            Apellidos = u.Apellidos,
            MetodoRegistro = u.MetodoRegistro,
            CorreoVerificado = u.CorreoVerificado,
            Activo = u.Activo
        }).ToList();
    }
}