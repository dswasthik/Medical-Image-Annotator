namespace MedImage.App.ViewModels;

public record StudyListItem(int Id, string Title, string? PatientRef, DateTime CreatedAt)
{
    public string Display => $"{Title}  ({CreatedAt.ToLocalTime():g})";
}
