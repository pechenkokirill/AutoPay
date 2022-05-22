using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Refit;

namespace AutoPay.UI.Admin.ViewModels;

public abstract class EditViewModelBase<T> : ObservableObject
{
    private readonly IAutoPayApi _api;
    private T _selectedData;
    protected IDialogService DialogService { get; }
    public ObservableCollection<T> EditableItems { get; set; }

    public T SelectedData
    {
        get => _selectedData;
        set => SetProperty(ref _selectedData, value);
    }

    public ICommand DeleteCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand AddCommand { get; }
    public bool IsDataLoading => EditableItems is null;

    public EditViewModelBase(IDialogService dialogService, IAutoPayApi api)
    {
        DialogService = dialogService;
        _api = api;
        DeleteCommand = new AsyncRelayCommand(DeleteImplementation);
        AddCommand = new AsyncRelayCommand(AddImplementation);
        EditCommand = new AsyncRelayCommand(EditImplementation);
    }

    protected abstract Task AddItem(IAutoPayApi api);
    protected abstract Task<IEnumerable<T>> PopulateDataAsync(IAutoPayApi api);
    protected abstract Task DeleteItem(T item, IAutoPayApi api);
    protected abstract string DeleteItemMessageProvider();

    private async Task AddImplementation()
    {
        try
        {
            await AddItem(_api);
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.Forbidden)
        {
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"У данного пользователя нет прав доступа!"));
        }
        catch (Exception e)
        {
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{e.Message}\""));
        }
    }

    protected abstract Task EditItem(IAutoPayApi autoPayApi);

    private async Task EditImplementation()
    {
        try
        {
            await EditItem(_api);
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.Forbidden)
        {
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"У данного пользователя нет прав доступа!"));
        }
        catch (Exception e)
        {
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{e.Message}\""));
        }
    }

    private async Task DeleteImplementation()
    {
        try
        {
            var result = await DialogService.ShowDialogAsync(400, 250, new YesNoViewModel(DeleteItemMessageProvider()));

            if (result)
            {
                await DeleteItem(SelectedData, _api);
            }
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.Forbidden)
        {
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"У данного пользователя нет прав доступа!"));
        }
        catch (Exception e)
        {
            await DialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{e.Message}\""));
        }
    }

    public async Task LoadAsync()
    {
        try
        {
            EditableItems = new ObservableCollection<T>(await PopulateDataAsync(_api));
        }
        catch
        {
            EditableItems = new ObservableCollection<T>();
        }
        finally
        {
            OnPropertyChanged(nameof(EditableItems));
            OnPropertyChanged(nameof(IsDataLoading));
        }
    }
}