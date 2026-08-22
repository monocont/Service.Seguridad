namespace Service.Seguridad.Domain.Entities;

public class TokenRefresco : EntidadAuditoria
{
    public Guid IdToken { get; private set; }
    public Guid IdUsuario { get; private set; }
    public string Token { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaExpiracion { get; private set; }
    public bool EsRevocado { get; private set; }
    public DateTime? FechaRevocacion { get; private set; }
    public DateTime FechaInicioSesion { get; private set; }
    public string? IpOrigen { get; private set; }
    public string? AgenteUsuario { get; private set; }

    private TokenRefresco() { }

    public static TokenRefresco Crear(Guid idUsuario, string token, TimeSpan duracion, string? ip, string? agente, DateTime? inicioSesion = null)
    {
        return new TokenRefresco
        {
            IdToken = Guid.NewGuid(),
            IdUsuario = idUsuario,
            Token = token,
            FechaCreacion = DateTime.UtcNow,
            FechaExpiracion = DateTime.UtcNow.Add(duracion),
            FechaInicioSesion = inicioSesion ?? DateTime.UtcNow,
            EsRevocado = false,
            Activo = true,
            IpOrigen = ip,
            AgenteUsuario = agente
        };
    }

    /// <summary>Verdadero si la sesión continua (desde el login original) superó el límite absoluto.</summary>
    public bool ExcedioLimiteAbsoluto(TimeSpan limite) =>
        DateTime.UtcNow - FechaInicioSesion >= limite;

    public void Revocar()
    {
        EsRevocado = true;
        FechaRevocacion = DateTime.UtcNow;
    }

    public bool EstaExpirado() => DateTime.UtcNow >= FechaExpiracion;
}