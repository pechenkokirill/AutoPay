using AutoPay.API.Services.Abstractions;

namespace AutoPay.API.Services;

public class JwtPrivateKeyGenerator : IJwtPrivateKeyGenerator
{
    public byte[] Generate()
    {
        var privateKey = new byte[256];
        var random = new Random();
        random.NextBytes(privateKey);
        return privateKey;
    }
}