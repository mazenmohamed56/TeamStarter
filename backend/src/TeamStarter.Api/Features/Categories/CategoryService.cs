using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TeamStarter.Api.Common;
using TeamStarter.Api.Data;

namespace TeamStarter.Api.Features.Categories;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct);
    Task<CategoryDto> GetByIdAsync(int id, CancellationToken ct);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct);
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}

public class CategoryService(AppDbContext db) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct) =>
        await db.Categories
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAtUtc)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.CreatedAtUtc))
            .ToListAsync(ct);

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken ct) =>
        ToDto(await FindAsync(id, ct));

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct)
    {
        var name = request.Name.Trim();
        await EnsureUniqueNameAsync(name, null, ct);

        var category = new Category
        {
            Name = name,
            Description = NormalizeDescription(request.Description)
        };
        db.Categories.Add(category);
        await SaveAsync(name, ct);
        return ToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct)
    {
        var category = await FindAsync(id, ct);
        var name = request.Name.Trim();
        await EnsureUniqueNameAsync(name, id, ct);

        category.Name = name;
        category.Description = NormalizeDescription(request.Description);
        await SaveAsync(name, ct);
        return ToDto(category);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var category = await FindAsync(id, ct);
        if (await db.Services.AnyAsync(s => s.CategoryId == id, ct))
            throw new ConflictException($"Category {id} cannot be deleted because it has services.");

        db.Categories.Remove(category);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            throw new ConflictException($"Category {id} cannot be deleted because it has services.");
        }
    }

    private async Task EnsureUniqueNameAsync(string name, int? excludingId, CancellationToken ct)
    {
        var normalized = name.ToLower();
        var exists = await db.Categories.AnyAsync(
            c => c.Name.ToLower() == normalized && (excludingId == null || c.Id != excludingId),
            ct);

        if (exists)
            throw new ConflictException($"A category named '{name}' already exists.");
    }

    private async Task SaveAsync(string name, CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ConflictException($"A category named '{name}' already exists.");
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is SqlException { Number: 2601 or 2627 })
                return true;
        }

        return false;
    }

    private static bool IsForeignKeyViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is SqlException { Number: 547 })
                return true;
        }

        return false;
    }

    private async Task<Category> FindAsync(int id, CancellationToken ct) =>
        await db.Categories.FindAsync([id], ct) ?? throw new NotFoundException($"Category {id} was not found.");

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        return description.Trim();
    }

    private static CategoryDto ToDto(Category category) =>
        new(category.Id, category.Name, category.Description, category.CreatedAtUtc);
}



