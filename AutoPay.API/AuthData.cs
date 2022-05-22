namespace AutoPay.API;

public static class AuthData
{
    public static string Issuer { get; } = "AutoPayApi";
    public static string Audience { get; } = "AutoPayClient";
}