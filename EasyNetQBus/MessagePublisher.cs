using System;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;

namespace EasyNetQBus
{
    public interface IMessagePublisher
    {
        void Publish<T>(T message) where T : class;

        Task PublishAsync<T>(T message) where T : class;

        void PublishWithTopic<T>(T message, string topic) where T : class;

        Task PublishWithTopicAsync<T>(T message, string topic) where T : class;

        TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class;

        Task RequestAsync<TRequest, TResponse>(TRequest request, Action<Task<TResponse>> onResponse)
            where TRequest : class
            where TResponse : class;

        void Response<TRequest, TResponse>(Func<TRequest, TResponse> onResponse)
            where TRequest : class
            where TResponse : class;
    }

    public class MessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;
        private readonly ILogger<MessagePublisher> _logger;

        public MessagePublisher(IBus bus, ILogger<MessagePublisher> logger) 
        {
            _bus = bus;
            _logger = logger;
        }

        public void Publish<T>(T message) where T : class
        {
            try
            {
                //_logger.LogInformation($"Publishing Message: {message}");
                _bus.PubSub.Publish(message);
            }
            catch (EasyNetQException ex)
            {
                _logger.LogError("Publish Message Failed: ", ex);
            }
        }

        public void PublishWithTopic<T>(T message, string topic) where T : class
        {
            try
            {
                //_logger.LogInformation($"Publishing Message: {message}");
                _bus.PubSub.Publish(message, topic);
            }
            catch (EasyNetQException ex)
            {
                _logger.LogError("Publish Message Failed: ", ex);
            }
        }

        public async Task PublishAsync<T>(T message) where T : class
        {
            try
            {
                //_logger.LogInformation($"Publishing Async Message: {message}");
                await _bus.PubSub.PublishAsync(message).ConfigureAwait(false);
            }
            catch (EasyNetQException ex)
            {
                _logger.LogError("PublishAsync Message Failed: ", ex);
            }
        }

        public async Task PublishWithTopicAsync<T>(T message, string topic) where T : class
        {
            try
            {
                //_logger.LogInformation($"Publishing Async Message: {message}");
                await _bus.PubSub.PublishAsync(message, topic).ConfigureAwait(false);
            }
            catch (EasyNetQException ex)
            {
                _logger.LogError("PublishAsync Message Failed: ", ex);
            }
        }


        public TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class

        {
            try
            {
                //_logger.LogInformation("Publishing Request: {0}", request);
                return _bus.Rpc.Request<TRequest, TResponse>(request);
            }
            catch (EasyNetQException ex)
            {
                _logger.LogError("Publish Request Failed: ", ex);
                return null;
            }
        }

        public async Task RequestAsync<TRequest, TResponse>(TRequest request, Action<Task<TResponse>> onResponse)
            where TRequest : class
            where TResponse : class

        {
            try
            {
                //_logger.LogInformation("Publishing Request: {0}", request);
                await _bus.Rpc.RequestAsync<TRequest, TResponse>(request).ContinueWith(onResponse);
            }
            catch (EasyNetQException ex)
            {
                _logger.LogError("Publish Request Failed: ", ex);
            }
        }

        public void Response<TRequest, TResponse>(Func<TRequest, TResponse> onResponse)
            where TRequest : class
            where TResponse : class

        {
            try
            {
                //_logger.LogInformation("Publishing Response: {0}", onResponse);
                _bus.Rpc.Respond(onResponse);
            }
            catch (EasyNetQException ex)
            {
                _logger.LogError("Respond Failed: ", ex);
            }
        }
    }
}