using AutoPay.Common.Models;

namespace AutoPay.Common.DTOs.Responses;

public class AccessRightResponse
{
    public Guid Id { get; set; }
    public AccessRightType Right { get; set; }
}