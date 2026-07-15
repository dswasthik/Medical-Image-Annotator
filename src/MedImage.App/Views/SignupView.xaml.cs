using System.Windows.Controls;
using MedImage.App.ViewModels;

namespace MedImage.App.Views;

public partial class SignupView : UserControl
{
    public SignupView() => InitializeComponent();

    private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SignupViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }

    private void ConfirmBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SignupViewModel vm)
            vm.ConfirmPassword = ((PasswordBox)sender).Password;
    }
}
