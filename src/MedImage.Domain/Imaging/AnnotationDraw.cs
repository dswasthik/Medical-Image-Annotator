using MedImage.Domain.Entities;

namespace MedImage.Domain.Imaging;

// Instruction for burning an annotation onto an exported image.
public record AnnotationDraw(
    AnnotationType Type,
    double X1, double Y1,
    double X2, double Y2,
    string? Label);
