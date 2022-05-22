using AutoPay.Common.Models.Abstraction;

namespace AutoPay.Common.Models;

public class Image : IIdentifiable
{
    public Guid Id { get; set; }

    public byte[] ImageData { get; set; }
}