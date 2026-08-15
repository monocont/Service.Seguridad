using FluentValidation;
namespace Service.Seguridad.Application.Commands.Usuario.EstablecerContrasena;

public class EstablecerContrasenaCommandValidator : AbstractValidator<EstablecerContrasenaCommand>
{
    public EstablecerContrasenaCommandValidator()
    {
        RuleFor(x => x.IdUsuario)
            .NotEqual(Guid.Empty).WithMessage("El ID de usuario debe ser válido.");

        RuleFor(x => x.NuevaContrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
    }
}