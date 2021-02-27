using System;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyNetQ;
using Google.Protobuf;
using Newtonsoft.Json;
using IMessage = Google.Protobuf.IMessage;

namespace EasyNetQBus
{
    public class ProtoMessageSerializer : ISerializer
    {
        private readonly ITypeNameSerializer _typeNameSerializer = new DefaultTypeNameSerializer();
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };
        
        private Assembly m_protoBufAssembly;

        public ProtoMessageSerializer()
        {
            m_protoBufAssembly = AppDomain.CurrentDomain
                .GetAssemblies().First(a => a.FullName != null && a.FullName.StartsWith("ProtobufMessages"));

        }
        public byte[] MessageToBytes(Type messageType, object message)
        {
            if (message is IMessage iMessage)
            {
                return iMessage.ToByteArray();
            }
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        }

        public object BytesToMessage(Type messageType, byte[] bytes)
        {
            var type = m_protoBufAssembly.GetType(messageType.FullName ?? string.Empty);
            if (type != null)
            {
                var inst = Activator.CreateInstance(type);
                try
                {
                    ((Google.Protobuf.IMessage) inst).MergeFrom(bytes);
                    return inst;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to parse protobuf");
                }

            }

            var name = _typeNameSerializer.DeSerialize(messageType.Name);
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), name, _serializerSettings);
        }
    }
}