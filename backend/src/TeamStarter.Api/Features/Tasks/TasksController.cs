using Microsoft.AspNetCore.Mvc;

namespace TeamStarter.Api.Features.Tasks;

[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskService tasks) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<TaskDto>> GetAll(CancellationToken ct) =>
        await tasks.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<TaskDto> GetById(int id, CancellationToken ct) =>
        await tasks.GetByIdAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(CreateTaskRequest request, CancellationToken ct)
    {
        var created = await tasks.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<TaskDto> Update(int id, UpdateTaskRequest request, CancellationToken ct) =>
        await tasks.UpdateAsync(id, request, ct);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await tasks.DeleteAsync(id, ct);
        return NoContent();
    }
}
