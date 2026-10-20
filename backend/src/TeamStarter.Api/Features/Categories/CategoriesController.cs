using Microsoft.AspNetCore.Mvc;

namespace TeamStarter.Api.Features.Categories;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ICategoryService categories) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<CategoryDto>> GetAll(CancellationToken ct) =>
        await categories.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<CategoryDto> GetById(int id, CancellationToken ct) =>
        await categories.GetByIdAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request, CancellationToken ct)
    {
        var created = await categories.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<CategoryDto> Update(int id, UpdateCategoryRequest request, CancellationToken ct) =>
        await categories.UpdateAsync(id, request, ct);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await categories.DeleteAsync(id, ct);
        return NoContent();
    }
}
