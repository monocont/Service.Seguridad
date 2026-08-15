using FluentValidation;
namespace Service.Seguridad.Application.Commands.Usuario.ActualizarDatosBasicos;

public class ActualizarDatosBasicosCommandValidator : AbstractValidator<ActualizarDatosBasicosCommand>
{
    public ActualizarDatosBasicosCommandValidator()
    {
        RuleFor(x => x.IdUsuario)
            .NotEqual(Guid.Empty).WithMessage("El ID de usuario debe ser válido.");

        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("Los nombres son obligatorios.")
            .MaximumLength(100).WithMessage("Los nombres no pueden tener más de 100 caracteres.");

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("Los apellidos son obligatorios.")
            .MaximumLength(100).WithMessage("Los apellidos no pueden tener más de 100 caracteres.");
    }
}