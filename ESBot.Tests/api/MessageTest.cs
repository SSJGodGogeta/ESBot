using System.Net;
using System.Net.Http.Json;
using ESBot.Domain.Contracts.Message;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ESBot.Tests.api;

public class MessageTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly EsBotDbContext _db;

    public MessageTest(ApiFactory factory)
    {
        _client = factory.CreateClient();

        _db = factory.Services
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<EsBotDbContext>();
        DbSeeder.Seed(_db);
    }

    [Fact]
    public async Task CreateMessageToSession_Returns_201()
    {
        var session = _db.UserSessions.FirstOrDefault();
        session.Should().NotBeNull();

        var createMessageDto = new CreateMessageDto
        {
            Content = "Hello World",
            SessionId = session.Id,
            Role = EMessageRole.User
        };
        
        var messageResponse = await _client.PostAsJsonAsync(
            "/v1/Message",
            createMessageDto);
        
        messageResponse.Should().NotBeNull();
        messageResponse.IsSuccessStatusCode.Should().BeTrue();
        messageResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    }

    [Fact]
    public async Task RetrieveMessageHistory_Returns200_And_ListContainingMessages()
    {
        var session = _db.UserSessions.FirstOrDefault();
        session.Should().NotBeNull();

        var createMessageDto = new CreateMessageDto
        {
            Content = "Hello World2",
            SessionId = session.Id,
            Role = EMessageRole.User
        };
        
        var messageResponse = await _client.PostAsJsonAsync(
            $"/v1/Message?SessionId={session.Id}",
            createMessageDto);
        
        var messageDto = await messageResponse.Content.ReadFromJsonAsync<MessageDto>();
        
        messageResponse.Should().NotBeNull();
        messageResponse.IsSuccessStatusCode.Should().BeTrue();
        messageResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getMessageResponse = await _client.GetAsync("/v1/Message");
        getMessageResponse.Should().NotBeNull();
        getMessageResponse.IsSuccessStatusCode.Should().BeTrue();
        
        var messageResponses = await getMessageResponse.Content.ReadFromJsonAsync<List<MessageDto>>();
        messageResponses.Should().NotBeNull();
        messageResponses.Should().NotBeEmpty();
        
        messageResponses.Should().ContainSingle(m => m.Id == messageDto!.Id);
    }
}