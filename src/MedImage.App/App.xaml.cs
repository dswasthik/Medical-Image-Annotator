using System.IO;
using System.Windows;
using MedImage.App.Services;
using MedImage.App.ViewModels;
using MedImage.App.Views;
using MedImage.Infrastructure;
using MedImage.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MedImage.App;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.SetBasePath(Directory.GetCurrentDirectory());
                cfg.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((context, services) =>
            {
                // Infrastructure (DbContext, repositories, auth, imaging)
                services.AddInfrastructure(context.Configuration);

                // App services
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IDialogService, DialogService>();

                // View-models
                services.AddSingleton<ShellViewModel>();
                services.AddTransient<LoginViewModel>();
                services.AddTransient<SignupViewModel>();
                services.AddTransient<EditorViewModel>();

                // Windows
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var sp = _host.Services;

        // Create the database schema if it does not exist yet.
        using (var db = sp.GetRequiredService<AppDbContext>())
            db.Database.EnsureCreated();

        var nav = sp.GetRequiredService<INavigationService>();
        var shell = sp.GetRequiredService<MainWindow>();
        shell.DataContext = sp.GetRequiredService<ShellViewModel>();

        nav.NavigateTo<LoginViewModel>();
        shell.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}
