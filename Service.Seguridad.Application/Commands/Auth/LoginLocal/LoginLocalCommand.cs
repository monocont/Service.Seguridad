using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Auth.LoginLocal;

public class LoginLocalCommand : IRequest<AuthResponse>
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? IpOrigen { get; set; }
    public string? AgenteUsuario { get; set; }
}