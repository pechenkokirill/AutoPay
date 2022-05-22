using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoPay.Common.DTOs.Requests;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Microsoft.Toolkit.Mvvm.Input;
using Refit;

namespace AutoPay.UI.Admin.ViewModels;

public class ProductEditDialogViewModel : DialogViewModel<bool>
{
    private readonly ProductViewModel _editedProduct;
    private readonly IDialogService _dialogService;
    private readonly IErrorMessageFactory _errorMessageFactory;
    private readonly IAutoPayApi _api;
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

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

    public ProductEditDialogViewModel(ProductViewModel editedProduct,
        IDialogService dialogService,
        IErrorMessageFactory errorMessageFactory,
        IAutoPayApi api)
    {
        _editedProduct = editedProduct;
        _dialogService = dialogService;
        _errorMessageFactory = errorMessageFactory;
        _api = api;

        Product = new ProductViewModel
        {
            Id = editedProduct.Id,
            Cost = editedProduct.Cost,
            Image = editedProduct.Image,
            Name = editedProduct.Name,
            Unit = editedProduct.Unit,
            ProductId = editedProduct.ProductId
        };

        Images = new ObservableCollection<ImageViewModel>
        {
            Product.Image
        };
        CloseCommand = new RelayCommand(() => RequestClose.Invoke(false));
        SaveCommand = new AsyncRelayCommand(SaveImplementation);
    }

    public async Task LoadImagesFromServerAsync()
    {
        var images = await _api.ImagesApi.GetImagesAsync();

        foreach (var imageResponse in images)
        {
            if(_editedProduct.Image.Id == imageResponse.Id)
                continue;

            var imageViewModel = new HttpImageViewModel(_api, imageResponse.Id);
            Images.Add(imageViewModel);
            _ = Task.Run(imageViewModel.LoadImageAsync);
        }
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

            await _api.ProductsApi.EditProductsAsync(Product.Id, productRequest);

            _editedProduct.Name = Product.Name;
            _editedProduct.Cost = Product.Cost;
            _editedProduct.Image = Product.Image;
            _editedProduct.Unit = Product.Unit;
            _editedProduct.ProductId = Product.ProductId;

            RequestClose.Invoke(true);
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
}