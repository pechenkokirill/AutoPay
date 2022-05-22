using System.Security.Cryptography;
using System.Text;
using AutoPay.API.Services.Abstractions;

namespace AutoPay.API.Services;

public class ShaPasswordHashService : IPasswordHashService
{
    public string CalculateHash(string password)
    {
        using SHA256 sha256Hash = SHA256.Create();
        var computeHash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

        return Encoding.UTF8.GetString(computeHash);
    }

    public bool ValidateHash(string hashedPassword, string password)
    {
        return CalculateHash(password) == hashedPassword;
    }
}