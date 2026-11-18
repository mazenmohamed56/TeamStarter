using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamStarter.Api.Features.Providers;
using TeamStarter.Api.Features.Reviews;
using TeamStarter.Api.Features.Services;

namespace TeamStarter.Api.Features.Bookings;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled,
    NoShow
}

public class Booking
{
    public int Id { get; set; }
    public required string CustomerId { get; set; }
    public IdentityUser Customer { get; set; } = null!;
    public int ProviderId { get; set; }
    public Provider Provider { get; set; } = null!;
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public BookingStatus Status { get; set; }
    public decimal PriceAtBooking { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = null!;
    public Review? Review { get; set; }
}

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.CustomerId).HasMaxLength(ProviderConfiguration.IdentityUserIdMaxLength);

        // Spec: HasOne(b => b.Customer).WithMany().HasForeignKey(b => b.CustomerId)
        //     .IsRequired().OnDelete(DeleteBehavior.Restrict).
        // Not applied until Identity is in this context. See ProviderConfiguration.
        builder.Ignore(b => b.Customer);

        builder.HasOne(b => b.Provider)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.ProviderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Service)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.ServiceId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(b => b.PriceAtBooking).HasPrecision(18, 2);
        builder.Property(b => b.RowVersion).IsRowVersion();
    }
}

