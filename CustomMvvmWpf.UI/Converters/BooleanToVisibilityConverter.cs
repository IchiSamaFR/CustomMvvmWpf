using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomMvvmWpf.UI.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var invert = (parameter as string)?.Equals("invert", StringComparison.OrdinalIgnoreCase) == true;
        var toReturn = false;

        if (value is bool boolValue)
        {
            toReturn = boolValue;
        }
        toReturn = invert ? !toReturn : toReturn;
        return toReturn ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}
