using FluentValidation;
namespace Service.Seguridad.Application.Commands.Auth.LoginLocal;

public class LoginLocalCommandValidator : AbstractValidator<LoginLocalCommand>
{
    public LoginLocalCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}