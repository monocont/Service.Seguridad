using System.ComponentModel.DataAnnotations;

namespace Service.Seguridad.API.DTOs;

public class RegistroLocalRequest
{
    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string Contrasena { get; set; } = "";

    [Required(ErrorMessage = "Los nombres son requeridos")]
    public string Nombres { get; set; } = "";

    [Required(ErrorMessage = "Los apellidos son requeridos")]
    public string Apellidos { get; set; } = "";
}
