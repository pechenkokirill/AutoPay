using AutoPay.Common.Models.Abstraction;

namespace AutoPay.Common.Models;

public class CheckLine : IIdentifiable
{
    public Guid Id { get; set; }
    public Product Product { get; set; }
    public float Count { get; set; }
}