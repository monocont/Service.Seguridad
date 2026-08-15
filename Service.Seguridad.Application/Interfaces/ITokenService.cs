namespace Service.Seguridad.Application.Interfaces;

public interface ITokenService
{
    string GenerarAccessToken(Guid idUsuario, string correo, IEnumerable<string> roles);
    string GenerarRefreshToken();
    int AccessTokenDuracionSegundos { get; }
}

