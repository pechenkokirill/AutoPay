using System.IO;
using Avalonia.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Common.ViewModels;

public abstract class ImageViewModel : ObservableObject
{
    public Guid Id { get; set; }
    public bool IsLoading => Bitmap is null;
    public Bitmap Bitmap { get; set; }
    protected abstract Task<Stream> LoadImplementationAsync();
    public async Task LoadImageAsync()
    {
        try
        {
            var stream = await LoadImplementationAsync();
            Bitmap = new Bitmap(stream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            OnPropertyChanged(nameof(Bitmap));
            OnPropertyChanged(nameof(IsLoading));
        }
    }
}