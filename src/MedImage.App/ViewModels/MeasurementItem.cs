using CommunityToolkit.Mvvm.ComponentModel;

namespace MedImage.App.ViewModels;

// One on-image measurement line (coordinates are in native image pixels).
public partial class MeasurementItem : ObservableObject
{
    [ObservableProperty] private double _x1;
    [ObservableProperty] private double _y1;
    [ObservableProperty] private double _x2;
    [ObservableProperty] private double _y2;

    public double LengthPixels => Math.Sqrt(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2));
    public string Label => $"{LengthPixels:0.0} px";
}
