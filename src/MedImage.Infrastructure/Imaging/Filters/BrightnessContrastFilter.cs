using OpenCvSharp;

namespace MedImage.Infrastructure.Imaging.Filters;

public sealed class BrightnessContrastFilter : IImageFilter
{
    private readonly double _alpha; // contrast
    private readonly double _beta;  // brightness

    public BrightnessContrastFilter(double contrast, double brightness)
    {
        _alpha = contrast;
        _beta = brightness;
    }

    public Mat Apply(Mat src)
    {
        var dst = new Mat();
        src.ConvertTo(dst, -1, _alpha, _beta);
        return dst;
    }
}
