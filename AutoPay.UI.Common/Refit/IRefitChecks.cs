using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using Refit;

namespace AutoPay.UI.Common;

public interface IRefitChecks
{
    [Get("/api/checks")]
    Task<IEnumerable<CheckResponse>> GetChecksAsync();
    
    [Post("/api/checks/add")]
    Task<CheckResponse> AddChecksAsync(CheckRequest checkRequest);
}   