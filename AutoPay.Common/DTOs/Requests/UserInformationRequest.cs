namespace AutoPay.Common.DTOs.Requests;

public class UserInformationRequest
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
}