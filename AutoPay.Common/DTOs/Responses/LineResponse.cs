using AutoPay.Common.Models;

namespace AutoPay.Common.DTOs.Responses;

public class LineResponse
{
    public Guid Id { get; set; }
    public ProductResponse Product { get; set; }
    public float Count { get; set; }
}