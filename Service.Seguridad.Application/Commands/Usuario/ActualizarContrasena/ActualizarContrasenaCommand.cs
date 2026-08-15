using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Usuario.ActualizarContrasena;

public class ActualizarContrasenaCommand : IRequest<Unit>
{
    public Guid IdUsuario { get; set; }
    public string ContrasenaActual { get; set; }
    public string NuevaContrasena { get; set; }
}

