using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Model;
using Mango.Services.OrderApi.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Mango.Services.OrderApi.Messaging
{
    public class AzureServiceBusConsumerOrder:IAzureServiceBusConsumer
    {
        private readonly string checkoutMessageTopic;
        private readonly string checkoutSubscriptionName;
        private readonly string checkoutPaymentMessageTopic;
        private readonly string serviceBusConnectionString;
        private readonly string orderUpdatePaymentResultTopic;


        private readonly OrderRepository orderRepository;

        private readonly ServiceBusProcessor serviceBusProcessor;
        private readonly ServiceBusProcessor orderUpdatePaymentProcessor;
        private readonly IMessageBus messageBus;

        public AzureServiceBusConsumerOrder(OrderRepository orderRepository, IConfiguration configuration)
        {
            this.orderRepository = orderRepository;
            this.serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            this.checkoutSubscriptionName = configuration.GetValue<string>("CheckoutSubscriptionName");
            this.checkoutMessageTopic = configuration.GetValue<string>("CheckoutMessageTopic");
            this.checkoutPaymentMessageTopic = configuration.GetValue<string>("OrderPaymentProcessTopic");
            this.orderUpdatePaymentResultTopic = configuration.GetValue<string>("OrderUpdatePaymentResultTopic");


            var client = new ServiceBusClient(serviceBusConnectionString);
            
            serviceBusProcessor = client.CreateProcessor(checkoutMessageTopic, checkoutSubscriptionName);
            orderUpdatePaymentProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, checkoutSubscriptionName);    

        }

        public async Task Start() {
            serviceBusProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            serviceBusProcessor.ProcessErrorAsync += ErrorHandler;
            await serviceBusProcessor.StartProcessingAsync();

            orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            orderUpdatePaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await serviceBusProcessor.StopProcessingAsync();
            await serviceBusProcessor.DisposeAsync();

            await orderUpdatePaymentProcessor.StopProcessingAsync();
            await orderUpdatePaymentProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args) { 
            
            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                Phone = checkoutHeaderDto.Phone,
                Email = checkoutHeaderDto.Email,
                OrderDetails = new List<OrderDetails>(),
                CouponCode = checkoutHeaderDto.CouponCode,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                CVV = checkoutHeaderDto.CVV,
                ExpirityMonthYear = checkoutHeaderDto.ExpirityMonthYear,
                CardNumber = checkoutHeaderDto.CardNumber,
                OrderTime = System.DateTime.Now,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                PaymentStatus = false,
                PickUpDate = checkoutHeaderDto.PickUpDate
            };

            foreach (var item in checkoutHeaderDto.OrderDetails)
            {
                OrderDetails orderDetails = new()
                {
                    Count = item.Count,
                    Price = item.Product.Price,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name                    
                };

                orderHeader.CartTotalItems += item.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            await orderRepository.AddOrder(orderHeader);

            PaymentRequestMessage requestMessage = new()
            {
                Name = orderHeader.FirstName + " " + orderHeader.LastName,
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpirityMonthYear = orderHeader.ExpirityMonthYear,
                OrderId = orderHeader.OrderHeaderId,
                Email = orderHeader.Email
            };

            try
            {
                await messageBus.PublishMessage(requestMessage, checkoutPaymentMessageTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex) {
                throw;
            }
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args) {
            
            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            await orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
