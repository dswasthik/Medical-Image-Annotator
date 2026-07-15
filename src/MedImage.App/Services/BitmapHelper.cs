using System.IO;
using System.Windows.Media.Imaging;

namespace MedImage.App.Services;

public static class BitmapHelper
{
    // Convert PNG bytes into a frozen, bindable ImageSource.
    public static BitmapImage FromPng(byte[] png)
    {
        var img = new BitmapImage();
        using var ms = new MemoryStream(png);
        img.BeginInit();
        img.CacheOption = BitmapCacheOption.OnLoad;
        img.StreamSource = ms;
        img.EndInit();
        img.Freeze();
        return img;
    }
}
