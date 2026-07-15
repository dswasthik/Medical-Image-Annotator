using MedImage.Domain.Interfaces;
using MedImage.Infrastructure.Data;
using MedImage.Infrastructure.Imaging;
using MedImage.Infrastructure.Security;
using MedImage.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedImage.Infrastructure;

// Single place that wires every Infrastructure dependency (DIP + SRP).
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default")
            ?? @"Server=(localdb)\MSSQLLocalDB;Database=MedImageDb;Trusted_Connection=True;TrustServerCertificate=True;";

        // Transient DbContext: each view-model gets its own short-lived context,
        // which suits a single-user desktop app and avoids scope/singleton conflicts.
        services.AddDbContext<AppDbContext>(
            o => o.UseSqlServer(connectionString),
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Singleton);

        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ISessionService, SessionService>();

        services.AddSingleton<IImageFilterFactory, ImageFilterFactory>();
        services.AddTransient<IImageProcessingService, ImageProcessingService>();

        return services;
    }
}
