using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Refit;

namespace AutoPay.UI.Admin.ViewModels;

public class UsersEditViewModel : EditViewModelBase<UserViewModel>
{
    private readonly IErrorMessageFactory _errorMessageFactory;

    public UsersEditViewModel(IErrorMessageFactory errorMessageFactory, IDialogService dialogService, IAutoPayApi api) :
        base(dialogService, api)
    {
        _errorMessageFactory = errorMessageFactory;
    }

    protected override async Task AddItem(IAutoPayApi api)
    {
        var viewModel = new RegisterUserDialogViewModel(_errorMessageFactory, DialogService, api);
        var result = await DialogService.ShowDialogAsync(640, 480, viewModel);

        if (result is not null)
        {
            EditableItems.Add(result);
        }
    }

    protected override async Task<IEnumerable<UserViewModel>> PopulateDataAsync(IAutoPayApi api)
    {
        var productResponses = await api.UsersApi.GetUsersAsync();
        List<UserViewModel> products = new List<UserViewModel>();

        foreach (var product in productResponses)
        {
            var rights = await api.AccessRightsApi.GetUserAccessRightAsync(product.Id);
            var rightsViewModels = rights.Select(x => new AccessRightViewModel(x));
            var viewModel = new UserViewModel(product, rightsViewModels);
            products.Add(viewModel);
        }

        return products;
    }

    protected override async Task DeleteItem(UserViewModel item, IAutoPayApi api)
    {
        try
        {
            await api.UsersApi.DeleteUserAsync(item.Id);

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
        return "Вы точно хотите удалить пользователя?";
    }

    protected override async Task EditItem(IAutoPayApi api)
    {
        try
        {
            var viewModel = new UserEditDialogViewModel(SelectedData, _errorMessageFactory, DialogService, api);
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