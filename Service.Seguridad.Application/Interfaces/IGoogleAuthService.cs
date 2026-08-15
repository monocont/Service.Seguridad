namespace Service.Seguridad.Application.Interfaces;

public interface IGoogleAuthService
{
    Task<GooglePayload?> ValidarTokenAsync(string identityToken);
}

public class GooglePayload
{
    public string Subject { get; set; }
    public string Email { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
}

