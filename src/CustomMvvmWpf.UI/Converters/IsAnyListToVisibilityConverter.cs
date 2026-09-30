using System.Globalization;

namespace CustomMvvmWpf.UI.Converters;

public class IsAnyListToVisibilityConverter : IsAnyListToBooleanConverter
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var invert = (parameter as string)?.Equals("invert", StringComparison.OrdinalIgnoreCase) == true;
        var toReturn = (bool)base.Convert(value, targetType, parameter, culture);
        toReturn = invert ? !toReturn : toReturn;

        return toReturn ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(IsAnyListToVisibilityConverter)} ne supporte pas la conversion inverse.");
    }
}
