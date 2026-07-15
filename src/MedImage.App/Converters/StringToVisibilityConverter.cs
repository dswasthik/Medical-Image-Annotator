using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MedImage.App.Converters;

// Visible when the string is non-empty; Collapsed otherwise.
public sealed class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type t, object p, CultureInfo c)
        => throw new NotSupportedException();
}
