using Microsoft.AspNetCore.Mvc.Testing;

namespace Spec;

public class HealthProbeSpec : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public HealthProbeSpec(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GivenAHealthyService_WhenTheProbeIsCalled_ThenTheResponseIsOK()
    {
        // Given a healthy service
        var client = _factory.CreateClient();
        // When the probe is called
        var response = await client.GetAsync("/healthz");
        // Then the response is a success status code
        response.EnsureSuccessStatusCode();
        // And the content is "Healthy"
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", content);
    }
}
