using System.Net;
using FluentAssertions;
using RestSharp;

namespace ESBot.Tests.api;

public class HealthTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public HealthTest(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_Returns200()
    {
        var response = await _client.GetAsync("api/v1/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
        content.Should().Contain("Healthy");
    }
}