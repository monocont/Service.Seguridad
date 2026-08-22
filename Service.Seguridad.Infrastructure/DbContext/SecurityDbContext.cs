using Microsoft.EntityFrameworkCore;
using Service.Seguridad.Domain.Entities;

namespace Service.Seguridad.Infrastructure.DbContext;

public class SecurityDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<TokenRefresco> TokensRefresco => Set<TokenRefresco>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();

    public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("seguridad");

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(150).IsRequired();
            entity.HasIndex(e => e.Correo).IsUnique();
            entity.Property(e => e.Nombres).HasColumnName("nombres").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apellidos).HasColumnName("apellidos").HasMaxLength(100).IsRequired();
            entity.Property(e => e.MetodoRegistro).HasColumnName("metodo_registro").HasMaxLength(20).IsRequired();
            entity.Property(e => e.ContrasenaHash).HasColumnName("contrasena_hash").HasMaxLength(255);
            entity.Property(e => e.GoogleId).HasColumnName("google_id").HasMaxLength(255);
            entity.HasIndex(e => e.GoogleId).IsUnique();
            entity.Property(e => e.CorreoVerificado).HasColumnName("correo_verificado").IsRequired();
            entity.Property(e => e.TokenVerificacion).HasColumnName("token_verificacion").HasMaxLength(255);
            entity.Property(e => e.ExpiraToken).HasColumnName("expira_token");
            entity.Property(e => e.UltimoAcceso).HasColumnName("ultimo_acceso");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por").HasMaxLength(150);
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion").IsRequired();
            entity.Property(e => e.ModificadoPor).HasColumnName("modificado_por").HasMaxLength(150);
            entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            entity.Property(e => e.Activo).HasColumnName("activo").IsRequired().HasDefaultValue(true);

            entity.HasQueryFilter(e => e.Activo);
        });

        modelBuilder.Entity<TokenRefresco>(entity =>
        {
            entity.ToTable("token_refresco");
            entity.HasKey(e => e.IdToken);
            entity.Property(e => e.IdToken).HasColumnName("id_token");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario").IsRequired();
            entity.Property(e => e.Token).HasColumnName("token").HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion").IsRequired();
            entity.Property(e => e.FechaExpiracion).HasColumnName("fecha_expiracion").IsRequired();
            entity.Property(e => e.EsRevocado).HasColumnName("es_revocado").IsRequired();
            entity.Property(e => e.FechaRevocacion).HasColumnName("fecha_revocacion");
            entity.Property(e => e.FechaInicioSesion).HasColumnName("fecha_inicio_sesion").IsRequired();
            entity.Property(e => e.IpOrigen).HasColumnName("ip_origen").HasMaxLength(45);
            entity.Property(e => e.AgenteUsuario).HasColumnName("agente_usuario").HasMaxLength(255);
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por").HasMaxLength(150);
            entity.Property(e => e.ModificadoPor).HasColumnName("modificado_por").HasMaxLength(150);
            entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            entity.Property(e => e.Activo).HasColumnName("activo").IsRequired().HasDefaultValue(true);

            entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.IdUsuario);

            entity.HasQueryFilter(e => e.Activo);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("rol");
            entity.HasKey(e => e.IdRol);
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Codigo).HasColumnName("codigo_rol").HasMaxLength(10).IsRequired();
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Nombre).HasColumnName("nombre_rol").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por").HasMaxLength(150);
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion").IsRequired();
            entity.Property(e => e.ModificadoPor).HasColumnName("modificado_por").HasMaxLength(150);
            entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            entity.Property(e => e.Activo).HasColumnName("activo").IsRequired().HasDefaultValue(true);

            entity.HasQueryFilter(e => e.Activo);
        });

        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.ToTable("usuario_rol");
            entity.HasKey(e => new { e.IdUsuario, e.IdRol });
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por").HasMaxLength(150);
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion").IsRequired();
            entity.Property(e => e.ModificadoPor).HasColumnName("modificado_por").HasMaxLength(150);
            entity.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            entity.Property(e => e.Activo).HasColumnName("activo").IsRequired().HasDefaultValue(true);

            entity.HasOne(e => e.Rol)
                .WithMany()
                .HasForeignKey(e => e.IdRol)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Usuario>()
                .WithMany(u => u.Roles)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => e.Activo);
        });
    }
}
