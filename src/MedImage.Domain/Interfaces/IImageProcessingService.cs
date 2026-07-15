using MedImage.Domain.Imaging;

namespace MedImage.Domain.Interfaces;

// UI-facing contract. Returns PNG bytes so the App never references OpenCV.
public interface IImageProcessingService
{
    void LoadOriginal(string path);
    bool HasImage { get; }
    int Width { get; }
    int Height { get; }

    // Render the current original with the given adjustments -> PNG bytes.
    byte[] RenderPng(ImageAdjustments adjustments);

    // Render + burn the annotations, returning PNG bytes ready to save.
    byte[] ExportWithAnnotations(ImageAdjustments adjustments, IEnumerable<AnnotationDraw> annotations);

    void SavePng(byte[] png, string path);
}
