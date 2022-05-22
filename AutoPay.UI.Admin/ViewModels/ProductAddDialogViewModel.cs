using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.Models;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Microsoft.Toolkit.Mvvm.Input;
using Refit;

namespace AutoPay.UI.Admin.ViewModels;

public class ProductAddDialogViewModel : DialogViewModel<ProductViewModel>
{
    public static IEnumerable<Unit> Options => new[]
    {
        Unit.Grams,
        Unit.Kilograms,
        Unit.Liters,
        Unit.Pieces
    };

    private readonly IDialogService _dialogService;
    private readonly IErrorMessageFactory _errorMessageFactory;
    private readonly IAutoPayApi _api;
    public ICommand CloseCommand { get;}
    public ICommand SaveCommand { get;}
    public ProductViewModel Product { get; }
    public bool IsSaveEnabled => SelectedImage is not null;

    public ImageViewModel SelectedImage
    {
        get => Product.Image;
        set
        {
            Product.Image = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsSaveEnabled));
        }
    }
    public ObservableCollection<ImageViewModel> Images { get; set; }
    public ProductAddDialogViewModel(IDialogService dialogService, IErrorMessageFactory errorMessageFactory, IAutoPayApi api)
    {
        _dialogService = dialogService;
        _errorMessageFactory = errorMessageFactory;
        _api = api;
        Product = new ProductViewModel();
        CloseCommand = new RelayCommand(() => RequestClose.Invoke(null));
        SaveCommand = new AsyncRelayCommand(SaveImplementation);
        Images = new ObservableCollection<ImageViewModel>();
    }

    private async Task SaveImplementation()
    {
        try
        {
            var productRequest = new ProductRequest
            {
                ProductId = Product.ProductId,
                Cost = Product.Cost,
                Image = Product.Image.Id,
                Name = Product.Name,
                Unit = Product.Unit
            };
            
            var response = await _api.ProductsApi.AddProductsAsync(productRequest);

            Product.Id = response.Id;
            
            RequestClose.Invoke(Product);
        }
        catch (Exception e)
        {
            var message = e.Message;


            if (e is ApiException apiException && apiException.HasContent)
            {
                message = _errorMessageFactory.GetMessage(apiException, Product.ProductId);
            }

            await _dialogService.ShowDialogAsync(500, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{message}\""));
        }
    }

    public async Task LoadImagesFromServerAsync()
    {
        var images = await _api.ImagesApi.GetImagesAsync();

        foreach (var imageResponse in images)
        {
            var imageViewModel = new HttpImageViewModel(_api, imageResponse.Id);
            Images.Add(imageViewModel);
            _ = Task.Run(imageViewModel.LoadImageAsync);
        }
    }
}