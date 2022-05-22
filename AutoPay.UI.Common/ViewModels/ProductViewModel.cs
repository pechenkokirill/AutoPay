using AutoPay.Common.DTOs.Responses;
using AutoPay.UI.Common.Api;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Unit = AutoPay.Common.Models.Unit;

namespace AutoPay.UI.Common.ViewModels;

public class ProductViewModel : ObservableObject
{
    private ImageViewModel _image;
    public Guid Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Cost { get; set; }
    public Unit Unit { get; set; }

    public ImageViewModel Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    public ProductViewModel(IAutoPayApi api, ProductResponse response)
    {
        Id = response.Id;
        Name = response.Name;
        Cost = response.Cost;
        ProductId = response.ProductId;
        Unit = response.Unit;
        Image = new HttpImageViewModel(api, response.ImageId);
    }

    public ProductViewModel()
    {
        
    }

    public async Task InitializeAsync()
    {
        await Image.LoadImageAsync();
    }
}