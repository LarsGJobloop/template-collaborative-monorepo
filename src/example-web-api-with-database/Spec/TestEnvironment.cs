using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Spec;

public class TestEnvironment : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{

    private readonly WebApplicationFactory<Program> _factory;
    private WebApplicationFactory<Program>? _currentFactory;

    public TestEnvironment(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _currentFactory = factory;
    }

    public HttpClient NewClient()
    {
        return _currentFactory!.CreateClient();
    }

    public void Kill()
    {
        if (_currentFactory != null && _currentFactory != _factory)
        {
            _currentFactory.Dispose();
        }
        _currentFactory = null;
    }

    public HttpClient Start()
    {
        // Always create a fresh factory to simulate server restart
        if (_currentFactory != null && _currentFactory != _factory)
        {
            _currentFactory.Dispose();
        }
        _currentFactory = new WebApplicationFactory<Program>();
        return _currentFactory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Reset the comment store before each test
        using var scope = _factory.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<CommentStore>();
        store.Clear();
        await Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Kill();
        return Task.CompletedTask;
    }
}
