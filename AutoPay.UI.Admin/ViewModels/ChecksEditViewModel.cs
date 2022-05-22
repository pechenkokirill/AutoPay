using System.Collections.Generic;
using System.Threading.Tasks;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;

namespace AutoPay.UI.Admin.ViewModels;

public class ChecksEditViewModel : EditViewModelBase<CheckViewModel>
{
    public ChecksEditViewModel(IDialogService dialogService, IAutoPayApi api) : base(dialogService, api)
    {
    }

    protected override Task AddItem(IAutoPayApi api)
    {
        throw new System.NotImplementedException();
    }

    protected override async Task<IEnumerable<CheckViewModel>> PopulateDataAsync(IAutoPayApi api)
    {
        var checkResponses = await api.ChecksApi.GetChecksAsync();
        List<CheckViewModel> checks = new List<CheckViewModel>();
        foreach (var check in checkResponses)
        {
            var viewModel = new CheckViewModel(check, api);
            _ = Task.Run(viewModel.InitializeAsync);
            checks.Add(viewModel);
        }

        return checks;
    }

    protected override Task DeleteItem(CheckViewModel item, IAutoPayApi api)
    {
        throw new System.NotImplementedException();
    }

    protected override string DeleteItemMessageProvider()
    {
        throw new System.NotImplementedException();
    }

    protected override Task EditItem(IAutoPayApi autoPayApi)
    {
        throw new System.NotImplementedException();
    }
}