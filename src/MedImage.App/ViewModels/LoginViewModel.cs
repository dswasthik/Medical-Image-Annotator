using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedImage.App.Services;
using MedImage.Domain.Interfaces;

namespace MedImage.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly ISessionService _session;
    private readonly INavigationService _nav;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string? _error;
    [ObservableProperty] private bool _isBusy;

    public LoginViewModel(IAuthService auth, ISessionService session, INavigationService nav)
    {
        _auth = auth;
        _session = session;
        _nav = nav;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            var result = await _auth.LoginAsync(Username, Password);
            if (!result.Success)
            {
                Error = result.Error;
                return;
            }
            _session.CurrentUser = result.User;
            _nav.NavigateTo<EditorViewModel>();
        }
        catch (Exception ex)
        {
            Error = "Login failed: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void GoToSignup() => _nav.NavigateTo<SignupViewModel>();
}
