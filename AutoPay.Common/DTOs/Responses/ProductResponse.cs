using System.Text.Json.Serialization;
using AutoPay.Common.Models;

namespace AutoPay.Common.DTOs.Responses;

public class ProductResponse
{
    public Guid Id { get; set; }
    public int ProductId { get; set; }
    public Guid ImageId { get; set; }
    public string Name { get; set; }
    public decimal Cost { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]  
    public Unit Unit { get; set; }
}