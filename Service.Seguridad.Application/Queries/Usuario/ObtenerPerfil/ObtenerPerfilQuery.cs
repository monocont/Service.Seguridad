using MediatR;
using Service.Seguridad.Application.DTOs.Usuario;
namespace Service.Seguridad.Application.Queries.Usuario.ObtenerPerfil;

public class ObtenerPerfilQuery : IRequest<UsuarioPerfilDTO>
{
    public Guid IdUsuario { get; set; }
}

