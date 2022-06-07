using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        private readonly string connectionString = "connection string";

        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            ServiceBusClient client = new(connectionString);

            var sender = client.CreateSender(topicName);

            var jsonMessage = JsonConvert.SerializeObject(message);
            
            var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);

            await sender.DisposeAsync();
        }
    }
}