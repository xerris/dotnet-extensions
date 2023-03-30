using Moq;
using Moq.Protected;

namespace Xerris.Extensions.Http.Tests;

internal static class TestUtilities
{
    public static Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpResponseMessage mockResponse,
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
            .ReturnsAsync(mockResponse)
            .Verifiable();

        return handlerMock;
    }
}
