namespace Service.Seguridad.Domain.Entities;

public class Rol : EntidadAuditoria
{
    public Guid IdRol { get; private set; }
    public string Codigo { get; private set; }
    public string Nombre { get; private set; }

    private Rol() { }

    public static Rol Crear(string codigo, string nombre)
    {
        return new Rol
        {
            IdRol = Guid.NewGuid(),
            Codigo = codigo,
            Nombre = nombre,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }
}
