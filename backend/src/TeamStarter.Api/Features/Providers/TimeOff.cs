using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamStarter.Api.Features.Providers;

public class TimeOff
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public Provider Provider { get; set; } = null!;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string? Reason { get; set; }
}

public class TimeOffConfiguration : IEntityTypeConfiguration<TimeOff>
{
    public void Configure(EntityTypeBuilder<TimeOff> builder)
    {
        builder.HasOne(t => t.Provider)
            .WithMany(p => p.TimeOff)
            .HasForeignKey(t => t.ProviderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
