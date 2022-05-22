using System.Collections;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AutoPay.UI.Common.Converters;

public class NumericConverter : IValueConverter
{
    public static NumericConverter Instance { get; } = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(string))
        {
            return value.ToString();
        }

        if (targetType == typeof(double))
        {
            return System.Convert.ToDouble(value);
        }

        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value is string str)
            {
                value = str.Replace('.', ',');
            }
            
            if (targetType == typeof(int))
            {
                return System.Convert.ToInt32(value);
            }
            if(targetType == typeof(float))
            {
                return System.Convert.ToSingle(value);
            }
            if(targetType == typeof(double))
            {
                return System.Convert.ToDouble(value);
            }
            if(targetType == typeof(long))
            {
                return System.Convert.ToInt64(value);
            }
            if(targetType == typeof(decimal))
            {
                return System.Convert.ToDecimal(value);
            }
        }
        catch
        {
            return BindingOperations.DoNothing;
        }
        

        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }
}