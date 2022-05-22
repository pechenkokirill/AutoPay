using System.IO;

namespace AutoPay.UI.Common.ViewModels;

public class MemoryImageViewModel : ImageViewModel
{
    private readonly byte[] _imageBlob;

    public MemoryImageViewModel(byte[] imageBlob)
    {
        _imageBlob = imageBlob;
    }
    protected override async Task<Stream> LoadImplementationAsync()
    {
        await Task.CompletedTask;
        return new MemoryStream(_imageBlob);
    }
}