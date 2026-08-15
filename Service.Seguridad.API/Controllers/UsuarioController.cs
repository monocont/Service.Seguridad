using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Seguridad.API.DTOs;
using Service.Seguridad.Application.Commands.Usuario.ActualizarContrasena;
using Service.Seguridad.Application.Commands.Usuario.ActualizarContrasenaAdmin;
using Service.Seguridad.Application.Commands.Usuario.ActualizarDatosBasicos;
using Service.Seguridad.Application.Commands.Usuario.ActualizarDatosBasicosAdmin;
using Service.Seguridad.Application.Commands.Usuario.EstablecerContrasena;
using Service.Seguridad.Application.DTOs.Usuario;
using Service.Seguridad.Application.Queries.Usuario.ObtenerPerfil;
using Service.Seguridad.Application.Queries.Usuario.ListarUsuarios;

namespace Service.Seguridad.API.Controllers;

[ApiController]
[Route("api/v1/seguridad/usuario")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuarioController(IMediator mediator) => _mediator = mediator;

    [HttpGet("me")]
    public async Task<IActionResult> ObtenerPerfil()
    {
        var idUsuarioStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(idUsuarioStr) || !Guid.TryParse(idUsuarioStr, out var idUsuario))
            return Unauthorized(new ErrorResponse
            {
                Error = "Unauthorized",
                Message = "No se pudo identificar al usuario."
            });

        var query = new ObtenerPerfilQuery { IdUsuario = idUsuario };
        var perfil = await _mediator.Send(query);

        return Ok(perfil);
    }

    [HttpPut("datos-basicos")]
    public async Task<IActionResult> ActualizarDatosBasicos([FromBody] ActualizarDatosBasicosRequest request)
    {
        var idUsuarioStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(idUsuarioStr) || !Guid.TryParse(idUsuarioStr, out var idUsuario))
            return Unauthorized(new ErrorResponse
            {
                Error = "Unauthorized",
                Message = "No se pudo identificar al usuario."
            });

        var command = new ActualizarDatosBasicosCommand
        {
            IdUsuario = idUsuario,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos
        };

        await _mediator.Send(command);

        return Ok(new { mensaje = "Datos básicos actualizados correctamente." });
    }

    [HttpPut("contrasena")]
    public async Task<IActionResult> ActualizarContrasena([FromBody] ActualizarContrasenaRequest request)
    {
        var idUsuarioStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(idUsuarioStr) || !Guid.TryParse(idUsuarioStr, out var idUsuario))
            return Unauthorized(new ErrorResponse
            {
                Error = "Unauthorized",
                Message = "No se pudo identificar al usuario."
            });

        var command = new ActualizarContrasenaCommand
        {
            IdUsuario = idUsuario,
            ContrasenaActual = request.ContrasenaActual,
            NuevaContrasena = request.NuevaContrasena
        };

        await _mediator.Send(command);

        return Ok(new { mensaje = "Contraseña actualizada correctamente." });
    }

    [HttpPost("establecer-contrasena")]
    public async Task<IActionResult> EstablecerContrasena([FromBody] EstablecerContrasenaRequest request)
    {
        var idUsuarioStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(idUsuarioStr) || !Guid.TryParse(idUsuarioStr, out var idUsuario))
            return Unauthorized(new ErrorResponse
            {
                Error = "Unauthorized",
                Message = "No se pudo identificar al usuario."
            });

        var command = new EstablecerContrasenaCommand
        {
            IdUsuario = idUsuario,
            NuevaContrasena = request.NuevaContrasena
        };

        await _mediator.Send(command);

        return Ok(new { mensaje = "Contraseña establecida correctamente." });
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("admin/datos-basicos")]
    public async Task<IActionResult> ActualizarDatosBasicosAdmin([FromBody] ActualizarDatosBasicosAdminRequest request)
    {
        var command = new ActualizarDatosBasicosAdminCommand
        {
            IdUsuario = request.IdUsuario,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos
        };

        await _mediator.Send(command);

        return Ok(new { mensaje = "Datos básicos actualizados correctamente." });
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("admin/contrasena")]
    public async Task<IActionResult> ActualizarContrasenaAdmin([FromBody] ActualizarContrasenaAdminRequest request)
    {
        var command = new ActualizarContrasenaAdminCommand
        {
            IdUsuario = request.IdUsuario,
            NuevaContrasena = request.NuevaContrasena
        };

        await _mediator.Send(command);

        return Ok(new { mensaje = "Contraseña actualizada correctamente." });
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("lista")]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await _mediator.Send(new ListarUsuariosQuery());

        return Ok(usuarios);
    }
}