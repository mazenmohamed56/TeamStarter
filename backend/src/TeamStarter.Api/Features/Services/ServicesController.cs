using Microsoft.AspNetCore.Mvc;

namespace TeamStarter.Api.Features.Services;

[ApiController]
[Route("api/services")]
public class ServicesController(IServiceService services) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<ServiceDto>> GetAll(CancellationToken ct) =>
        await services.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ServiceDto> GetById(int id, CancellationToken ct) =>
        await services.GetByIdAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create(CreateServiceRequest request, CancellationToken ct)
    {
        var created = await services.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ServiceDto> Update(int id, UpdateServiceRequest request, CancellationToken ct) =>
        await services.UpdateAsync(id, request, ct);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await services.DeleteAsync(id, ct);
        return NoContent();
    }
}
