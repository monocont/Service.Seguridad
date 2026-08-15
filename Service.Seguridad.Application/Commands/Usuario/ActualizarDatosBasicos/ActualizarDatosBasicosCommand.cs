using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Usuario.ActualizarDatosBasicos;

public class ActualizarDatosBasicosCommand : IRequest<Unit>
{
    public Guid IdUsuario { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
}

