using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
namespace Service.Seguridad.Application.Commands.Auth.CerrarSesion;

public class CerrarSesionCommand : IRequest<bool>
{
    public string RefreshToken { get; set; } = string.Empty;
}

