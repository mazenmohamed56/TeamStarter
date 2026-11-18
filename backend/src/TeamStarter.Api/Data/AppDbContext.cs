using Microsoft.EntityFrameworkCore;
using TeamStarter.Api.Features.Bookings;
using TeamStarter.Api.Features.Categories;
using TeamStarter.Api.Features.Notifications;
using TeamStarter.Api.Features.Providers;
using TeamStarter.Api.Features.Reviews;
using TeamStarter.Api.Features.Services;
using TeamStarter.Api.Features.Tasks;

namespace TeamStarter.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<ProviderService> ProviderServices => Set<ProviderService>();
    public DbSet<WorkingHours> WorkingHours => Set<WorkingHours>();
    public DbSet<TimeOff> TimeOff => Set<TimeOff>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Picks up every IEntityTypeConfiguration<T> in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}








