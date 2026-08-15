using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Auth.RefreshSession;

public class RefreshSessionCommand : IRequest<AuthResponse>
{
    public string RefreshToken { get; set; }
    public string? IpOrigen { get; set; }
    public string? AgenteUsuario { get; set; }
}
