using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace AutoPay.UI.Admin.Converters;

public class UrlToImageConverter : IValueConverter
{
    public static UrlToImageConverter Instance { get; } = new UrlToImageConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string url)
        {
            var client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = client.GetAsync(url).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }

            return new Bitmap(response.Content.ReadAsStream());
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}