using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using EasyNetQDI.Protobuf;
using EasyNetQBus;

namespace EasyNetDIServer.Responders
{
    public class TestMessageResponder: EasyNetQBus.IResponder
    {
        private readonly IBus m_bus;
        private readonly ILogger<TestMessageResponder> m_logger;
        private readonly Responder m_responder;

        public TestMessageResponder(Responder responder, ILogger<TestMessageResponder> logger, IBus bus)
        {
            m_responder = responder;
            m_logger = logger;
            m_bus = bus;
        }

        public void SubscribeAsync()
        {
            m_bus.Rpc.RespondAsync<EasyNetQDI.Protobuf.TestMessageRequest, EasyNetQDI.Protobuf.TestMessageResponse>(ResponseAsync);
        }

        private async Task<TestMessageResponse> ResponseAsync(TestMessageRequest request)
        {
            if (request != null)
            {
                m_logger.LogDebug(request.GetType().Name);
                return await m_responder.ProcessTestMessage(request);
            }

            return null;
        }
    }
}