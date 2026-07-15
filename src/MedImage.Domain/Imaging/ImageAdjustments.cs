namespace MedImage.Domain.Imaging;

// A plain value object describing the "look" applied to an image.
// Kept platform-neutral so Domain has no dependency on OpenCV or WPF.
public class ImageAdjustments
{
    public bool Grayscale { get; set; }
    public double Contrast { get; set; } = 1.0;   // alpha 0.5 .. 3.0
    public double Brightness { get; set; } = 0.0;  // beta -100 .. 100
    public bool WindowLevelEnabled { get; set; }
    public double Window { get; set; } = 255;      // width
    public double Level { get; set; } = 127;       // center
    public bool HistogramEqualize { get; set; }
    public bool Sharpen { get; set; }
    public bool Invert { get; set; }
}
