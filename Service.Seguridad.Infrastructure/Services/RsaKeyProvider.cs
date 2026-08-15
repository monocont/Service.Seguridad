using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Service.Seguridad.Infrastructure.Services;

public class RsaKeyProvider
{
    private readonly RSA _rsa;
    private readonly RsaSecurityKey _key;
    private readonly string _keyFilePath = Path.Combine(AppContext.BaseDirectory, "rsa_key.json");

    public RsaKeyProvider()
    {
        _rsa = RSA.Create();
        
        if (File.Exists(_keyFilePath))
        {
            try
            {
                var keyJson = File.ReadAllText(_keyFilePath);
                var keyParams = JsonSerializer.Deserialize<RSAParameters>(keyJson);
                _rsa.ImportParameters(keyParams);
            }
            catch
            {
                GenerateAndSaveKey();
            }
        }
        else
        {
            GenerateAndSaveKey();
        }

        _key = new RsaSecurityKey(_rsa) { KeyId = "monocont-key-1" };
    }

    private void GenerateAndSaveKey()
    {
        _rsa.KeySize = 2048;
        var keyParams = _rsa.ExportParameters(true);
        var keyJson = JsonSerializer.Serialize(keyParams);
        File.WriteAllText(_keyFilePath, keyJson);
    }

    public RsaSecurityKey GetKey() => _key;

    public JsonWebKey GetJsonWebKey()
    {
        var parameters = _rsa.ExportParameters(false);
        var jwk = new JsonWebKey
        {
            Kty = JsonWebAlgorithmsKeyTypes.RSA,
            Use = "sig",
            Kid = "monocont-key-1",
            Alg = SecurityAlgorithms.RsaSha256,
            N = Base64UrlEncoder.Encode(parameters.Modulus),
            E = Base64UrlEncoder.Encode(parameters.Exponent)
        };
        return jwk;
    }
}
