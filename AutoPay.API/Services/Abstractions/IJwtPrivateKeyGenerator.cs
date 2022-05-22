namespace AutoPay.API.Services.Abstractions;

public interface IJwtPrivateKeyGenerator
{
    byte[] Generate();
}