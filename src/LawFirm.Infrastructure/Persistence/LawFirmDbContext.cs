using System.Security.Claims;
using LawFirm.Domain.Entities;
using LawFirm.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Infrastructure.Persistence;

public sealed class LawFirmDbContext : DbContext
{

    public LawFirmDbContext(DbContextOptions<LawFirmDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientContact> ClientContacts => Set<ClientContact>();

    private static readonly DateTimeOffset SeedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).ValueGeneratedNever();

            entity.Property(c => c.ClientCode).HasMaxLength(30).IsRequired();
            entity.HasIndex(c => c.ClientCode).IsUnique();

            entity.Property(c => c.ClientType).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(30).IsRequired();

            entity.Property(c => c.FirstName).HasMaxLength(100);
            entity.Property(c => c.LastName).HasMaxLength(100);
            entity.Property(c => c.PreferredName).HasMaxLength(100);
            entity.Property(c => c.OrganizationName).HasMaxLength(200);
            entity.Property(c => c.TradingName).HasMaxLength(200);
            entity.Property(c => c.CompanyNumber).HasMaxLength(50);

            entity.Property(c => c.Email).HasMaxLength(256);
            entity.Property(c => c.Phone).HasMaxLength(50);

            entity.Property(c => c.AddressLine1).HasMaxLength(200);
            entity.Property(c => c.AddressLine2).HasMaxLength(200);
            entity.Property(c => c.City).HasMaxLength(100);
            entity.Property(c => c.State).HasMaxLength(100);
            entity.Property(c => c.Postcode).HasMaxLength(20);
            entity.Property(c => c.Country).HasMaxLength(100);

            entity.Property(c => c.InternalNotesSummary).HasMaxLength(1000);

            entity.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(c => c.UpdatedBy).HasMaxLength(100);

            entity.HasIndex(c => c.Status);
            entity.HasIndex(c => c.IsArchived);

            entity.HasMany(c => c.Contacts)
                .WithOne()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(c => c.Contacts)
                .HasField("_contacts")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.HasData(
                new
                {
                    Id = 1, ClientCode = "CLI-2026-000001",
                    ClientType = ClientType.Organization, Status = ClientStatus.ActiveClient,
                    OrganizationName = "Acme Holdings Pty Ltd", TradingName = "Acme",
                    CompanyNumber = "12 345 678 901", Email = "legal@acme.example",
                    Phone = "+61 2 5550 1000", City = "Sydney", State = "NSW", Country = "Australia",
                    IsArchived = false, CreatedAt = SeedDate, CreatedBy = "system"
                },
                new
                {
                    Id = 2, ClientCode = "CLI-2026-000002",
                    ClientType = ClientType.Individual, Status = ClientStatus.ActiveClient,
                    FirstName = "John", LastName = "Smith", Email = "john.smith@example.com",
                    Phone = "+61 400 111 222", City = "Melbourne", State = "VIC", Country = "Australia",
                    IsArchived = false, CreatedAt = SeedDate, CreatedBy = "system"
                },
                new
                {
                    Id = 3, ClientCode = "CLI-2026-000003",
                    ClientType = ClientType.Individual, Status = ClientStatus.NewIntake,
                    FirstName = "Maria", LastName = "Nguyen", Email = "maria.nguyen@example.com",
                    IsArchived = false, CreatedAt = SeedDate, CreatedBy = "system"
                });
        });

        modelBuilder.Entity<ClientContact>(entity =>
        {
            entity.ToTable("ClientContacts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.RelationshipType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.Company).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(1000);

            entity.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            entity.HasIndex(x => x.ClientId);

            entity.HasData(new
            {
                Id = 1, ClientId = 2,
                Name = "Jane Director", RelationshipType = "Director",
                Email = "jane@acme.example", Phone = "+61 400 000 001", Company = "Acme Holdings",
                IsActive = true, CreatedAt = SeedDate, CreatedBy = "system"
            });
        });

        base.OnModelCreating(modelBuilder);
    }

}
