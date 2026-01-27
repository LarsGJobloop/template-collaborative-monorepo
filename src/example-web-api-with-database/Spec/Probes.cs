using Microsoft.AspNetCore.Mvc.Testing;

namespace Spec;

public class HealthProbeSpec : TestEnvironment
{

    public HealthProbeSpec(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task GivenAHealthyService_WhenTheProbeIsCalled_ThenTheResponseIsOK()
    {
        // Given a healthy service
        var client = NewClient();
        // When the probe is called
        var response = await client.GetAsync("/healthz");
        // Then the response is a success status code
        response.EnsureSuccessStatusCode();
        // And the content is "Healthy"
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", content);
    }
}
