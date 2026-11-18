using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamStarter.Api.Features.Providers;

public class WorkingHours
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public Provider Provider { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

public class WorkingHoursConfiguration : IEntityTypeConfiguration<WorkingHours>
{
    public void Configure(EntityTypeBuilder<WorkingHours> builder)
    {
        builder.HasOne(w => w.Provider)
            .WithMany(p => p.WorkingHours)
            .HasForeignKey(w => w.ProviderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
