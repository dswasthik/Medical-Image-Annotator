using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedImage.App.Services;
using MedImage.Domain.Interfaces;

namespace MedImage.App.ViewModels;

public partial class SignupViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string? _error;
    [ObservableProperty] private bool _isBusy;

    public SignupViewModel(IAuthService auth, INavigationService nav, IDialogService dialog)
    {
        _auth = auth;
        _nav = nav;
        _dialog = dialog;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        Error = null;
        if (Password != ConfirmPassword)
        {
            Error = "Passwords do not match.";
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _auth.RegisterAsync(Username, Password);
            if (!result.Success)
            {
                Error = result.Error;
                return;
            }
            _dialog.Info("Account created. Please sign in.");
            _nav.NavigateTo<LoginViewModel>();
        }
        catch (Exception ex)
        {
            Error = "Registration failed: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void GoToLogin() => _nav.NavigateTo<LoginViewModel>();
}
