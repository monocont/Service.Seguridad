using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Usuario.EstablecerContrasena;

public class EstablecerContrasenaCommand : IRequest<Unit>
{
    public Guid IdUsuario { get; set; }
    public string NuevaContrasena { get; set; } = string.Empty;
}

