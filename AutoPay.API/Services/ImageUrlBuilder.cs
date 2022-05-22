using AutoPay.API.Services.Abstractions;
using AutoPay.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AutoPay.API.Services;

public class ImageUrlBuilder : IImageUrlBuilder
{
    private readonly string _url;
    private readonly IActionContextAccessor _actionContextAccessor;

    public ImageUrlBuilder(string url, IActionContextAccessor actionContextAccessor)
    {
        _url = url;
        _actionContextAccessor = actionContextAccessor;
    }
    
    public string GetUrl(Image image)
    {
        return GenerateUrl(_actionContextAccessor, image.Id);
    }

    private string GenerateUrl(IActionContextAccessor actionContextAccessor, Guid imageId)
    {
        if (_actionContextAccessor.ActionContext is null)
        {
            throw new NullReferenceException();
        }
        
        var context = _actionContextAccessor.ActionContext.HttpContext;
        var scheme = context.Request.Scheme;
        var host = context.Request.Host.Value;

        return scheme + "://" + host + _url + $"/{imageId}";
    }
}