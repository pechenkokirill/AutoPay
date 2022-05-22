using System.Net.Http;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using Refit;

namespace AutoPay.UI.Common;

public interface IRefitImages
{
    [Get("/api/images/{imageId}")]
    Task<HttpResponseMessage> GetImageAsync(Guid imageId);
    
    [Get("/api/images")]
    Task<IEnumerable<ImageResponse>> GetImagesAsync();
    
    [Post("/api/images")]
    [Headers("Content-Type: application/json; image/png")]
    Task<ImageResponse> AddImageAsync(ImageRequest imageRequest);
    
    [Delete("/api/images/{imageId}")]
    Task DeleteImageAsync(Guid imageId);
}