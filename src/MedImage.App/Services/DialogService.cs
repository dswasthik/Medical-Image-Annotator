using System.Windows;
using Microsoft.Win32;

namespace MedImage.App.Services;

public sealed class DialogService : IDialogService
{
    public string? OpenImage()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Open medical image",
            Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff|All files|*.*"
        };
        return dlg.ShowDialog() == true ? dlg.FileName : null;
    }

    public string? SavePng(string suggestedName)
    {
        var dlg = new SaveFileDialog
        {
            Title = "Save processed image",
            FileName = suggestedName,
            DefaultExt = ".png",
            Filter = "PNG image|*.png"
        };
        return dlg.ShowDialog() == true ? dlg.FileName : null;
    }

    public void Info(string message, string title = "MedImage") =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public void Error(string message, string title = "MedImage") =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
}
