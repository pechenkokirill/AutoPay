using System.Collections;
using System.Globalization;
using AutoPay.Common.Models;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AutoPay.UI.Common.Converters;

public class UnitConverter : IValueConverter
{
    public static UnitConverter Instance { get; } = new();

    private readonly Dictionary<Unit, string> _from;

    private readonly Dictionary<string, Unit> _to;

    public UnitConverter()
    {
        _from = new Dictionary<Unit, string>
        {
            {Unit.Grams, "гр."},
            {Unit.Kilograms, "кг."},
            {Unit.Liters, "лт."},
            {Unit.Pieces, "шт."},
        };
        
        _to = _from.ToDictionary(x => x.Value, x => x.Key);
    }
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(string) || targetType == typeof(object))
        {
            if (value is Unit unit)
            {
                if (!_from.ContainsKey(unit))
                {
                    return "-";
                }
            
                return _from[unit];
            }
        }

        if (targetType == typeof(IEnumerable) && value is IEnumerable<Unit> enumerable)
        {
            return enumerable.Select(x => _from.ContainsKey(x) ? _from[x] : "-");
        }

        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(Unit) && value is string str)
        {
            if (!_to.ContainsKey(str))
            {
                return default(Unit);
            }
            
            return _to[str];
        }

        if (targetType == typeof(IEnumerable<Unit>) && value is IEnumerable enumerable)
        {
            return enumerable.OfType<string>().Select(x => _to.ContainsKey(x) ? _to[x] : default);
        }

        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }
}