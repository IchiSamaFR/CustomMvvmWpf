using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace CustomMvvmWpf.UI.Converters;

/// <summary>
/// Convertit une collection en booléen : true si elle contient au moins un élément.
/// </summary>
public class IsAnyListToBooleanConverter : IValueConverter
{
    public virtual object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var invert = (parameter as string)?.Equals("invert", StringComparison.OrdinalIgnoreCase) == true;
        var toReturn = false;

        if (value is ICollection collection)
            toReturn = collection.Count > 0;
        else if (value is IEnumerable enumerable)
            toReturn = enumerable.GetEnumerator().MoveNext();
        else if (value is IDictionary dictionary)
            toReturn = dictionary.GetEnumerator().MoveNext();

        return invert ? !toReturn : toReturn;
    }

    public virtual object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(IsAnyListToBooleanConverter)} ne supporte pas la conversion inverse.");
    }
}