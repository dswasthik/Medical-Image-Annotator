namespace MedImage.App.Services;

// Abstracts file dialogs / message boxes so view-models stay UI-framework free.
public interface IDialogService
{
    string? OpenImage();
    string? SavePng(string suggestedName);
    void Info(string message, string title = "MedImage");
    void Error(string message, string title = "MedImage");
}
