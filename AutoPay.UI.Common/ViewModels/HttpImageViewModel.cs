using System.IO;
using AutoPay.Common.DTOs.Responses;
using AutoPay.UI.Common.Api;

namespace AutoPay.UI.Common.ViewModels;

public class HttpImageViewModel : ImageViewModel
{
    private readonly IAutoPayApi _api;

    public HttpImageViewModel(IAutoPayApi api, Guid imageId)
    {
        Id = imageId;
        _api = api;
    }
    protected override async Task<Stream> LoadImplementationAsync()
    {
        var responseMessage = await _api.ImagesApi.GetImageAsync(Id);
        var buffer = await responseMessage.Content.ReadAsByteArrayAsync();

        return new MemoryStream(buffer);
    }
}