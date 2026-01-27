using Microsoft.AspNetCore.Mvc.Testing;

namespace Spec;

public class TestEnvironment : IClassFixture<WebApplicationFactory<Program>>
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
}
