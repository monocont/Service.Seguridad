using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Auth.RegistroLocal;

public class RegistroLocalCommand : IRequest<AuthResponse>
{
    public string Correo { get; set; } = "";
    public string Contrasena { get; set; } = "";
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string? IpOrigen { get; set; }
    public string? AgenteUsuario { get; set; }
}

