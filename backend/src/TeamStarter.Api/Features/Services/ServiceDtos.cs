using System.ComponentModel.DataAnnotations;
using TeamStarter.Api.Features.Categories;

namespace TeamStarter.Api.Features.Services;

// DTOs are what the API sends/receives — never expose entities directly.

public record ServiceDto(
    int Id,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    bool IsActive,
    string? ImageUrl,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAtUtc);

public record CreateServiceRequest(
    [RequiredTrimmed, MaxLength(100)] string Name,
    [MaxLength(500)] string? Description,
    [Range(1, int.MaxValue)] int DurationMinutes,
    [Required, Range(typeof(decimal), "0", "9999999999999999.99")] decimal? Price,
    bool IsActive,
    [MaxLength(2048)] string? ImageUrl,
    [Range(1, int.MaxValue)] int CategoryId);

public record UpdateServiceRequest(
    [RequiredTrimmed, MaxLength(100)] string Name,
    [MaxLength(500)] string? Description,
    [Range(1, int.MaxValue)] int DurationMinutes,
    [Required, Range(typeof(decimal), "0", "9999999999999999.99")] decimal? Price,
    bool IsActive,
    [MaxLength(2048)] string? ImageUrl,
    [Range(1, int.MaxValue)] int CategoryId);
