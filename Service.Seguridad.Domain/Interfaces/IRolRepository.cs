using Service.Seguridad.Domain.Entities;

namespace Service.Seguridad.Domain.Interfaces;

public interface IRolRepository
{
    Task<Rol?> obtenerPorCodigoAsync(string codigo);
    Task<List<Rol>> obtenerTodosAsync();
    Task<Rol?> obtenerPorIdAsync(Guid id);
    Task agregarAsync(Rol rol);
}
