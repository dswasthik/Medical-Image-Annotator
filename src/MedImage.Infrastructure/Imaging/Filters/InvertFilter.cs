using OpenCvSharp;

namespace MedImage.Infrastructure.Imaging.Filters;

public sealed class InvertFilter : IImageFilter
{
    public Mat Apply(Mat src)
    {
        var dst = new Mat();
        Cv2.BitwiseNot(src, dst);
        return dst;
    }
}
