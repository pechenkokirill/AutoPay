namespace AutoPay.API.Services.Abstractions;

public interface IPasswordHashService
{
    string CalculateHash(string password);
    bool ValidateHash(string hashedPassword, string password);
}