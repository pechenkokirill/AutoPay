using AutoPay.API.DB;
using AutoPay.API.Services.Abstractions;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.DTOs.Responses.Errors;
using AutoPay.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IImageUrlBuilder _imageUrlBuilder;
    private readonly DataContext _dataContext;

    public ProductsController(IImageUrlBuilder imageUrlBuilder, DataContext dataContext)
    {
        _imageUrlBuilder = imageUrlBuilder;
        _dataContext = dataContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
    {
        var products = await _dataContext.Products.Include(x => x.Image).ToListAsync();
        var productsResponse = products.Select(x => new ProductResponse
        {
            Id = x.Id,
            ProductId = x.ProductId,
            Cost = x.Cost,
            Name = x.Name,
            Unit = x.Unit,
            ImageId = x.Image.Id
        });

        return new JsonResult(productsResponse);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("add")]
    public async Task<ActionResult<ProductResponse>> AddProduct([FromBody] ProductRequest productRequest)
    {
        var isProductWithIdExist = await _dataContext.Products.AnyAsync(x => x.ProductId == productRequest.ProductId);
        if (isProductWithIdExist)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.ProductAlreadyExist));
        }
        
        var image = await _dataContext.Images.FirstOrDefaultAsync(x => x.Id == productRequest.Image);
        if (image is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.ImageNotFound));
        }
        
        var product = new Product
        {
            Id = Guid.NewGuid(),
            ProductId = productRequest.ProductId,
            Cost = productRequest.Cost,
            Image = image,
            Name = productRequest.Name,
            Unit = productRequest.Unit
        };

        _dataContext.Products.Add(product);
        await _dataContext.SaveChangesAsync();
        
        return new ProductResponse
        {
            Id = product.Id,
            ProductId = product.ProductId,
            Cost = product.Cost,
            Name = product.Name,
            Unit = product.Unit,
            ImageId = image.Id
        };
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveProduct(Guid productId)
    {
        var product = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (product is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.NotFound));
        }

        _dataContext.Products.Remove(product);
        await _dataContext.SaveChangesAsync();
        
        return Ok();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("edit/{productId}")]
    public async Task<IActionResult> EditProduct(Guid productId,[FromBody] ProductRequest productRequest)
    {
        var isProductWithIdExist = await _dataContext.Products.AnyAsync(x => x.ProductId == productRequest.ProductId && x.Id != productId);
        if (isProductWithIdExist)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.ProductAlreadyExist));
        }
        
        var product = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (product is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.NotFound));
        }

        var image = await _dataContext.Images.FirstOrDefaultAsync(x => x.Id == productRequest.Image);
        if (image is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.ImageNotFound));
        }

        product.Cost = productRequest.Cost;
        product.ProductId = productRequest.ProductId;
        product.Image = image;
        product.Name = productRequest.Name;
        product.Unit = productRequest.Unit;
        await _dataContext.SaveChangesAsync();
        
        return Ok();
    }
}