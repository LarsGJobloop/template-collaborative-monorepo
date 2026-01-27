using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Spec;

public class TestEnvironment : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{

    private readonly WebApplicationFactory<Program> _factory;

    public TestEnvironment(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public HttpClient NewClient()
    {
        return _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Reset the comment store before each test
        using var scope = _factory.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<CommentStore>();
        store.Clear();
        await Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
