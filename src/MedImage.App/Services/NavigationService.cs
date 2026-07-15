using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace MedImage.App.Services;

// Resolves view-models from the DI container and raises Changed so the shell updates.
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _provider;
    public ObservableObject? Current { get; private set; }
    public event Action? Changed;

    public NavigationService(IServiceProvider provider) => _provider = provider;

    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        Current = _provider.GetRequiredService<TViewModel>();
        Changed?.Invoke();
    }
}
