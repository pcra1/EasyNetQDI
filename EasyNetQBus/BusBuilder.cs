using Common;
using EasyNetQ;
using EasyNetQ.Logging;
namespace EasyNetQBus
{
    public static class BusBuilder
    {
        public static IBus CreateMessageBus(RabbitMqConfiguration configuration)
        {
            var connectionString = "host=" + configuration.Host + ";virtualHost=" + configuration.VirtualHost +
                                   ";username=" + configuration.Username + ";password=" + configuration.Password;
            LogProvider.IsDisabled = true;
            return RabbitHutch.CreateBus(connectionString,
                serviceRegister => serviceRegister.Register<ISerializer>(_ => new ProtoMessageSerializer()));
        }
        
    }
}