using AutoPay.Common.DTOs.Responses;
using Refit;

namespace AutoPay.UI.Common;

public interface IRefitAccessRights
{
    [Get("/api/rights")]
    Task<IEnumerable<AccessRightResponse>> GetAccessRightsAsync();
    
    [Get("/api/rights/current")]
    Task<AccessRightResponse> GetCurrentAccessRightAsync();
    
    [Get("/api/rights/{userId}")]
    Task<IEnumerable<AccessRightResponse>> GetUserAccessRightAsync(Guid userId);
}