namespace Service.Seguridad.API.DTOs;

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class FederatedLoginRequest
{
    public required string IdentityToken { get; set; }
}

public class RegistroGoogleRequest
{
    public required string IdentityToken { get; set; }
}

public class ActualizarDatosBasicosRequest
{
    public required string Nombres { get; set; }
    public required string Apellidos { get; set; }
}

public class ActualizarContrasenaRequest
{
    public required string ContrasenaActual { get; set; }
    public required string NuevaContrasena { get; set; }
}

public class ActualizarDatosBasicosAdminRequest
{
    public Guid IdUsuario { get; set; }
    public required string Nombres { get; set; }
    public required string Apellidos { get; set; }
}

public class ActualizarContrasenaAdminRequest
{
    public Guid IdUsuario { get; set; }
    public required string NuevaContrasena { get; set; }
}

public class ErrorResponse
{
    public required string Error { get; set; }
    public required string Message { get; set; }
}