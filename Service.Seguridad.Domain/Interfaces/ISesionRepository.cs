using Service.Seguridad.Domain.Entities;

namespace Service.Seguridad.Domain.Interfaces;

public interface ISesionRepository
{
    Task<TokenRefresco?> ObtenerPorTokenAsync(string token);
    Task AgregarAsync(TokenRefresco sesion);
    Task ActualizarAsync(TokenRefresco sesion);
    Task RevocarTodasPorUsuarioAsync(Guid idUsuario);
}