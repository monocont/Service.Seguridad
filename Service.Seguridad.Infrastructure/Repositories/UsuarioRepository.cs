using Microsoft.EntityFrameworkCore;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;
using Service.Seguridad.Infrastructure.DbContext;

namespace Service.Seguridad.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SecurityDbContext _context;

    public UsuarioRepository(SecurityDbContext context) => _context = context;

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        => await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

    public async Task<Usuario?> ObtenerPorGoogleIdAsync(string googleId)
        => await _context.Usuarios.FirstOrDefaultAsync(u => u.GoogleId == googleId);

    public async Task<Usuario?> ObtenerPorIdAsync(Guid id)
        => await _context.Usuarios.FindAsync(id);

    public async Task AgregarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        // _context.Usuarios.Update(usuario); // No es necesario si la entidad ya está siendo rastreada (tracked) por EF Core
        await _context.SaveChangesAsync();
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
        => await _context.Usuarios.ToListAsync();
}