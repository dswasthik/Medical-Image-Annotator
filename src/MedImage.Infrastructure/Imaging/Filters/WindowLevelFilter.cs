using OpenCvSharp;

namespace MedImage.Infrastructure.Imaging.Filters;

// Classic radiology window/level: remap [level-window/2, level+window/2] -> [0,255].
public sealed class WindowLevelFilter : IImageFilter
{
    private readonly double _window;
    private readonly double _level;

    public WindowLevelFilter(double window, double level)
    {
        _window = window <= 0 ? 1 : window;
        _level = level;
    }

    public Mat Apply(Mat src)
    {
        double alpha = 255.0 / _window;
        double beta = -(_level - _window / 2.0) * alpha;
        var dst = new Mat();
        src.ConvertTo(dst, -1, alpha, beta);
        return dst;
    }
}
