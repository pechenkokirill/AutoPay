namespace AutoPay.Common.DTOs.Responses;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
}