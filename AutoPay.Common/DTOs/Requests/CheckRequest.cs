using AutoPay.Common.Models;

namespace AutoPay.Common.DTOs.Requests;

public class CheckRequest
{
    public Guid Issuer { get; set; }
    public decimal ActualPaymentAmount { get; set; }
    public IEnumerable<CheckLineRequest> Lines { get; set; }
}