using Service.Seguridad.Domain.Entities;

namespace Service.Seguridad.Domain.Interfaces;

public interface IUsuarioRolRepository
{
    Task<List<UsuarioRol>> obtenerPorUsuarioIdAsync(Guid idUsuario);
    Task<List<Rol>> obtenerCodigosPorUsuarioIdAsync(Guid idUsuario);
    Task asignarAsync(Guid idUsuario, Guid idRol);
    Task quitarAsync(Guid idUsuario, Guid idRol);
    Task quitarTodosPorUsuarioAsync(Guid idUsuario);
}
