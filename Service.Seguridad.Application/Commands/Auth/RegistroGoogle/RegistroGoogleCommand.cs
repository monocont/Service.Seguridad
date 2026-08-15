using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Auth.RegistroGoogle;

public class RegistroGoogleCommand : IRequest<AuthResponse>
{
    public string IdentityToken { get; set; }
    public string? IpOrigen { get; set; }
    public string? AgenteUsuario { get; set; }
}

