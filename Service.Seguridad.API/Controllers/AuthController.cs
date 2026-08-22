using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Seguridad.API.DTOs;
using Service.Seguridad.Application.Commands.Auth.LoginLocal;
using Service.Seguridad.Application.Commands.Auth.LoginFederado;
using Service.Seguridad.Application.Commands.Auth.RegistroLocal;
using Service.Seguridad.Application.Commands.Auth.RegistroGoogle;
using Service.Seguridad.Application.Commands.Auth.RefreshSession;
using Service.Seguridad.Application.Commands.Auth.CerrarSesion;

namespace Service.Seguridad.API.Controllers;

[ApiController]
[Route("api/v1/seguridad/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _environment;
    private readonly Application.Common.SesionOptions _sesionOptions;

    public AuthController(
        IMediator mediator,
        IWebHostEnvironment environment,
        Microsoft.Extensions.Options.IOptions<Application.Common.SesionOptions> sesionOptions)
    {
        _mediator = mediator;
        _environment = environment;
        _sesionOptions = sesionOptions.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var agente = HttpContext.Request.Headers.UserAgent.ToString();

        var command = new LoginLocalCommand
        {
            Email = request.Email,
            Password = request.Password,
            IpOrigen = ip,
            AgenteUsuario = agente
        };

        var result = await _mediator.Send(command);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new Application.DTOs.Auth.AuthResponse
        {
            AccessToken = result.AccessToken,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpPost("federated-login")]
    public async Task<IActionResult> FederatedLogin([FromBody] FederatedLoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var agente = HttpContext.Request.Headers.UserAgent.ToString();

        var command = new LoginFederadoCommand
        {
            IdentityToken = request.IdentityToken,
            IpOrigen = ip,
            AgenteUsuario = agente
        };

        var result = await _mediator.Send(command);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new Application.DTOs.Auth.AuthResponse
        {
            AccessToken = result.AccessToken,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpPost("registro")]
    public async Task<IActionResult> RegistroLocal([FromBody] RegistroLocalRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var agente = HttpContext.Request.Headers.UserAgent.ToString();

        var command = new RegistroLocalCommand
        {
            Correo = request.Correo,
            Contrasena = request.Contrasena,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            IpOrigen = ip,
            AgenteUsuario = agente
        };

        var result = await _mediator.Send(command);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new Application.DTOs.Auth.AuthResponse
        {
            AccessToken = result.AccessToken,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpPost("registro-google")]
    public async Task<IActionResult> RegistrarConGoogle([FromBody] RegistroGoogleRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var agente = HttpContext.Request.Headers.UserAgent.ToString();

        var command = new RegistroGoogleCommand
        {
            IdentityToken = request.IdentityToken,
            IpOrigen = ip,
            AgenteUsuario = agente
        };

        var result = await _mediator.Send(command);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new Application.DTOs.Auth.AuthResponse
        {
            AccessToken = result.AccessToken,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new ErrorResponse
            {
                Error = "Unauthorized",
                Message = "Token de refresco no encontrado."
            });

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var agente = HttpContext.Request.Headers.UserAgent.ToString();

        var command = new RefreshSessionCommand
        {
            RefreshToken = refreshToken,
            IpOrigen = ip,
            AgenteUsuario = agente
        };

        var result = await _mediator.Send(command);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new Application.DTOs.Auth.AuthResponse
        {
            AccessToken = result.AccessToken,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var command = new CerrarSesionCommand { RefreshToken = refreshToken };
            await _mediator.Send(command);
        }

        Response.Cookies.Delete("refreshToken");

        return Ok(new { mensaje = "Sesi\xF3n cerrada correctamente." });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            // En desarrollo no hay HTTPS; Secure=true impediría guardar la cookie en localhost.
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(_sesionOptions.RefreshTokenDuracion)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}