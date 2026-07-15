using MedImage.Domain.Entities;
using MedImage.Domain.Imaging;
using MedImage.Domain.Interfaces;
using OpenCvSharp;

namespace MedImage.Infrastructure.Imaging;

public sealed class ImageProcessingService : IImageProcessingService, IDisposable
{
    private readonly IImageFilterFactory _factory;
    private Mat? _original;

    public ImageProcessingService(IImageFilterFactory factory) => _factory = factory;

    public bool HasImage => _original is { Empty: false };
    public int Width => _original?.Width ?? 0;
    public int Height => _original?.Height ?? 0;

    public void LoadOriginal(string path)
    {
        _original?.Dispose();
        // Read as color so the pipeline is uniform (BGR).
        _original = Cv2.ImRead(path, ImreadModes.Color);
        if (_original.Empty())
            throw new InvalidOperationException($"Could not read image: {path}");
    }

    public byte[] RenderPng(ImageAdjustments adjustments)
    {
        using var result = Render(adjustments);
        return Encode(result);
    }

    public byte[] ExportWithAnnotations(ImageAdjustments adjustments, IEnumerable<AnnotationDraw> annotations)
    {
        using var result = Render(adjustments);
        var color = new Scalar(0, 255, 255); // yellow (BGR)
        foreach (var d in annotations)
        {
            var p1 = new Point((int)d.X1, (int)d.Y1);
            var p2 = new Point((int)d.X2, (int)d.Y2);
            switch (d.Type)
            {
                case AnnotationType.Rectangle:
                    Cv2.Rectangle(result, p1, p2, color, 2);
                    break;
                default: // Line / Measurement
                    Cv2.Line(result, p1, p2, color, 2);
                    break;
            }
            if (!string.IsNullOrWhiteSpace(d.Label))
                Cv2.PutText(result, d.Label, p2, HersheyFonts.HersheySimplex, 0.6, color, 1, LineTypes.AntiAlias);
        }
        return Encode(result);
    }

    public void SavePng(byte[] png, string path) => File.WriteAllBytes(path, png);

    // CHAIN: apply each filter in order, disposing intermediates.
    private Mat Render(ImageAdjustments adjustments)
    {
        if (_original is null || _original.Empty())
            throw new InvalidOperationException("No image loaded.");

        Mat current = _original.Clone();
        foreach (var filter in _factory.Build(adjustments))
        {
            var next = filter.Apply(current);
            current.Dispose();
            current = next;
        }
        return current;
    }

    private static byte[] Encode(Mat m)
    {
        Cv2.ImEncode(".png", m, out var buf);
        return buf;
    }

    public void Dispose() => _original?.Dispose();
}
