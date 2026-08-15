namespace Service.Seguridad.Domain.Entities;

public class UsuarioRol : EntidadAuditoria
{
    public Guid IdUsuario { get; private set; }
    public Guid IdRol { get; private set; }
    public Rol Rol { get; private set; }

    private UsuarioRol() { }

    public static UsuarioRol Crear(Guid idUsuario, Guid idRol)
    {
        return new UsuarioRol
        {
            IdUsuario = idUsuario,
            IdRol = idRol,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }
}
