using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
using Service.Seguridad.Domain.Interfaces;
namespace Service.Seguridad.Application.Commands.Auth.CerrarSesion;

public class CerrarSesionCommandHandler : IRequestHandler<CerrarSesionCommand, bool>
{
    private readonly ISesionRepository _sesionRepository;

    public CerrarSesionCommandHandler(ISesionRepository sesionRepository)
    {
        _sesionRepository = sesionRepository;
    }

    public async Task<bool> Handle(CerrarSesionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return false;

        var sesion = await _sesionRepository.ObtenerPorTokenAsync(request.RefreshToken);

        if (sesion == null || sesion.EsRevocado)
            return false;

        sesion.Revocar();
        await _sesionRepository.ActualizarAsync(sesion);

        return true;
    }
}

