using System.Text.Json.Serialization;
using AutoPay.Common.Models;

namespace AutoPay.Common.DTOs.Requests;

public class ProductRequest
{
    public int ProductId { get; set; }
    public Guid Image { get; set; }
    public string Name { get; set; }
    public decimal Cost { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]  
    public Unit Unit { get; set; }
}