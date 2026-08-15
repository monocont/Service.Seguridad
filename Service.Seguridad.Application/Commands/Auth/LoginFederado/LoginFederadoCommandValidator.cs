using FluentValidation;
namespace Service.Seguridad.Application.Commands.Auth.LoginFederado;

public class LoginFederadoCommandValidator : AbstractValidator<LoginFederadoCommand>
{
    public LoginFederadoCommandValidator()
    {
        RuleFor(x => x.IdentityToken)
            .NotEmpty().WithMessage("El token de identidad es obligatorio.");
    }
}