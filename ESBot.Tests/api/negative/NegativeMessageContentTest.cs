using FluentAssertions;
using RestSharp;

namespace ESBot.Tests.api;

public class NegativeMessageContentTest
{

    [Fact]
    public async Task EmptyContent_Returns400()
    {
        RestClient client = new RestClient("http://localhost:5243");
        RestRequest request = new RestRequest("/v1/Message", Method.Post);
        request.AddJsonBody(new { session_id = "a667cb37-84e8-4ad7-94c6-7d139636d8a3", content = "" });

        RestResponse response = await client.ExecuteAsync(request);
        
        response.StatusCode.Should()
            .Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LargeContent_Returns400()
    {
        RestClient client = new RestClient("http://localhost:5243");
        RestRequest request = new RestRequest("/v1/Message", Method.Post);
        request.AddJsonBody(new { session_id = "a667cb37-84e8-4ad7-94c6-7d139636d8a3", content = "A".PadRight(4001, 'A') });
        RestResponse response = await client.ExecuteAsync(request);
        
        response.StatusCode.Should()
            .Be(System.Net.HttpStatusCode.BadRequest);
    }
}