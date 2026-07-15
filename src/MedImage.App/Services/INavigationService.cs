using CommunityToolkit.Mvvm.ComponentModel;

namespace MedImage.App.Services;

public interface INavigationService
{
    ObservableObject? Current { get; }
    event Action? Changed;
    void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
}
