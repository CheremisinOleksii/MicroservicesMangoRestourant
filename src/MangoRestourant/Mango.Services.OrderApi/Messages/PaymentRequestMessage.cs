﻿using Mango.MessageBus;

namespace Mango.Services.OrderApi.Messages
{
    public class PaymentRequestMessage:BaseMessage
    {
        public int OrderId { get; set; }
        public double OrderTotal { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpirityMonthYear { get; set; }
    }
}