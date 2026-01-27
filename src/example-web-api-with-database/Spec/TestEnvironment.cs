using Microsoft.AspNetCore.Mvc.Testing;

namespace Spec;

public class TestEnvironment : IDisposable, IClassFixture<WebApplicationFactory<Program>>
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

    public void Dispose()
    {
        _factory.Dispose();
    }
}
