using FluentValidation;
namespace Service.Seguridad.Application.Commands.Auth.CerrarSesion;

public class CerrarSesionCommandValidator : AbstractValidator<CerrarSesionCommand>
{
    public CerrarSesionCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("El token de refresco es obligatorio.");
    }
}