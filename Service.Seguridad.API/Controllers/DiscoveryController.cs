using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Seguridad.Infrastructure.Services;

namespace Service.Seguridad.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route(".well-known")]
public class DiscoveryController : ControllerBase
{
    private readonly RsaKeyProvider _rsaKeyProvider;
    private readonly IConfiguration _configuration;

    public DiscoveryController(RsaKeyProvider rsaKeyProvider, IConfiguration configuration)
    {
        _rsaKeyProvider = rsaKeyProvider;
        _configuration = configuration;
    }

    [HttpGet("openid-configuration")]
    public IActionResult GetOpenIdConfiguration()
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "Service.Seguridad";
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        
        return Ok(new
        {
            issuer = issuer,
            jwks_uri = $"{baseUrl}/.well-known/jwks"
        });
    }

    [HttpGet("jwks")]
    public IActionResult GetJwks()
    {
        var jwk = _rsaKeyProvider.GetJsonWebKey();
        var json = JsonSerializer.Serialize(new { keys = new[] { jwk } }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return Content(json, "application/json");
    }
}
