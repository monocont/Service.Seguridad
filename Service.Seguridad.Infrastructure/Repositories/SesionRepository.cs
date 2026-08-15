using Microsoft.EntityFrameworkCore;
using Service.Seguridad.Domain.Entities;
using Service.Seguridad.Domain.Interfaces;
using Service.Seguridad.Infrastructure.DbContext;

namespace Service.Seguridad.Infrastructure.Repositories;

public class SesionRepository : ISesionRepository
{
    private readonly SecurityDbContext _context;

    public SesionRepository(SecurityDbContext context) => _context = context;

    public async Task<TokenRefresco?> ObtenerPorTokenAsync(string token)
        => await _context.TokensRefresco.FirstOrDefaultAsync(t => t.Token == token);

    public async Task AgregarAsync(TokenRefresco sesion)
    {
        await _context.TokensRefresco.AddAsync(sesion);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(TokenRefresco sesion)
    {
        _context.TokensRefresco.Update(sesion);
        await _context.SaveChangesAsync();
    }

    public async Task RevocarTodasPorUsuarioAsync(Guid idUsuario)
    {
        var sesionesActivas = await _context.TokensRefresco
            .Where(t => t.IdUsuario == idUsuario && !t.EsRevocado)
            .ToListAsync();

        foreach (var sesion in sesionesActivas)
            sesion.Revocar();

        await _context.SaveChangesAsync();
    }
}