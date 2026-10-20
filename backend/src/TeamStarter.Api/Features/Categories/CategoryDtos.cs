using System.ComponentModel.DataAnnotations;

namespace TeamStarter.Api.Features.Categories;

// DTOs are what the API sends/receives — never expose entities directly.

public record CategoryDto(int Id, string Name, string? Description, DateTime CreatedAtUtc);

public record CreateCategoryRequest(
    [RequiredTrimmed, MaxLength(100)] string Name,
    [MaxLength(500)] string? Description);

public record UpdateCategoryRequest(
    [RequiredTrimmed, MaxLength(100)] string Name,
    [MaxLength(500)] string? Description);

/// <summary>Rejects null, empty, and whitespace-only strings. Applied before the service trims the name.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class RequiredTrimmedAttribute : ValidationAttribute
{
    public RequiredTrimmedAttribute() => ErrorMessage = "The {0} field is required.";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text))
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

        return ValidationResult.Success;
    }
}
