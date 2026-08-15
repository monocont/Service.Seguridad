namespace Service.Seguridad.Domain.Entities;

public class Usuario : EntidadAuditoria
{
    public Guid IdUsuario { get; private set; }
    public string Correo { get; private set; }
    public string Nombres { get; private set; }
    public string Apellidos { get; private set; }
    public string MetodoRegistro { get; private set; }
    public string? ContrasenaHash { get; private set; }
    public string? GoogleId { get; private set; }
    public bool CorreoVerificado { get; private set; }
    public string? TokenVerificacion { get; private set; }
    public DateTime? ExpiraToken { get; private set; }
    public DateTime? UltimoAcceso { get; private set; }
    public ICollection<UsuarioRol> Roles { get; private set; } = new List<UsuarioRol>();

    private Usuario() { }

    public static Usuario CrearLocal(string correo, string nombres, string apellidos, string contrasenaHash)
    {
        return new Usuario
        {
            IdUsuario = Guid.NewGuid(),
            Correo = correo,
            Nombres = nombres,
            Apellidos = apellidos,
            MetodoRegistro = "Local",
            ContrasenaHash = contrasenaHash,
            CorreoVerificado = false,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static Usuario CrearGoogle(string correo, string nombres, string apellidos, string googleId)
    {
        return new Usuario
        {
            IdUsuario = Guid.NewGuid(),
            Correo = correo,
            Nombres = nombres,
            Apellidos = apellidos,
            MetodoRegistro = "Google",
            GoogleId = googleId,
            CorreoVerificado = true,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public void VerificarCorreo()
    {
        CorreoVerificado = true;
        TokenVerificacion = null;
        ExpiraToken = null;
    }

    public void ActualizarDatosBasicos(string nombres, string apellidos, string usuarioModificacion)
    {
        Nombres = nombres;
        Apellidos = apellidos;
        ModificadoPor = usuarioModificacion;
        FechaModificacion = DateTime.UtcNow;
    }

    public void ActualizarContrasena(string contrasenaHash)
    {
        ContrasenaHash = contrasenaHash;
        FechaModificacion = DateTime.UtcNow;
    }
}