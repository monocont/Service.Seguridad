using Service.Seguridad.Domain.Entities;

namespace Service.Seguridad.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    Task<Usuario?> ObtenerPorGoogleIdAsync(string googleId);
    Task<Usuario?> ObtenerPorIdAsync(Guid id);
    Task AgregarAsync(Usuario usuario);
    Task ActualizarAsync(Usuario usuario);
    Task<List<Usuario>> ObtenerTodosAsync();
}