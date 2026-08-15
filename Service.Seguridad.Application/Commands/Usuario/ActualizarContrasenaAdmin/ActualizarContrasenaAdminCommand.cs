using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Usuario.ActualizarContrasenaAdmin;

public class ActualizarContrasenaAdminCommand : IRequest<Unit>
{
    public Guid IdUsuario { get; set; }
    public string NuevaContrasena { get; set; }
}

