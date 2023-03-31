using Moq;
using Xerris.Extensions.Http.OAuth;
using Xerris.Extensions.Testing;

namespace Xerris.Extensions.Http.Tests.OAuth;

public class DistributedCachingAccessTokenProviderTests
{
    [Fact]
    public async Task Unexpired_access_token_responses_are_retrieved_from_cache()
    {
        // Arrange
        var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();

        mockAccessTokenProvider
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<string[]>()))
            .ReturnsAsync(() => new AccessTokenResponse
            {
                AccessToken = Guid.NewGuid().ToString(), // Repeated calls will generate different values
                ExpiresIn = (int) TimeSpan.FromHours(1).TotalSeconds,
                TokenType = "dummy",
                Scope = "dummy_scope"
            });

        await ServiceTestHarness<DistributedCachingAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseCustomProvider(mockAccessTokenProvider.Object)
                        .WithDistributedCaching();
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var firstResponse = await provider.GetAccessTokenAsync();

            var secondResponse = await provider.GetAccessTokenAsync();

            // Assert
            firstResponse.AccessToken.Should().Be(secondResponse.AccessToken);

            mockAccessTokenProvider.Verify(x => x.GetAccessTokenAsync(It.IsAny<string[]>()), Times.Once);
        }
    }

    [Fact]
    public async Task Expired_access_token_responses_are_refreshed()
    {
        // Arrange
        var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();

        var tokenExpiryWindow = TimeSpan.FromSeconds(1);

        mockAccessTokenProvider
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<string[]>()))
            .ReturnsAsync(() => new AccessTokenResponse
            {
                AccessToken = Guid.NewGuid().ToString(), // Repeated calls will generate different values
                ExpiresIn = (int) tokenExpiryWindow.TotalSeconds,
                TokenType = "dummy",
                Scope = "dummy_scope"
            });

        await ServiceTestHarness<DistributedCachingAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseCustomProvider(mockAccessTokenProvider.Object)
                        .WithDistributedCaching(opts =>
                        {
                            opts.ExpirationBuffer = tokenExpiryWindow / 2;
                        });
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var firstResponse = await provider.GetAccessTokenAsync();

            await Task.Delay(tokenExpiryWindow);

            var secondResponse = await provider.GetAccessTokenAsync();

            // Assert
            firstResponse.AccessToken.Should().NotBe(secondResponse.AccessToken);

            mockAccessTokenProvider.Verify(x => x.GetAccessTokenAsync(It.IsAny<string[]>()), Times.Exactly(2));
        }
    }

    [Fact]
    public async Task Throws_exception_if_expiration_buffer_is_greater_than_token_expiry()
    {
        // Arrange
        var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();

        var tokenExpiryWindow = TimeSpan.FromSeconds(1);

        mockAccessTokenProvider
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<string[]>()))
            .ReturnsAsync(() => new AccessTokenResponse { ExpiresIn = (int) tokenExpiryWindow.TotalSeconds });

        await ServiceTestHarness<DistributedCachingAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseCustomProvider(mockAccessTokenProvider.Object)
                        .WithDistributedCaching(opts =>
                        {
                            opts.ExpirationBuffer = tokenExpiryWindow.Add(TimeSpan.FromTicks(1));
                        });
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var action = async () => await provider.GetAccessTokenAsync();

            // Assert
            await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}
