using System.Net;
using System.Net.Http.Json;
using TeamStarter.Api.Features.Categories;

namespace TeamStarter.Api.Tests;

public class CategoriesApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_then_get_returns_the_category()
    {
        var created = await CreateAsync("Design", "  Visual work  ");

        var fetched = await _client.GetFromJsonAsync<CategoryDto>($"/api/categories/{created.Id}");

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal("Design", fetched.Name);
        Assert.Equal("Visual work", fetched.Description);
        Assert.NotEqual(default, fetched.CreatedAtUtc);
    }

    [Fact]
    public async Task Get_all_includes_created_categories()
    {
        var created = await CreateAsync("Listed category");

        var listed = await _client.GetFromJsonAsync<List<CategoryDto>>("/api/categories");

        Assert.NotNull(listed);
        Assert.Contains(listed, c => c.Id == created.Id && c.Name == "Listed category");
    }

    [Fact]
    public async Task Update_changes_name_and_description()
    {
        var created = await CreateAsync("Ops", "Old");

        var response = await _client.PutAsJsonAsync(
            $"/api/categories/{created.Id}",
            new UpdateCategoryRequest("Operations", "New description"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Operations", updated.Name);
        Assert.Equal("New description", updated.Description);
        Assert.Equal(created.CreatedAtUtc, updated.CreatedAtUtc);
    }

    [Fact]
    public async Task Update_with_the_same_name_succeeds()
    {
        var created = await CreateAsync("Facilities", "Keep me");

        var response = await _client.PutAsJsonAsync(
            $"/api/categories/{created.Id}",
            new UpdateCategoryRequest("Facilities", null));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.Equal("Facilities", updated!.Name);
        Assert.Null(updated.Description);
    }

    [Fact]
    public async Task Delete_removes_only_that_category()
    {
        var keep = await CreateAsync("Keep");
        var remove = await CreateAsync("Remove");

        var deleted = await _client.DeleteAsync($"/api/categories/{remove.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        var missing = await _client.GetAsync($"/api/categories/{remove.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);

        var stillThere = await _client.GetFromJsonAsync<CategoryDto>($"/api/categories/{keep.Id}");
        Assert.Equal("Keep", stillThere!.Name);
    }

    [Fact]
    public async Task Get_missing_category_returns_404()
    {
        var response = await _client.GetAsync("/api/categories/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_missing_category_returns_404()
    {
        var response = await _client.PutAsJsonAsync(
            "/api/categories/99999",
            new UpdateCategoryRequest("Missing", null));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_missing_category_returns_404()
    {
        var response = await _client.DeleteAsync("/api/categories/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_empty_name_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new { name = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_whitespace_name_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new { name = "   " });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_too_long_name_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new { name = new string('a', 101) });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_too_long_description_returns_400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/categories",
            new { name = "Valid", description = new string('b', 501) });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_with_empty_name_returns_400()
    {
        var created = await CreateAsync("Needs a name");

        var response = await _client.PutAsJsonAsync($"/api/categories/{created.Id}", new { name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_duplicate_name_returns_409()
    {
        await CreateAsync("Design systems");

        var duplicate = await _client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest("  design systems  ", null));

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        var body = await duplicate.Content.ReadAsStringAsync();
        Assert.Contains("already exists", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Update_to_existing_name_returns_409()
    {
        await CreateAsync("Alpha");
        var beta = await CreateAsync("Beta");

        var response = await _client.PutAsJsonAsync(
            $"/api/categories/{beta.Id}",
            new UpdateCategoryRequest("alpha", "unchanged"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var stillBeta = await _client.GetFromJsonAsync<CategoryDto>($"/api/categories/{beta.Id}");
        Assert.Equal("Beta", stillBeta!.Name);
    }

    private async Task<CategoryDto> CreateAsync(string name, string? description = null)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest(name, description));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(created);
        return created;
    }
}
