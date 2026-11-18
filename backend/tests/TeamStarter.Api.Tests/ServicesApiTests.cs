using System.Net;
using System.Net.Http.Json;
using TeamStarter.Api.Features.Categories;
using TeamStarter.Api.Features.Services;

namespace TeamStarter.Api.Tests;

public class ServicesApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_then_get_returns_the_service()
    {
        var category = await CreateCategoryAsync();

        var created = await CreateAsync(
            category.Id,
            "  Haircut  ",
            "  Basic cut  ",
            30,
            25.50m,
            true,
            "  https://cdn.example/haircut.png  ");

        var fetched = await _client.GetFromJsonAsync<ServiceDto>($"/api/services/{created.Id}");

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal("Haircut", fetched.Name);
        Assert.Equal("Basic cut", fetched.Description);
        Assert.Equal(30, fetched.DurationMinutes);
        Assert.Equal(25.50m, fetched.Price);
        Assert.True(fetched.IsActive);
        Assert.Equal("https://cdn.example/haircut.png", fetched.ImageUrl);
        Assert.Equal(category.Id, fetched.CategoryId);
        Assert.Equal(category.Name, fetched.CategoryName);
        Assert.NotEqual(default, fetched.CreatedAtUtc);
    }

    [Fact]
    public async Task Get_all_includes_created_services()
    {
        var category = await CreateCategoryAsync();
        var created = await CreateAsync(category.Id, "Listed service");

        var listed = await _client.GetFromJsonAsync<List<ServiceDto>>("/api/services");

        Assert.NotNull(listed);
        Assert.Contains(listed, s => s.Id == created.Id && s.Name == "Listed service" && s.CategoryName == category.Name);
    }

    [Fact]
    public async Task Update_changes_fields_and_keeps_created_at()
    {
        var original = await CreateCategoryAsync();
        var next = await CreateCategoryAsync();
        var created = await CreateAsync(original.Id, "Trim", "Old", 20, 10m, true, "https://cdn.example/old.png");

        var response = await _client.PutAsJsonAsync(
            $"/api/services/{created.Id}",
            new UpdateServiceRequest("  Color  ", "  New description  ", 45, 80m, false, "  https://cdn.example/new.png  ", next.Id));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<ServiceDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Color", updated.Name);
        Assert.Equal("New description", updated.Description);
        Assert.Equal(45, updated.DurationMinutes);
        Assert.Equal(80m, updated.Price);
        Assert.False(updated.IsActive);
        Assert.Equal("https://cdn.example/new.png", updated.ImageUrl);
        Assert.Equal(next.Id, updated.CategoryId);
        Assert.Equal(next.Name, updated.CategoryName);
        Assert.Equal(created.CreatedAtUtc, updated.CreatedAtUtc);
    }

    [Fact]
    public async Task Update_can_clear_optional_text()
    {
        var category = await CreateCategoryAsync();
        var created = await CreateAsync(category.Id, "Keep name", "Remove me", 15, 0m, true, "https://cdn.example/gone.png");

        var response = await _client.PutAsJsonAsync(
            $"/api/services/{created.Id}",
            new UpdateServiceRequest("Keep name", "   ", 15, 0m, true, "   ", category.Id));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<ServiceDto>();
        Assert.Equal("Keep name", updated!.Name);
        Assert.Null(updated.Description);
        Assert.Null(updated.ImageUrl);
        Assert.Equal(0m, updated.Price);
    }

    [Fact]
    public async Task Delete_removes_only_that_service()
    {
        var category = await CreateCategoryAsync();
        var keep = await CreateAsync(category.Id, "Keep service");
        var remove = await CreateAsync(category.Id, "Remove service");

        var deleted = await _client.DeleteAsync($"/api/services/{remove.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        var missing = await _client.GetAsync($"/api/services/{remove.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);

        var stillThere = await _client.GetFromJsonAsync<ServiceDto>($"/api/services/{keep.Id}");
        Assert.Equal("Keep service", stillThere!.Name);

        var categoryStillThere = await _client.GetFromJsonAsync<CategoryDto>($"/api/categories/{category.Id}");
        Assert.Equal(category.Name, categoryStillThere!.Name);
    }

    [Fact]
    public async Task Get_missing_service_returns_404()
    {
        var response = await _client.GetAsync("/api/services/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_missing_service_returns_404()
    {
        var category = await CreateCategoryAsync();
        var response = await _client.PutAsJsonAsync(
            "/api/services/99999",
            new UpdateServiceRequest("Missing", null, 30, 10m, true, null, category.Id));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_missing_service_returns_404()
    {
        var response = await _client.DeleteAsync("/api/services/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_missing_category_returns_404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/services",
            new CreateServiceRequest("Orphan", null, 30, 10m, true, null, 99999));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_with_missing_category_returns_404()
    {
        var category = await CreateCategoryAsync();
        var created = await CreateAsync(category.Id, "Stays put");

        var response = await _client.PutAsJsonAsync(
            $"/api/services/{created.Id}",
            new UpdateServiceRequest("Stays put", null, 30, 10m, true, null, 99999));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var stillThere = await _client.GetFromJsonAsync<ServiceDto>($"/api/services/{created.Id}");
        Assert.Equal(category.Id, stillThere!.CategoryId);
    }

    [Fact]
    public async Task Create_with_empty_name_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/services", ValidBody(name: ""));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_whitespace_name_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/services", ValidBody(name: "   "));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_too_long_name_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/services", ValidBody(name: new string('a', 101)));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_too_long_description_returns_400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/services",
            ValidBody(description: new string('b', 501)));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_too_long_image_url_returns_400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/services",
            ValidBody(imageUrl: new string('u', 2049)));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_zero_duration_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/services", ValidBody(durationMinutes: 0));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_negative_price_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/services", ValidBody(price: -1));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_without_price_returns_400()
    {
        var body = ValidBody();
        body.Remove("price");
        var response = await _client.PostAsJsonAsync("/api/services", body);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_missing_category_id_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/services", ValidBody(categoryId: 0));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_with_empty_name_returns_400()
    {
        var category = await CreateCategoryAsync();
        var created = await CreateAsync(category.Id, "Needs a name");

        var response = await _client.PutAsJsonAsync($"/api/services/{created.Id}", ValidBody(name: ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_category_with_services_returns_409()
    {
        var category = await CreateCategoryAsync();
        var service = await CreateAsync(category.Id, "Blocks delete");

        var response = await _client.DeleteAsync($"/api/categories/{category.Id}");
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("services", body, StringComparison.OrdinalIgnoreCase);

        var categoryStillThere = await _client.GetFromJsonAsync<CategoryDto>($"/api/categories/{category.Id}");
        Assert.Equal(category.Name, categoryStillThere!.Name);
        var serviceStillThere = await _client.GetFromJsonAsync<ServiceDto>($"/api/services/{service.Id}");
        Assert.Equal("Blocks delete", serviceStillThere!.Name);
    }

    [Fact]
    public async Task Delete_category_succeeds_after_its_service_is_removed()
    {
        var category = await CreateCategoryAsync();
        var service = await CreateAsync(category.Id, "Temporary");

        var removed = await _client.DeleteAsync($"/api/services/{service.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removed.StatusCode);

        var deleted = await _client.DeleteAsync($"/api/categories/{category.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
    }

    private async Task<CategoryDto> CreateCategoryAsync()
    {
        var name = $"Category {Guid.NewGuid():N}";
        var response = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest(name, null));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(created);
        return created;
    }

    private async Task<ServiceDto> CreateAsync(
        int categoryId,
        string name,
        string? description = null,
        int durationMinutes = 30,
        decimal price = 10m,
        bool isActive = true,
        string? imageUrl = null)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/services",
            new CreateServiceRequest(name, description, durationMinutes, price, isActive, imageUrl, categoryId));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<ServiceDto>();
        Assert.NotNull(created);
        return created;
    }

    private static Dictionary<string, object?> ValidBody(
        string name = "Valid",
        string? description = null,
        int durationMinutes = 30,
        decimal price = 10m,
        bool isActive = true,
        string? imageUrl = null,
        int categoryId = 1) =>
        new()
        {
            ["name"] = name,
            ["description"] = description,
            ["durationMinutes"] = durationMinutes,
            ["price"] = price,
            ["isActive"] = isActive,
            ["imageUrl"] = imageUrl,
            ["categoryId"] = categoryId
        };
}
