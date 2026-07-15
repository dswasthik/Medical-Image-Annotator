namespace MedImage.Domain.Entities;

public enum AnnotationType { Line, Rectangle, Measurement, Text }

public class Annotation
{
    public int Id { get; set; }
    public int StudyId { get; set; }
    public Study? Study { get; set; }

    public AnnotationType Type { get; set; }
    // JSON geometry, e.g. {"x1":..,"y1":..,"x2":..,"y2":..}
    public string GeometryJson { get; set; } = "{}";
    public string? Label { get; set; }
    public double LengthPixels { get; set; }
}
