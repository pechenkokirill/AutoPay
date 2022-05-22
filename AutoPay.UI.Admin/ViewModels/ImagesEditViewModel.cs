using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Avalonia.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Refit;

namespace AutoPay.UI.Admin.ViewModels;

public class ImagesEditViewModel : EditViewModelBase<ImageViewModel>
{
    private readonly IErrorMessageFactory _errorMessageFactory;

    public ImagesEditViewModel(IErrorMessageFactory errorMessageFactory, IDialogService dialogService, IAutoPayApi api)
        : base(dialogService, api)
    {
        _errorMessageFactory = errorMessageFactory;
    }

    protected override async Task AddItem(IAutoPayApi api)
    {
        var filePath = await DialogService.OpenFileDialogAsync(new List<FileDialogFilter>()
        {
            new()
            {
                Name = "Image file (.png)",
                Extensions = new List<string>
                {
                    "png"
                }
            }
        });

        if (filePath is null)
        {
            return;
        }

        byte[] imageBuffer;
        try
        {
            imageBuffer = await File.ReadAllBytesAsync(filePath);
        }
        catch (Exception e)
        {
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{e.Message}\""));
            return;
        }

        var imageRequest = new ImageRequest
        {
            EncodedPng = imageBuffer
        };


        var imageResponse = await api.ImagesApi.AddImageAsync(imageRequest);

        var imageViewModel = new HttpImageViewModel(api, imageResponse.Id);

        _ = Task.Run(imageViewModel.LoadImageAsync);

        EditableItems.Add(imageViewModel);
    }

    protected override async Task<IEnumerable<ImageViewModel>> PopulateDataAsync(IAutoPayApi api)
    {
        var images = await api.ImagesApi.GetImagesAsync();
        List<ImageViewModel> imageViewModels = new List<ImageViewModel>();
        foreach (var image in images)
        {
            var imageViewModel = new HttpImageViewModel(api, image.Id);
            imageViewModels.Add(imageViewModel);
            _ = Task.Run(imageViewModel.LoadImageAsync);
        }

        return imageViewModels;
    }

    protected override async Task DeleteItem(ImageViewModel item, IAutoPayApi api)
    {
        try
        {
            await api.ImagesApi.DeleteImageAsync(item.Id);

            EditableItems.Remove(item);
        }
        catch (ApiException e) when (e.HasContent && e.StatusCode == HttpStatusCode.BadRequest)
        {
            var message = _errorMessageFactory.GetMessage(e, item.Id);
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{message}\""));
        }
    }

    protected override string DeleteItemMessageProvider()
    {
        return "Вы точно хотите удалить картинку?";
    }

    protected override Task EditItem(IAutoPayApi autoPayApi)
    {
        return Task.CompletedTask;
    }
}