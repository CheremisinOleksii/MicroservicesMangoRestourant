using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumerEmail:IAzureServiceBusConsumerEmail
    {
 
        private readonly string subscriptionName;
    
        private readonly string serviceBusConnectionString;
        private readonly string orderUpdatePaymentResultTopic;


        private readonly EmailRepository emailRepository;

        private readonly ServiceBusProcessor orderUpdatePaymentProcessor;


        public AzureServiceBusConsumerEmail(EmailRepository emailRepository, IConfiguration configuration)
        {
            this.emailRepository = emailRepository;
            this.serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            this.subscriptionName = configuration.GetValue<string>("SubscriptionName");
            this.orderUpdatePaymentResultTopic = configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);

            orderUpdatePaymentProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionName);   
        }

        public async Task Start() {
          
            orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            orderUpdatePaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await orderUpdatePaymentProcessor.StopProcessingAsync();
            await orderUpdatePaymentProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args) {

            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage resultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            try
            {
                await emailRepository.SendAndLogEmail(resultMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
