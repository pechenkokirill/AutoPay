using AutoPay.Common.Models;

namespace AutoPay.API.Services.Abstractions;

public interface IImageUrlBuilder
{
    string GetUrl(Image image);
}