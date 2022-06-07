using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Model;
using Mango.Services.OrderApi.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Mango.Services.OrderApi.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string checkoutSubscriptionName;
        private readonly string checkoutMessageTopic;
        private readonly string serviceBusConnectionString;
        
        private readonly OrderRepository orderRepository;

        private readonly ServiceBusProcessor serviceBusProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration)
        {
            this.orderRepository = orderRepository;
            this.serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            this.checkoutSubscriptionName = configuration.GetValue<string>("SubscriptionName");
            this.checkoutMessageTopic = configuration.GetValue<string>("CheckoutMessageTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            serviceBusProcessor = client.CreateProcessor(checkoutMessageTopic, checkoutSubscriptionName);
       
        }

        public async Task Start() {
            serviceBusProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            serviceBusProcessor.ProcessErrorAsync += ErrorHandler;
            await serviceBusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await serviceBusProcessor.StopProcessingAsync();
            await serviceBusProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        public async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args) { 
            
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
        }
    }
}
