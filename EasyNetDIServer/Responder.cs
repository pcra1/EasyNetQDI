using System;
using System.Threading.Tasks;
using EasyNetQDI.Protobuf;
using Microsoft.Extensions.Logging;

namespace EasyNetDIServer
{
    public class Responder
    {
        private ILogger<Responder> _logger;

        public Responder(ILogger<Responder> logger)
        {
            _logger = logger;
        }

        public async Task<TestMessageResponse> ProcessTestMessage(TestMessageRequest request)
        {
            var response = new TestMessageResponse
            {
                Item1 = request.Item1,
                Item2 = request.Item2,
                Item3 = request.Item3,
                Item4 = request.Item4
            };
            await Task.Delay(new TimeSpan(0,0,0, 0, 500));
            return response;
        }
    }
}