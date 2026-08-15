using MediatR;
using Service.Seguridad.Application.DTOs.Usuario;

namespace Service.Seguridad.Application.Queries.Usuario.ListarUsuarios;

public class ListarUsuariosQuery : IRequest<List<UsuarioPerfilDTO>>
{
}