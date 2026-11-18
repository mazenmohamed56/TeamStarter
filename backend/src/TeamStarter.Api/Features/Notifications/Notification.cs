using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamStarter.Api.Features.Providers;

namespace TeamStarter.Api.Features.Notifications;

public class Notification
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public IdentityUser User { get; set; } = null!;
    public required string Title { get; set; }
    public required string Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.UserId).HasMaxLength(ProviderConfiguration.IdentityUserIdMaxLength);

        // Spec: HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId)
        //     .IsRequired().OnDelete(DeleteBehavior.Cascade).
        // Not applied until Identity is in this context. See ProviderConfiguration.
        builder.Ignore(n => n.User);
    }
}

