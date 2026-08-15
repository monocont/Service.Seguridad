namespace Service.Seguridad.Application.DTOs.Usuario;

public class UsuarioPerfilDTO
{
    public Guid IdUsuario { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string MetodoRegistro { get; set; } = string.Empty;
    public bool TieneContrasena { get; set; }
    public bool CorreoVerificado { get; set; }
    public bool Activo { get; set; }
    public List<string> Roles { get; set; } = new();
}
