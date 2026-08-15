using Microsoft.EntityFrameworkCore;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;
using Service.Seguridad.Infrastructure.DbContext;

namespace Service.Seguridad.Infrastructure.Repositories;

public class RolRepository : IRolRepository
{
    private readonly SecurityDbContext _context;

    public RolRepository(SecurityDbContext context) => _context = context;

    public async Task<Rol?> obtenerPorCodigoAsync(string codigo)
        => await _context.Roles.FirstOrDefaultAsync(r => r.Codigo == codigo);

    public async Task<List<Rol>> obtenerTodosAsync()
        => await _context.Roles.ToListAsync();

    public async Task<Rol?> obtenerPorIdAsync(Guid id)
        => await _context.Roles.FindAsync(id);

    public async Task agregarAsync(Rol rol)
    {
        await _context.Roles.AddAsync(rol);
        await _context.SaveChangesAsync();
    }
}
