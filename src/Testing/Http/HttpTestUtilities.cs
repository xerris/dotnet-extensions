using System.Net;
using Moq;
using Moq.Protected;

namespace Xerris.Extensions.Testing.Http;

public static class HttpTestUtilities
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
                ItExpr.IsAny<CancellationToken>())
            .Callback(requestCallback ?? ((_, _) => { /* no-op */ }))
            .ReturnsAsync(mockResponse)
            .Verifiable();

        return handlerMock;
    }

    public class DelegatingHandlerStub : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

        public DelegatingHandlerStub()
        {
            _handlerFunc = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

        public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }
    }
}
