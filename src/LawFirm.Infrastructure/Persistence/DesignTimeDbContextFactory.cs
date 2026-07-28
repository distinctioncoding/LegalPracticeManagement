using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LawFirm.Infrastructure.Persistence;

/// <summary>
/// Lets the EF Core CLI (<c>dotnet ef migrations add ...</c>) build the DbContext without
/// running the full application. Migrations always target SQL Server (the production
/// provider, Volume 2 §9.1), regardless of what the running app uses for local dev.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LawFirmDbContext>
{
    public LawFirmDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LawFirmDbContext>()
            // A real connection is not needed to scaffold a migration; this is a placeholder.
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=LawFirm;Trusted_Connection=True;")
            .Options;

        return new LawFirmDbContext(options);
    }
}
