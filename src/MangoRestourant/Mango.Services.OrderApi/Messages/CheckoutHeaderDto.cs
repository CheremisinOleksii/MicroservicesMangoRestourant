
using System;
using System.Collections.Generic;


namespace Mango.Services.OrderApi.Messages
{
    public class CheckoutHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        public double OrderTotal { get; set; }
        public double DiscountTotal { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime PickUpDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpirityMonthYear { get; set; }
        public int CardTotalItems { get; set; }
        public List<CartDetailsDto> OrderDetails { get; set; }
    }
}
