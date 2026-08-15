using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Auth.LoginFederado;

public class LoginFederadoCommand : IRequest<AuthResponse>
{
    public string IdentityToken { get; set; }
    public string? IpOrigen { get; set; }
    public string? AgenteUsuario { get; set; }
}
