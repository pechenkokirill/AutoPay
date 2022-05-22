using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using Refit;

namespace AutoPay.UI.Common;

public interface IRefitProducts
{
    [Get("/api/products")]
    Task<IEnumerable<ProductResponse>> GetProductsAsync();
    
    [Post("/api/products/add")]
    Task<ProductResponse> AddProductsAsync(ProductRequest productRequest);
    
    [Delete("/api/products/remove/{productId}")]
    Task DeleteProductsAsync(Guid productId);
    
    [Post("/api/products/edit/{productId}")]
    Task EditProductsAsync(Guid productId, ProductRequest productRequest);
}