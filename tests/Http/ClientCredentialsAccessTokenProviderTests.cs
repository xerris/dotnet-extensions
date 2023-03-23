using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Xerris.Extensions.Http.OAuth;
using Xerris.Extensions.Http.OAuth.Internal;
using Xerris.Extensions.Testing;

namespace Xerris.Extensions.Http.Tests;

public class ClientCredentialsAccessTokenProviderTests
{
    [Fact]
    public async Task Returns_expected_access_token()
    {
        // Arrange
        var response = new AccessTokenResponse
        {
            AccessToken = "foobar",
            ExpiresIn = (int) TimeSpan.FromDays(1).TotalSeconds,
            TokenType = "bearer"
        };

        var handlerMock = GetMockHttpMessageHandler(response);

        await ServiceTestHarness<ClientCredentialsAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddTransient(_ => new HttpClient(handlerMock.Object));

                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseClientCredentialsFlow(new ClientCredentialsProviderOptions
                    {
                        TokenEndpoint = new Uri("https://test.com/"),
                        ClientId = "foo",
                        ClientSecret = "bar"
                    });
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var actualResponse = await provider.GetAccessTokenAsync();

            // Assert
            actualResponse.Should().BeEquivalentTo(response);
        }
    }

    [Fact]
    public async Task Inner_client_posts_request_as_configured()
    {
        // Arrange
        var response = new AccessTokenResponse
        {
            AccessToken = "foobar",
            ExpiresIn = (int) TimeSpan.FromDays(1).TotalSeconds,
            TokenType = "bearer"
        };

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(response))
        };

        var actualRequest = new HttpRequestMessage();

        var handlerMock = GetMockHttpMessageHandler(httpResponse, (req, _) => actualRequest = req);

        var providerOptions = new ClientCredentialsProviderOptions
        {
            TokenEndpoint = new Uri("https://test.com/"),
            ClientId = "foo",
            ClientSecret = "bar",
            AdditionalProperties = new Dictionary<string, string> { { "foo", "bar" } }
        };

        await ServiceTestHarness<ClientCredentialsAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddTransient(_ => new HttpClient(handlerMock.Object));

                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseClientCredentialsFlow(providerOptions);
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            const string someScope = "some_scope";

            // Act
            var result = await provider.GetAccessTokenAsync(someScope);

            // Assert
            result.AccessToken.Should().Be(response.AccessToken);

            actualRequest.RequestUri.Should().Be(providerOptions.TokenEndpoint.ToString());

            var actualRequestContent =
                await actualRequest.Content!.ReadFromJsonAsync<ClientCredentialsAccessTokenRequest>();

            actualRequestContent!.ClientId.Should().Be(providerOptions.ClientId);
            actualRequestContent.ClientSecret.Should().Be(providerOptions.ClientSecret);
            actualRequestContent.Scope.Should().Be(someScope);

            actualRequestContent.AdditionalProperties!.Keys
                .Should().BeEquivalentTo(providerOptions.AdditionalProperties.Keys);

            actualRequestContent.AdditionalProperties.Values.Select(v => v.ToString())
                .Should().BeEquivalentTo(providerOptions.AdditionalProperties.Values);

            actualRequestContent.GrantType.Should().Be("client_credentials");
        }
    }

    [Fact]
    public async Task Throws_exception_for_unsuccessful_request()
    {
        // Arrange
        var handlerMock = GetMockHttpMessageHandler(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var providerOptions = new ClientCredentialsProviderOptions
        {
            TokenEndpoint = new Uri("https://test.com/"),
            ClientId = "foo",
            ClientSecret = "bar",
            AdditionalProperties = new Dictionary<string, string> { { "foo", "bar" } }
        };

        await ServiceTestHarness<ClientCredentialsAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddTransient(_ => new HttpClient(handlerMock.Object));

                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseClientCredentialsFlow(providerOptions);
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var action = async () => await provider.GetAccessTokenAsync();

            // Assert
            await action.Should().ThrowAsync<HttpRequestException>();
        }
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(AccessTokenResponse response)
    {
        return GetMockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(response))
        });
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpResponseMessage response,
        Action<HttpRequestMessage, CancellationToken>? requestCallback = null)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                nameof(HttpClient.SendAsync),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback(requestCallback ?? ((_, _) => { /* no-op */ }))
            .ReturnsAsync(response)
            .Verifiable();

        return handlerMock;
    }
}
