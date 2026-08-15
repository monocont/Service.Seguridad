using FluentValidation;
namespace Service.Seguridad.Application.Commands.Usuario.ActualizarDatosBasicosAdmin;

public class ActualizarDatosBasicosAdminCommandValidator : AbstractValidator<ActualizarDatosBasicosAdminCommand>
{
    public ActualizarDatosBasicosAdminCommandValidator()
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