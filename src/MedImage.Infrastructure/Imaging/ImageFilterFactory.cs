using MedImage.Domain.Imaging;
using MedImage.Infrastructure.Imaging.Filters;

namespace MedImage.Infrastructure.Imaging;

// FACTORY: builds the ordered list of filters for a given adjustment set.
public interface IImageFilterFactory
{
    IReadOnlyList<IImageFilter> Build(ImageAdjustments a);
}

public sealed class ImageFilterFactory : IImageFilterFactory
{
    public IReadOnlyList<IImageFilter> Build(ImageAdjustments a)
    {
        var pipeline = new List<IImageFilter>();

        if (a.Grayscale)               pipeline.Add(new GrayscaleFilter());
        if (a.WindowLevelEnabled)      pipeline.Add(new WindowLevelFilter(a.Window, a.Level));

        // Brightness/contrast always runs (identity when contrast=1, brightness=0).
        pipeline.Add(new BrightnessContrastFilter(a.Contrast, a.Brightness));

        if (a.HistogramEqualize)       pipeline.Add(new HistogramEqualizationFilter());
        if (a.Sharpen)                 pipeline.Add(new SharpenFilter());
        if (a.Invert)                  pipeline.Add(new InvertFilter());

        return pipeline;
    }
}
