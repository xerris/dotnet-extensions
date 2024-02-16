using Moq;
using Moq.Protected;

namespace Xerris.Extensions.Testing.Http;

/// <summary>
/// Utility methods and classes for mocking and capturing HTTP responses.
/// </summary>
public static class HttpTestUtilities
{
    /// <summary>
    /// Creates a mock <see cref="HttpMessageHandler" /> that returns a supplied <see cref="HttpResponseMessage" />.
    /// </summary>
    /// <param name="mockResponse">The expected result of any HTTP request that passes through this handler.</param>
    /// <param name="requestCallback">
    /// A callback to invoke when the <see cref="HttpMessageHandler" /> handles a request.
    /// </param>
    /// <returns>The <see cref="HttpMessageHandler" /> mock.</returns>
    public static Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpResponseMessage mockResponse,
        Action<HttpRequestMessage, CancellationToken>? requestCallback = null)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                nameof(HttpClient.SendAsync),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback(requestCallback ?? ((_, _) =>
            {
                /* no-op */
            }))
            .ReturnsAsync(mockResponse)
            .Verifiable();

        return handlerMock;
    }

    /// <summary>
    /// A <see cref="DelegatingHandler" /> that invokes a specified function when it handles a
    /// <see cref="HttpRequestMessage" />.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of this class.
    /// </remarks>
    /// <param name="handlerFunc">The function to invoke when handling <see cref="HttpRequestMessage" /></param>
    public class DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc = handlerFunc;

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }
    }
}
