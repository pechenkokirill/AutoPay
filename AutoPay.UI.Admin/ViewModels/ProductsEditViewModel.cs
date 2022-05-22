using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoPay.Common.Models;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Refit;

namespace AutoPay.UI.Admin.ViewModels;

public class ProductsEditViewModel : EditViewModelBase<ProductViewModel>
{
    private readonly IErrorMessageFactory _errorMessageFactory;
    private readonly IAutoPayApi _api;

    public ProductsEditViewModel(IErrorMessageFactory errorMessageFactory, IDialogService dialogService, IAutoPayApi api) : base(dialogService, api)
    {
        _errorMessageFactory = errorMessageFactory;
        _api = api;
    }

    protected override async Task AddItem(IAutoPayApi api)
    {
        var viewModel = new ProductAddDialogViewModel(DialogService, _errorMessageFactory, api);
        await viewModel.LoadImagesFromServerAsync();
        var result = await DialogService.ShowDialogAsync(640, 480, viewModel);

        if (result is not null)
        {
            EditableItems.Add(result);
        }
    }

    protected override async Task<IEnumerable<ProductViewModel>> PopulateDataAsync(IAutoPayApi api)
    {
        var productResponses = await api.ProductsApi.GetProductsAsync();
        List<ProductViewModel> products = new List<ProductViewModel>();

        foreach (var product in productResponses)
        {
            var viewModel = new ProductViewModel(api, product);
            _ = Task.Run(viewModel.InitializeAsync);
            products.Add(viewModel);
        }

        return products;
    }

    protected override async Task DeleteItem(ProductViewModel item, IAutoPayApi api)
    {
        try
        {
            await api.ProductsApi.DeleteProductsAsync(item.Id);

            EditableItems.Remove(item);
        }
        catch (ApiException e) when(e.HasContent && e.StatusCode == HttpStatusCode.BadRequest)
        {
            var message = _errorMessageFactory.GetMessage(e, item.Id);
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{message}\""));
        }
    }

    protected override string DeleteItemMessageProvider()
    {
        return "Вы точно хотите удалить продукт?";
    }

    protected override async Task EditItem(IAutoPayApi api)
    {
        try
        {
            var viewModel = new ProductEditDialogViewModel(SelectedData, DialogService, _errorMessageFactory, api);
            await viewModel.LoadImagesFromServerAsync();
            var result = await DialogService.ShowDialogAsync(640, 480, viewModel);
        }
        catch (ApiException e) when(e.HasContent && e.StatusCode == HttpStatusCode.BadRequest)
        {
            var message = _errorMessageFactory.GetMessage(e, SelectedData.Id);
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{message}\""));
        }
    }
}