namespace MedImage.Domain.Entities;

public class Study
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? PatientRef { get; set; }
    public string OriginalImagePath { get; set; } = string.Empty;
    public string? ProcessedImagePath { get; set; }

    // Serialized ImageAdjustments so a study reopens with the same look.
    public string AdjustmentsJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Annotation> Annotations { get; set; } = new List<Annotation>();
}
