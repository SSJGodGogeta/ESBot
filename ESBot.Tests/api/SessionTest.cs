using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ESBot.Domain.Contracts.Session;
using ESBot.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;

namespace ESBot.Tests.api;

public class SessionTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly EsBotDbContext _db;

    public SessionTest(ApiFactory factory)
    {
        _client = factory.CreateClient();

        _db = factory.Services
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<EsBotDbContext>();
        DbSeeder.Seed(_db);
    }

    [Fact]
    public async Task GetSessions_Returns200()
    {
        var response = await _client.GetAsync("/v1/Session");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CreateSession_Returns201_With_SessionId()
    {
        var user = _db.Users.First();

        var dto = new CreateSessionDto
        {
            UserId = user.Id,
            Title = "Test Session"
        };

        var response = await _client.PostAsJsonAsync(
            "/v1/Session",
            dto);

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var session = await response.Content
            .ReadFromJsonAsync<SessionDto>();

        session.Should().NotBeNull();

        session!.Id.Should().NotBe(Guid.Empty);
        session.UserId.Should().Be(user.Id);
        session.Title.Should().Be("Test Session");

        session.StartedAt.Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        session.EndedAt.Should().BeNull();

        session.MessageCount.Should().Be(0);
        session.QuizRequestCount.Should().Be(0);
    }
}