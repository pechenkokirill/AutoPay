namespace AutoPay.Common.DTOs.Requests;

public class LineRequest
{
    public Guid ProductId { get; set; }
    public float Count { get; set; }
}