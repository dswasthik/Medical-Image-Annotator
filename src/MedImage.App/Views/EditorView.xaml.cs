using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using MedImage.App.ViewModels;

namespace MedImage.App.Views;

public partial class EditorView : UserControl
{
    private bool _drawing;
    private Point _start;

    public EditorView() => InitializeComponent();

    private EditorViewModel? Vm => DataContext as EditorViewModel;

    private void Overlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (PreviewImage.Source is null) return;
        _drawing = true;
        // Coordinates relative to the image = native pixels (image shown at Stretch=None, 96 DPI).
        _start = e.GetPosition(PreviewImage);
        TempLine.X1 = TempLine.X2 = _start.X;
        TempLine.Y1 = TempLine.Y2 = _start.Y;
        TempLine.Visibility = Visibility.Visible;
        Overlay.CaptureMouse();
    }

    private void Overlay_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_drawing) return;
        var p = e.GetPosition(PreviewImage);
        TempLine.X2 = p.X;
        TempLine.Y2 = p.Y;
    }

    private void Overlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_drawing) return;
        _drawing = false;
        Overlay.ReleaseMouseCapture();
        TempLine.Visibility = Visibility.Collapsed;

        var end = e.GetPosition(PreviewImage);
        // Ignore tiny accidental clicks.
        if (Math.Abs(end.X - _start.X) < 2 && Math.Abs(end.Y - _start.Y) < 2) return;

        Vm?.AddMeasurement(_start.X, _start.Y, end.X, end.Y);
    }
}
