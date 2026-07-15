using OpenCvSharp;

namespace MedImage.Infrastructure.Imaging.Filters;

public sealed class GrayscaleFilter : IImageFilter
{
    public Mat Apply(Mat src)
    {
        using var gray = new Mat();
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
        var bgr = new Mat();
        Cv2.CvtColor(gray, bgr, ColorConversionCodes.GRAY2BGR);
        return bgr;
    }
}
