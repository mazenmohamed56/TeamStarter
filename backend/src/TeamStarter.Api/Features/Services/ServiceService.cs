using Microsoft.EntityFrameworkCore;
using TeamStarter.Api.Common;
using TeamStarter.Api.Data;
using TeamStarter.Api.Features.Categories;

namespace TeamStarter.Api.Features.Services;

public interface IServiceService
{
    Task<IReadOnlyList<ServiceDto>> GetAllAsync(CancellationToken ct);
    Task<ServiceDto> GetByIdAsync(int id, CancellationToken ct);
    Task<ServiceDto> CreateAsync(CreateServiceRequest request, CancellationToken ct);
    Task<ServiceDto> UpdateAsync(int id, UpdateServiceRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}

public class ServiceService(AppDbContext db) : IServiceService
{
    public async Task<IReadOnlyList<ServiceDto>> GetAllAsync(CancellationToken ct) =>
        await db.Services
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(s => new ServiceDto(
                s.Id,
                s.Name,
                s.Description,
                s.DurationMinutes,
                s.Price,
                s.IsActive,
                s.ImageUrl,
                s.CategoryId,
                s.Category.Name,
                s.CreatedAtUtc))
            .ToListAsync(ct);

    public async Task<ServiceDto> GetByIdAsync(int id, CancellationToken ct) =>
        ToDto(await FindAsync(id, ct));

    public async Task<ServiceDto> CreateAsync(CreateServiceRequest request, CancellationToken ct)
    {
        var category = await FindCategoryAsync(request.CategoryId, ct);
        var service = new Service
        {
            Name = request.Name.Trim(),
            Description = NormalizeOptional(request.Description),
            DurationMinutes = request.DurationMinutes,
            Price = request.Price!.Value,
            IsActive = request.IsActive,
            ImageUrl = NormalizeOptional(request.ImageUrl),
            CategoryId = category.Id,
            Category = category
        };
        db.Services.Add(service);
        await db.SaveChangesAsync(ct);
        return ToDto(service);
    }

    public async Task<ServiceDto> UpdateAsync(int id, UpdateServiceRequest request, CancellationToken ct)
    {
        var service = await FindAsync(id, ct);
        var category = await FindCategoryAsync(request.CategoryId, ct);

        service.Name = request.Name.Trim();
        service.Description = NormalizeOptional(request.Description);
        service.DurationMinutes = request.DurationMinutes;
        service.Price = request.Price!.Value;
        service.IsActive = request.IsActive;
        service.ImageUrl = NormalizeOptional(request.ImageUrl);
        service.CategoryId = category.Id;
        service.Category = category;
        await db.SaveChangesAsync(ct);
        return ToDto(service);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var service = await FindAsync(id, ct);
        db.Services.Remove(service);
        await db.SaveChangesAsync(ct);
    }

    private async Task<Service> FindAsync(int id, CancellationToken ct) =>
        await db.Services.Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id, ct)
        ?? throw new NotFoundException($"Service {id} was not found.");

    private async Task<Category> FindCategoryAsync(int categoryId, CancellationToken ct) =>
        await db.Categories.FindAsync([categoryId], ct)
        ?? throw new NotFoundException($"Category {categoryId} was not found.");

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }

    private static ServiceDto ToDto(Service service) =>
        new(
            service.Id,
            service.Name,
            service.Description,
            service.DurationMinutes,
            service.Price,
            service.IsActive,
            service.ImageUrl,
            service.CategoryId,
            service.Category.Name,
            service.CreatedAtUtc);
}
