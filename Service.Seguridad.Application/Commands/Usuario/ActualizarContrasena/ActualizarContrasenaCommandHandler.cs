using MediatR;
using Service.Seguridad.Application.DTOs.Auth;
using Service.Seguridad.Domain.Interfaces;
namespace Service.Seguridad.Application.Commands.Usuario.ActualizarContrasena;

public class ActualizarContrasenaCommandHandler : IRequestHandler<ActualizarContrasenaCommand, Unit>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ActualizarContrasenaCommandHandler(IUsuarioRepository usuarioRepository)
        => _usuarioRepository = usuarioRepository;

    public async Task<Unit> Handle(ActualizarContrasenaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.IdUsuario);

        if (usuario is null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        if (usuario.ContrasenaHash is null)
            throw new UnauthorizedAccessException("El usuario no tiene contraseÃ±a configurada.");

        if (!BCrypt.Net.BCrypt.Verify(request.ContrasenaActual, usuario.ContrasenaHash))
            throw new UnauthorizedAccessException("La contraseÃ±a actual es incorrecta.");

        if (!ValidarContrasenaFuerte(request.NuevaContrasena))
            throw new ArgumentException("La nueva contraseÃ±a debe tener al menos 8 caracteres, una mayÃºscula, una minÃºscula, un nÃºmero y un carÃ¡cter especial.");

        var contrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.NuevaContrasena);
        usuario.ActualizarContrasena(contrasenaHash);

        await _usuarioRepository.ActualizarAsync(usuario);

        return Unit.Value;
    }

    private static bool ValidarContrasenaFuerte(string contrasena)
    {
        if (contrasena is null || contrasena.Length < 8)
            return false;

        if (!contrasena.Any(char.IsUpper))
            return false;

        if (!contrasena.Any(char.IsLower))
            return false;

        if (!contrasena.Any(char.IsDigit))
            return false;

        const string caracteresEspeciales = "!@#$%^&*()_+-=[]{}|;:',.<>?/`~";
        if (!contrasena.Any(c => caracteresEspeciales.Contains(c)))
            return false;

        return true;
    }
}

