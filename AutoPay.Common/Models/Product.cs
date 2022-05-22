using AutoPay.Common.Models.Abstraction;

namespace AutoPay.Common.Models;

public sealed class Product : IIdentifiable
{
    public Guid Id { get; set; }
    
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Cost { get; set; }
    public Image Image { get; set; }
    public Unit Unit { get; set; }
}