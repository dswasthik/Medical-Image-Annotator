using OpenCvSharp;

namespace MedImage.Infrastructure.Imaging.Filters;

// Equalize luminance only (YCrCb) so color is preserved.
public sealed class HistogramEqualizationFilter : IImageFilter
{
    public Mat Apply(Mat src)
    {
        using var ycrcb = new Mat();
        Cv2.CvtColor(src, ycrcb, ColorConversionCodes.BGR2YCrCb);
        var channels = Cv2.Split(ycrcb);
        try
        {
            Cv2.EqualizeHist(channels[0], channels[0]);
            using var merged = new Mat();
            Cv2.Merge(channels, merged);
            var dst = new Mat();
            Cv2.CvtColor(merged, dst, ColorConversionCodes.YCrCb2BGR);
            return dst;
        }
        finally
        {
            foreach (var c in channels) c.Dispose();
        }
    }
}
