using System.Net;
using System.Net.Http.Json;
using Xerris.Extensions.Common.Serialization;
using Xerris.Extensions.Testing.Http;

namespace Xerris.Extensions.Http.Tests;

public class HttpClientExtensionsTests
{
    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record RequestType(string Value);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record ResponseType(string Value);

    [Fact]
    public async Task PostAsJson_posts_json_request_and_deserializes_response()
    {
        // Arrange
        var requestValue = new RequestType("foo");

        var expectedResponseValue = new ResponseType("bar");

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Content = new StringContent(expectedResponseValue.ToJson())
        };

        var actualRequest = new HttpRequestMessage();

        var handlerMock = HttpTestUtilities.GetMockHttpMessageHandler(httpResponse, (req, _) => actualRequest = req);

        var httpClient = new HttpClient(handlerMock.Object);

        // Act
        var response = await httpClient
            .PostAsJsonAsync<RequestType, ResponseType>("http://example.com", requestValue);

        // Assert
        actualRequest.Method.Should().Be(HttpMethod.Post);

        var actualRequestValue = await actualRequest.Content!.ReadFromJsonAsync<RequestType>();
        actualRequestValue.Should().BeEquivalentTo(requestValue);

        response.Should().BeEquivalentTo(expectedResponseValue);
    }

    [Fact]
    public async Task PostAsJson_throws_exception_for_unsuccessful_request()
    {
        // Arrange
        var handlerMock = HttpTestUtilities.GetMockHttpMessageHandler(
            new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var httpClient = new HttpClient(handlerMock.Object);

        // Act
        var action = async () =>
            await httpClient.PostAsJsonAsync<RequestType, ResponseType>("http://example.com", new RequestType("foo"));

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }


    [Fact]
    public async Task PutAsJson_posts_json_request_and_deserializes_response()
    {
        // Arrange
        var requestValue = new RequestType("foo");

        var expectedResponseValue = new ResponseType("bar");

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(expectedResponseValue.ToJson())
        };

        var actualRequest = new HttpRequestMessage();

        var handlerMock = HttpTestUtilities.GetMockHttpMessageHandler(httpResponse, (req, _) => actualRequest = req);

        var httpClient = new HttpClient(handlerMock.Object);

        // Act
        var response = await httpClient
            .PutAsJsonAsync<RequestType, ResponseType>("http://example.com", requestValue);

        // Assert
        actualRequest.Method.Should().Be(HttpMethod.Put);

        response.Should().BeEquivalentTo(expectedResponseValue);

        var actualRequestValue = await actualRequest.Content!.ReadFromJsonAsync<RequestType>();
        actualRequestValue.Should().BeEquivalentTo(requestValue);
    }

    [Fact]
    public async Task PutAsJson_throws_exception_for_unsuccessful_request()
    {
        // Arrange
        var handlerMock = HttpTestUtilities.GetMockHttpMessageHandler(
            new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var httpClient = new HttpClient(handlerMock.Object);

        // Act
        var action = async () =>
            await httpClient.PutAsJsonAsync<RequestType, ResponseType>("http://example.com", new RequestType("foo"));

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task DeleteAsJson_posts_json_request_and_deserializes_response()
    {
        // Arrange
        var requestValue = new RequestType("foo");

        var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };

        var actualRequest = new HttpRequestMessage();

        var handlerMock = HttpTestUtilities.GetMockHttpMessageHandler(httpResponse, (req, _) => actualRequest = req);

        var httpClient = new HttpClient(handlerMock.Object);

        // Act
        await httpClient.DeleteAsJsonAsync("http://example.com", requestValue);

        // Assert
        actualRequest.Method.Should().Be(HttpMethod.Delete);

        var actualRequestValue = await actualRequest.Content!.ReadFromJsonAsync<RequestType>();
        actualRequestValue.Should().BeEquivalentTo(requestValue);
    }

    [Fact]
    public async Task DeleteAsJson_throws_exception_for_unsuccessful_request()
    {
        // Arrange
        var handlerMock = HttpTestUtilities.GetMockHttpMessageHandler(
            new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var httpClient = new HttpClient(handlerMock.Object);

        // Act
        var action = async () => await httpClient.DeleteAsJsonAsync("http://example.com", new RequestType("foo"));

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }
}
