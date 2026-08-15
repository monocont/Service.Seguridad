using FluentValidation;
namespace Service.Seguridad.Application.Queries.Usuario.ObtenerPerfil;

public class ObtenerPerfilQueryValidator : AbstractValidator<ObtenerPerfilQuery>
{
    public ObtenerPerfilQueryValidator()
    {
        RuleFor(x => x.IdUsuario)
            .NotEqual(Guid.Empty).WithMessage("El ID de usuario debe ser válido.");
    }
}