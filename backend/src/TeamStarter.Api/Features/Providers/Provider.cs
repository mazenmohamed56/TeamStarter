using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamStarter.Api.Features.Bookings;
using TeamStarter.Api.Features.Services;

namespace TeamStarter.Api.Features.Providers;

public class Provider
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public IdentityUser User { get; set; } = null!;
    public required string DisplayName { get; set; }
    public string? Bio { get; set; }
    public ICollection<ProviderService> ProviderServices { get; set; } = [];
    public ICollection<Service> Services { get; set; } = [];
    public ICollection<WorkingHours> WorkingHours { get; set; } = [];
    public ICollection<TimeOff> TimeOff { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}

public class ProviderService
{
    public int ProviderId { get; set; }
    public Provider Provider { get; set; } = null!;
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
}

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    // Identity keys are nvarchar(450). SQL Server cannot index nvarchar(max), and the unique UserId index requires a length.
    public const int IdentityUserIdMaxLength = 450;

    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.Property(p => p.UserId).HasMaxLength(IdentityUserIdMaxLength);
        builder.HasIndex(p => p.UserId).IsUnique();

        // The Day 3 spec maps this with HasOne(p => p.User).WithOne()
        //     .HasForeignKey<Provider>(p => p.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict).
        // That mapping cannot be applied yet: this context has no Identity tables, and mapping
        // IdentityUser would create a non-Identity user table. The navigation stays on the class.
        builder.Ignore(p => p.User);

        builder.HasMany(p => p.Services)
            .WithMany(s => s.Providers)
            .UsingEntity<ProviderService>(
                right => right.HasOne(ps => ps.Service)
                    .WithMany(s => s.ProviderServices)
                    .HasForeignKey(ps => ps.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne(ps => ps.Provider)
                    .WithMany(p => p.ProviderServices)
                    .HasForeignKey(ps => ps.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade),
                join => join.HasKey(ps => new { ps.ProviderId, ps.ServiceId }));
    }
}

