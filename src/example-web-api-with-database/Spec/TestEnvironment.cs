using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Api;

namespace Spec;

public class TestEnvironment : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly ServiceDependencies _services;
    private WebApplicationFactory<Program> _factory = null!;

    public TestEnvironment(WebApplicationFactory<Program> factory)
    {
        _services = new ServiceDependencies();
    }

    // 1. Extracting API Clients
    public HttpClient NewClient() => _factory.CreateClient();

    // 2. Resetting the Application
    public HttpClient ResetApp()
    {
        _factory.Dispose();
        _factory = CreateConfiguredFactory();
        return _factory.CreateClient();
    }

    // 3. Resetting the Database
    public async Task ResetDatabase()
    {
        await _services.ResetPostgreSQL();
    }

    // 4. Configuring the Application
    private WebApplicationFactory<Program> CreateConfiguredFactory()
    {
        var connectionString = _services.GetDatabaseConnectionString();

        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("POSTGRES_CONNECTION_STRING", connectionString);
            });
    }

    public async Task InitializeAsync()
    {
        // Initialize database
        await _services.InitializePostgreSQL();

        // Create configured factory with database connection
        _factory = CreateConfiguredFactory();

        // Clear database before each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CommentaryContext>();
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Comments\"");
    }

    public async Task DisposeAsync()
    {
        _factory.Dispose();
        await _services.DisposeAsync();
    }
}
