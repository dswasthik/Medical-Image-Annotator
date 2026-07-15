using OpenCvSharp;

namespace MedImage.Infrastructure.Imaging;

// STRATEGY: every enhancement is an interchangeable filter.
// Open/Closed: add a new look by adding a class, no pipeline edits.
public interface IImageFilter
{
    Mat Apply(Mat src);
}
