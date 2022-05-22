namespace AutoPay.Common.DTOs.Requests;

public class CheckLineRequest
{
    public float Count { get; set; }
    public Guid ProductId { get; set; }
}