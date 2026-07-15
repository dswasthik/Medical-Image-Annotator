using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedImage.Infrastructure.Data;

// Lets `dotnet ef migrations add ...` work without running the WPF app.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=MedImageDb;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;
        return new AppDbContext(options);
    }
}
