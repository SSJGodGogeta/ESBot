using FluentAssertions;
using RestSharp;

namespace ESBot.Tests.api;

public class SessionTest
{

    [Fact]
    public async Task GetSessions_Returns200()
    {
        RestClient client = new RestClient("http://localhost:5243");
        RestRequest request = new RestRequest("/v1/sessions", Method.Get);
        
        RestResponse response = await client.ExecuteAsync(request);
        
        response.StatusCode.Should()
            .Be(System.Net.HttpStatusCode.OK);

        response.Content.Should()
            .NotBeNullOrWhiteSpace();
    }
}