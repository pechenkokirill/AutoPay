using System.Windows.Input;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace AutoPay.UI.Common.ViewModels;

public class CheckLineProductViewModel : ObservableObject
{
    private float _count;
    public ProductViewModel Product { get; }

    public float Count
    {
        get => _count;
        set
        {
            SetProperty(ref _count, value);
            OnPropertyChanged(nameof(TotalCost));
        }
    }

    public bool CanEditCount => Product.Unit != Unit.Pieces;
    public decimal TotalCost => Product.Cost * (decimal) Count;
    public IList<CheckLineProductViewModel> Source { get; }
    public ICommand DeleteCommand { get; }

    public CheckLineProductViewModel(ProductViewModel productViewModel, IList<CheckLineProductViewModel> source)
    {
        Source = source;
        Product = productViewModel;
        Count = 1;
        DeleteCommand = new RelayCommand(DeleteAction);
    }

    public async Task InitializeProductAsync()
    {
        await Product.InitializeAsync();
    }

    private void DeleteAction()
    {
        Source.Remove(this);
    }

    public CheckLineRequest ToCheckLineRequest()
    {
        return new CheckLineRequest
        {
            Count = Count,
            ProductId = Product.Id
        };
    }
}