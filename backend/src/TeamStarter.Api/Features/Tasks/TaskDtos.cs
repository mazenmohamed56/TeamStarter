using System.ComponentModel.DataAnnotations;

namespace TeamStarter.Api.Features.Tasks;

// DTOs are what the API sends/receives — never expose entities directly.

public record TaskDto(int Id, string Title, string? Description, bool IsDone, DateTime CreatedAtUtc);

public record CreateTaskRequest(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description);

public record UpdateTaskRequest(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    bool IsDone);
