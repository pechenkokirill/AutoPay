using AutoPay.Common.Models.Abstraction;

namespace AutoPay.Common.Models;

public sealed class SettlementCheck : IIdentifiable
{
    public Guid Id { get; set; }
    public User Issuer { get; set; }
    public DateTime IssueDate { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal ActualPaymentAmount { get; set; }
    
    public IEnumerable<CheckLine> Lines { get; set; }
}