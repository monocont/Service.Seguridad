using Microsoft.EntityFrameworkCore;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;
using Service.Seguridad.Infrastructure.DbContext;

namespace Service.Seguridad.Infrastructure.Repositories;

public class UsuarioRoleRepository : IUsuarioRolRepository
{
    private readonly SecurityDbContext _context;

    public UsuarioRoleRepository(SecurityDbContext context) => _context = context;

    public async Task<List<UsuarioRol>> obtenerPorUsuarioIdAsync(Guid idUsuario)
        => await _context.UsuarioRoles
            .Include(ur => ur.Rol)
            .Where(ur => ur.IdUsuario == idUsuario)
            .ToListAsync();

    public async Task<List<Rol>> obtenerCodigosPorUsuarioIdAsync(Guid idUsuario)
        => await _context.UsuarioRoles
            .Where(ur => ur.IdUsuario == idUsuario)
            .Select(ur => ur.Rol)
            .ToListAsync();

    public async Task asignarAsync(Guid idUsuario, Guid idRol)
    {
        var usuarioRol = UsuarioRol.Crear(idUsuario, idRol);
        await _context.UsuarioRoles.AddAsync(usuarioRol);
        await _context.SaveChangesAsync();
    }

    public async Task quitarAsync(Guid idUsuario, Guid idRol)
    {
        var usuarioRol = await _context.UsuarioRoles
            .FirstOrDefaultAsync(ur => ur.IdUsuario == idUsuario && ur.IdRol == idRol);

        if (usuarioRol != null)
        {
            usuarioRol.MarcarComoEliminado();
            await _context.SaveChangesAsync();
        }
    }

    public async Task quitarTodosPorUsuarioAsync(Guid idUsuario)
    {
        var usuarioRoles = await _context.UsuarioRoles
            .Where(ur => ur.IdUsuario == idUsuario)
            .ToListAsync();

        foreach (var ur in usuarioRoles)
            ur.MarcarComoEliminado();

        await _context.SaveChangesAsync();
    }
}
