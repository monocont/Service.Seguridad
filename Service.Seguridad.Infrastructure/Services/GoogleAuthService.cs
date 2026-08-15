using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Service.Seguridad.Application.Interfaces;

namespace Service.Seguridad.Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<GooglePayload?> ValidarTokenAsync(string identityToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(identityToken);

        var audClaim = jsonToken.Audiences.FirstOrDefault();
        var expectedAudience = _configuration["Google:ClientId"];

        if (audClaim != expectedAudience)
            return null;

        var kid = jsonToken.Header.Kid;
        var jwks = await ObtenerLlavesPublicasAsync();

        if (!jwks.Keys.Any(k => k.Kid == kid))
            return null;

        return new GooglePayload
        {
            Subject = jsonToken.Subject,
            Email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "",
            Nombres = jsonToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "",
            Apellidos = jsonToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? ""
        };
    }

    private async Task<JwkSet> ObtenerLlavesPublicasAsync()
    {
        var response = await _httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");
        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return System.Text.Json.JsonSerializer.Deserialize<JwkSet>(response, options) ?? new JwkSet();
    }
}

public class JwkSet
{
    public List<JwkKey> Keys { get; set; } = new();
}

public class JwkKey
{
    public string Kid { get; set; } = "";
}