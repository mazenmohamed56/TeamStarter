using Microsoft.EntityFrameworkCore;
using TeamStarter.Api.Common;
using TeamStarter.Api.Data;

namespace TeamStarter.Api.Features.Tasks;

public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> GetAllAsync(CancellationToken ct);
    Task<TaskDto> GetByIdAsync(int id, CancellationToken ct);
    Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken ct);
    Task<TaskDto> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}

public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<IReadOnlyList<TaskDto>> GetAllAsync(CancellationToken ct) =>
        await db.Tasks
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(t => new TaskDto(t.Id, t.Title, t.Description, t.IsDone, t.CreatedAtUtc))
            .ToListAsync(ct);

    public async Task<TaskDto> GetByIdAsync(int id, CancellationToken ct) =>
        ToDto(await FindAsync(id, ct));

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken ct)
    {
        var task = new TaskItem { Title = request.Title, Description = request.Description };
        db.Tasks.Add(task);
        await db.SaveChangesAsync(ct);
        return ToDto(task);
    }

    public async Task<TaskDto> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await FindAsync(id, ct);
        task.Title = request.Title;
        task.Description = request.Description;
        task.IsDone = request.IsDone;
        await db.SaveChangesAsync(ct);
        return ToDto(task);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var task = await FindAsync(id, ct);
        db.Tasks.Remove(task);
        await db.SaveChangesAsync(ct);
    }

    private async Task<TaskItem> FindAsync(int id, CancellationToken ct) =>
        await db.Tasks.FindAsync([id], ct) ?? throw new NotFoundException($"Task {id} was not found.");

    private static TaskDto ToDto(TaskItem t) => new(t.Id, t.Title, t.Description, t.IsDone, t.CreatedAtUtc);
}
