using FluentValidation;
namespace Service.Seguridad.Application.Commands.Usuario.ActualizarContrasena;

public class ActualizarContrasenaCommandValidator : AbstractValidator<ActualizarContrasenaCommand>
{
    public ActualizarContrasenaCommandValidator()
    {
        RuleFor(x => x.IdUsuario)
            .NotEqual(Guid.Empty).WithMessage("El ID de usuario debe ser válido.");

        RuleFor(x => x.ContrasenaActual)
            .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

        RuleFor(x => x.NuevaContrasena)
            .NotEmpty().WithMessage("La nueva contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La nueva contraseña debe tener al menos 8 caracteres.")
            .Must(SerDiferenteDeActual).WithMessage("La nueva contraseña debe ser diferente de la actual.");
    }

    private bool SerDiferenteDeActual(ActualizarContrasenaCommand command, string contraseña) =>
        command.ContrasenaActual != contraseña;
}