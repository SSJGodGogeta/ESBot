using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ESBot.Domain.Contracts.Session;
using ESBot.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
namespace ESBot.Tests.api;

public class NegativeMessageContentTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly EsBotDbContext _db;

    public NegativeMessageContentTest(ApiFactory factory)
    {
        _client = factory.CreateClient();

        _db = factory.Services
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<EsBotDbContext>();
        DbSeeder.Seed(_db);
    }

    [Fact]
    public async Task EmptyContent_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/v1/Message", new
        {
            session_id = "a667cb37-84e8-4ad7-94c6-7d139636d8a3",
            content = ""
        });

        response.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LargeContent_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/v1/Message", new
        {
            session_id = "a667cb37-84e8-4ad7-94c6-7d139636d8a3",
            content = new string('x', 10001)
        });

        response.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
}