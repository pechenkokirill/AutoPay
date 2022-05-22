using AutoPay.API.DB;
using AutoPay.API.Services.Abstractions;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.DTOs.Responses.Errors;
using AutoPay.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace AutoPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private readonly IImageUrlBuilder _imageUrlBuilder;
    private readonly DataContext _dataContext;

    public ImagesController(IImageUrlBuilder imageUrlBuilder, DataContext dataContext)
    {
        _imageUrlBuilder = imageUrlBuilder;
        _dataContext = dataContext;
    }

    [HttpGet("{imageId}")]
    public async Task<IActionResult> GetImage(Guid imageId)
    {
        var image = await _dataContext.Images.FirstOrDefaultAsync(x => x.Id == imageId);
        if (image is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.ImageNotFound));
        }

        return new FileContentResult(image.ImageData, "image/png");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ImageResponse>>> GetImages()
    {
        var images = await _dataContext.Images.ToListAsync();
        var imagesResponse = images.Select(image => new ImageResponse
        {
            Id = image.Id,
            Url = _imageUrlBuilder.GetUrl(image)
        });

        return new JsonResult(imagesResponse);
    }
    
    [HttpPost]
    public async Task<ActionResult<ImageResponse>> AddImage([FromBody] ImageRequest imageRequest)
    {
        if (Request.ContentType is null || !Request.ContentType.Contains("image/png"))
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.ImageBadFormat));
        }

        var newImage = new Image
        {
            Id = Guid.NewGuid(),
            ImageData = imageRequest.EncodedPng
        };
        
        _dataContext.Images.Add(newImage);
        await _dataContext.SaveChangesAsync();

        return new ImageResponse
        {
            Id = newImage.Id,
            Url = _imageUrlBuilder.GetUrl(newImage)
        };
    }
    
    [HttpDelete("{imageId}")]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var image = await _dataContext.Images.FirstOrDefaultAsync(x => x.Id == imageId);
        if (image is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.ImageNotFound));
        }

        _dataContext.Images.Remove(image);
        await  _dataContext.SaveChangesAsync();
        
        return Ok();
    }
}