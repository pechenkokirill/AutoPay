using AutoPay.Common.Models;

namespace AutoPay.Common.DTOs.Requests;

public class RegistrationRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public bool IsAdmin { get; set; }
}