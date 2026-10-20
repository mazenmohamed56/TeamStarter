using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeamStarter.Api.Data;
using TeamStarter.Api.Features.Tasks;

namespace TeamStarter.Api.Tests;

/// <summary>Boots the real API in memory, swapping SQL Server for the EF InMemory provider.</summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(_dbName));
        });
    }
}

public class TasksApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_then_get_returns_the_task()
    {
        var create = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Write tests", null));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var created = await create.Content.ReadFromJsonAsync<TaskDto>();
        var fetched = await _client.GetFromJsonAsync<TaskDto>($"/api/tasks/{created!.Id}");

        Assert.Equal("Write tests", fetched!.Title);
        Assert.False(fetched.IsDone);
    }

    [Fact]
    public async Task Create_with_empty_title_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { title = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_missing_task_returns_404()
    {
        var response = await _client.GetAsync("/api/tasks/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
