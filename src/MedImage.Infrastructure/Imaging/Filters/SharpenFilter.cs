using OpenCvSharp;

namespace MedImage.Infrastructure.Imaging.Filters;

public sealed class SharpenFilter : IImageFilter
{
    public Mat Apply(Mat src)
    {
        using var kernel = new Mat(3, 3, MatType.CV_32F);
        kernel.Set(0, 0, 0f);  kernel.Set(0, 1, -1f); kernel.Set(0, 2, 0f);
        kernel.Set(1, 0, -1f); kernel.Set(1, 1, 5f);  kernel.Set(1, 2, -1f);
        kernel.Set(2, 0, 0f);  kernel.Set(2, 1, -1f); kernel.Set(2, 2, 0f);
        var dst = new Mat();
        Cv2.Filter2D(src, dst, -1, kernel);
        return dst;
    }
}
