using AutoPay.Common.Models;

namespace AutoPay.Common.DTOs.Responses;

public class AuthenticationResponse
{
    public UserResponse User { get; set; }
    public string Token { get; set; }
}