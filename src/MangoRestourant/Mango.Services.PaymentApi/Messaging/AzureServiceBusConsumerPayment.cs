using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.PaymentProcessor;
using Mango.Services.PaymentApi.Messages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Mango.Services.PaymentApi.Messaging
{
    public class AzureServiceBusConsumerPayment : IAzureServiceBusConsumer
    {
        private readonly string paymentMessageTopic;
        private readonly string paymentSubscriptionName;
        private readonly string orderPaymentMessageTopic;
        private readonly string orderUpdatePaymentResultTopic;
        private readonly string serviceBusConnectionString;


        private readonly ServiceBusProcessor orderPaymentProcessor;
        private readonly IPaymentProcessor paymentProcessor;
        private readonly IMessageBus messageBus;

        public AzureServiceBusConsumerPayment(IPaymentProcessor paymentProcessor, IConfiguration configuration)
        {
            this.paymentProcessor = paymentProcessor;
            
            this.serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            this.paymentSubscriptionName = configuration.GetValue<string>("PaymenySubscriptionName");
            this.orderPaymentMessageTopic = configuration.GetValue<string>("OrderPaymentProcessTopic");
            this.orderUpdatePaymentResultTopic = configuration.GetValue<string>("OrderUpdatePaymentResultTopic");


            var client = new ServiceBusClient(serviceBusConnectionString);
            orderPaymentProcessor = client.CreateProcessor(paymentMessageTopic, paymentSubscriptionName);

        }

        public async Task Start()
        {
            orderPaymentProcessor.ProcessMessageAsync += ProcessPayment;
            orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderPaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await orderPaymentProcessor.StopProcessingAsync();
            await orderPaymentProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        public async Task ProcessPayment(ProcessMessageEventArgs args)
        {

            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

            var result = await paymentProcessor.ProcessPayment();

            UpdatePaymentResultMessage updatePaymentResultMessage = new UpdatePaymentResultMessage()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId
            };

            try
            {
                await messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
