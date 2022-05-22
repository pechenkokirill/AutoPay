namespace AutoPay.Common.DTOs.Responses;

public class CheckResponse
{
    public Guid Id { get; set; }
    public UserResponse Issuer { get; set; }
    public DateTime IssueDate { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal ActualPaymentAmount { get; set; }
    
    public IList<LineResponse> Lines { get; set; }
}