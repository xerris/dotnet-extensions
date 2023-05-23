using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xerris.Extensions.Authentication.OAuth;
using Xerris.Extensions.Testing;
using Xerris.Extensions.Testing.Http;

namespace Xerris.Extensions.Authentication.Tests.OAuth;

public class DummyHttpService
{
    private readonly HttpClient _client;

    public DummyHttpService(HttpClient client)
    {
        _client = client;
    }

    public Task MakeRequest()
    {
        return _client.GetAsync("http://example.com");
    }
}

public class BearerTokenAuthenticationHandlerTests
{
    [Fact]
    public async Task Access_tokens_are_added_in_authentication_header_of_request()
    {
        // Arrange
        var actualRequest = new HttpRequestMessage();

        const string accessTokenValue = "access_token";

        var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
        mockAccessTokenProvider
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new AccessTokenResponse { AccessToken = accessTokenValue });

        async Task TestAction(DummyHttpService service)
        {
            await service.MakeRequest();

            var authorizationHeader = actualRequest.Headers.Authorization;

            authorizationHeader!.Scheme.Should().Be("Bearer");
            authorizationHeader.Parameter.Should().Be(accessTokenValue);
        }

        await ServiceTestHarness<DummyHttpService>.Create(TestAction)
            .WithServices(services =>
            {
                services.AddAccessTokenProvider(
                    configure => configure.UseCustomProvider(mockAccessTokenProvider.Object));

                services.AddTransient(_ => new HttpTestUtilities.DelegatingHandlerStub((request, _) =>
                {
                    actualRequest = request;

                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                }));

                services.AddTransient(sp =>
                    new BearerTokenAuthenticationHandler(
                        sp.GetRequiredService<IAccessTokenProvider>(),
                        "foo", "bar"));

                services.AddHttpClient<DummyHttpService>()
                    .AddHttpMessageHandler<BearerTokenAuthenticationHandler>()
                    .AddHttpMessageHandler<HttpTestUtilities.DelegatingHandlerStub>();
            })
            .TestAsync();
    }
}
