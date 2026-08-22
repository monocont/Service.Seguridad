namespace Service.Seguridad.Application.Common;

/// <summary>
/// Política de sesión configurable desde appsettings (sección "Sesion").
/// </summary>
public class SesionOptions
{
    public const string SectionName = "Sesion";

    /// <summary>Vida del refresh token (ventana de inactividad deslizante), en horas.</summary>
    public int RefreshTokenHoras { get; set; } = 2;

    /// <summary>Vida máxima de una sesión continua sin importar la actividad, en horas.</summary>
    public int LimiteAbsolutoHoras { get; set; } = 12;

    public TimeSpan RefreshTokenDuracion => TimeSpan.FromHours(RefreshTokenHoras);

    public TimeSpan LimiteAbsolutoDuracion => TimeSpan.FromHours(LimiteAbsolutoHoras);
}
