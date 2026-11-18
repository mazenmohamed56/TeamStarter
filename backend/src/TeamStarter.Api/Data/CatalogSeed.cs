using TeamStarter.Api.Features.Categories;
using TeamStarter.Api.Features.Services;

namespace TeamStarter.Api.Data;

/// <summary>
/// Day 3 catalog seed. The guide requires 3 categories and 10 services and does not name them.
/// These names are sample catalog rows for that count. Providers are not seeded: UserId is required
/// and Identity users do not exist yet.
/// </summary>
public static class CatalogSeed
{
    public static readonly DateTime CreatedAtUtc = new(2026, 11, 17, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Category[] Categories =
    [
        new() { Id = 1, Name = "Hair", Description = "Haircuts and styling", CreatedAtUtc = CreatedAtUtc },
        new() { Id = 2, Name = "Skin", Description = "Facials and skin care", CreatedAtUtc = CreatedAtUtc },
        new() { Id = 3, Name = "Nails", Description = "Manicure and nail care", CreatedAtUtc = CreatedAtUtc }
    ];

    public static readonly Service[] Services =
    [
        new() { Id = 1, Name = "Haircut", Description = "Cut and finish", DurationMinutes = 30, Price = 80m, IsActive = true, CategoryId = 1, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 2, Name = "Beard trim", Description = "Beard shape and trim", DurationMinutes = 20, Price = 40m, IsActive = true, CategoryId = 1, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 3, Name = "Hair color", Description = "Full color service", DurationMinutes = 90, Price = 200m, IsActive = true, CategoryId = 1, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 4, Name = "Blow dry", Description = "Wash and blow dry", DurationMinutes = 40, Price = 60m, IsActive = true, CategoryId = 1, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 5, Name = "Classic facial", Description = "Standard facial", DurationMinutes = 60, Price = 150m, IsActive = true, CategoryId = 2, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 6, Name = "Deep cleansing", Description = "Deep cleansing facial", DurationMinutes = 75, Price = 180m, IsActive = true, CategoryId = 2, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 7, Name = "Brow shape", Description = "Eyebrow shaping", DurationMinutes = 20, Price = 50m, IsActive = true, CategoryId = 2, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 8, Name = "Manicure", Description = "Classic manicure", DurationMinutes = 45, Price = 70m, IsActive = true, CategoryId = 3, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 9, Name = "Pedicure", Description = "Classic pedicure", DurationMinutes = 60, Price = 90m, IsActive = true, CategoryId = 3, CreatedAtUtc = CreatedAtUtc },
        new() { Id = 10, Name = "Gel polish", Description = "Gel polish application", DurationMinutes = 50, Price = 100m, IsActive = true, CategoryId = 3, CreatedAtUtc = CreatedAtUtc }
    ];
}
