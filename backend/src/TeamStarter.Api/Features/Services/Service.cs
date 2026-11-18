using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamStarter.Api.Data;
using TeamStarter.Api.Features.Bookings;
using TeamStarter.Api.Features.Categories;
using TeamStarter.Api.Features.Providers;

namespace TeamStarter.Api.Features.Services;

public class Service
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<ProviderService> ProviderServices { get; set; } = [];
    public ICollection<Provider> Providers { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.Property(s => s.Name).HasMaxLength(100);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.ImageUrl).HasMaxLength(2048);
        builder.Property(s => s.Price).HasPrecision(18, 2);

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(CatalogSeed.Services);
    }
}



