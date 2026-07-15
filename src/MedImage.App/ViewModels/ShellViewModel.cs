using CommunityToolkit.Mvvm.ComponentModel;
using MedImage.App.Services;

namespace MedImage.App.ViewModels;

// Hosts whatever view-model is currently active (login -> signup -> editor).
public partial class ShellViewModel : ObservableObject
{
    private readonly INavigationService _nav;

    public ObservableObject? Current => _nav.Current;

    public ShellViewModel(INavigationService nav)
    {
        _nav = nav;
        _nav.Changed += () => OnPropertyChanged(nameof(Current));
    }
}
