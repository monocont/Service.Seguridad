using System.ComponentModel.DataAnnotations;

namespace Service.Seguridad.API.DTOs;

public class EstablecerContrasenaRequest
{
    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    public string NuevaContrasena { get; set; } = string.Empty;
}
