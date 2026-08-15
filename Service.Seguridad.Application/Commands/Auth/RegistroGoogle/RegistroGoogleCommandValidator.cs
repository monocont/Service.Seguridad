using FluentValidation;
namespace Service.Seguridad.Application.Commands.Auth.RegistroGoogle;

public class RegistroGoogleCommandValidator : AbstractValidator<RegistroGoogleCommand>
{
    public RegistroGoogleCommandValidator()
    {
        RuleFor(x => x.IdentityToken)
            .NotEmpty().WithMessage("El token de identidad es obligatorio.");
    }
}