using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.Models;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Avalonia.Data.Converters;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace AutoPay.UI.Client.ViewModels;

public class MainViewModel : ObservableObject
{
    private ObservableCollection<ProductViewModel> _allProducts;
    private readonly IAutoPayApi _api;
    private readonly ICheckViewBuilder _checkViewBuilder;
    private readonly IDialogService _dialogService;
    private readonly UserResponse _user;
    private ProductViewModel _selectedProduct;
    private decimal _totalPayment;
    private decimal _actualPayment;
    public UserViewModel CurrentUser { get; set; }

    public ObservableCollection<ProductViewModel> AllProducts
    {
        get => _allProducts;
        set
        {
            SetProperty(ref _allProducts, value);
            OnPropertyChanged(nameof(IsProductsLoading));
        }
    }

    public bool IsProductsLoading => AllProducts is null;

    public ProductViewModel SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(AddCommandCanExecute));
        }
    }

    public decimal ActualPayment
    {
        get => _actualPayment;
        set => SetProperty(ref _actualPayment, value);
    }

    public bool AddCommandCanExecute => SelectedProduct is not null;

    public ObservableCollection<CheckLineProductViewModel> CheckProducts { get; set; }

    public ICommand AddProductCommand { get; set; }
    public ICommand ReloadProductsCommand { get; set; }
    public ICommand IssueCheckCommand { get; set; }

    public decimal TotalPayment
    {
        get => _totalPayment;
        set => SetProperty(ref _totalPayment, value);
    }

    public MainViewModel(IAutoPayApi api, ICheckViewBuilder checkViewBuilder, IDialogService dialogService, UserResponse user)
    {
        _api = api;
        _checkViewBuilder = checkViewBuilder;
        _dialogService = dialogService;
        _user = user;
        AddProductCommand = new AsyncRelayCommand(AddProductAsync);
        CurrentUser = new UserViewModel(user);
        ReloadProductsCommand = new AsyncRelayCommand(ReloadAsync);
        IssueCheckCommand = new AsyncRelayCommand(IssueCheckAsync);
        CheckProducts = new ObservableCollection<CheckLineProductViewModel>();
        CheckProducts.CollectionChanged += (_, _) => RecalculateTotalPayment();
    }

    private async Task IssueCheckAsync()
    {
        var result = await _dialogService.ShowDialogAsync(500,300,new YesNoViewModel("Вы точно хотите завершить операцию и выдать чек?"));

        if (result)
        {
            var checkRequest = new CheckRequest
            {
                Issuer = _user.Id,
                Lines = CheckProducts.Select(x => x.ToCheckLineRequest()).ToList(),
                ActualPaymentAmount = ActualPayment
            };

            try
            {
                var response = await _api.ChecksApi.AddChecksAsync(checkRequest);

                await _dialogService.ShowDialogAsync(640, 480, new CheckDialogViewModel(_checkViewBuilder, response));
            }
            catch (Exception e)
            {
                await _dialogService.ShowDialogAsync(500, 300,
                    new MessageDialogViewModel($"Ой произошла ошибка: {e.Message}"));
            }
            finally
            {
                CheckProducts.Clear();
            }
        }
    }

    private async Task AddProductAsync()
    {
        var viewModel = new CheckLineProductViewModel(SelectedProduct, CheckProducts);
        viewModel.PropertyChanged += (_, _) => RecalculateTotalPayment();
        CheckProducts.Add(viewModel);
        await viewModel.InitializeProductAsync();
    }

    private void RecalculateTotalPayment()
    {
        TotalPayment = CheckProducts.Sum(x => (decimal) x.Count * x.Product.Cost);
        ActualPayment = Math.Max(ActualPayment, TotalPayment);
    }

    private async Task ReloadAsync()
    {
        AllProducts = null;
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        try
        {
            var products = await _api.ProductsApi.GetProductsAsync();
            await Task.Delay(200);

            var productViewModels = products.Select(productResponse => new ProductViewModel(_api, productResponse));

            AllProducts = new ObservableCollection<ProductViewModel>(productViewModels);
        }
        catch
        {
            AllProducts = new ObservableCollection<ProductViewModel>();
        }
        finally
        {
            OnPropertyChanged(nameof(AllProducts));
            OnPropertyChanged(nameof(IsProductsLoading));
        }
    }
}