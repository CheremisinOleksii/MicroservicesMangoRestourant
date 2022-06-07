using Mango.MessageBus;
using Mango.Services.ShoppingCartApi.Messages;
using Mango.Services.ShoppingCartApi.Model.Dto;
using Mango.Services.ShoppingCartApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class ShoppingCartController : Controller
    {
        private readonly ICartRepository repository;
        private readonly ICouponRepository couponRepository;
        private readonly IMessageBus messageBus;
        protected ResponseDto response;

        public ShoppingCartController(ICartRepository repository, IMessageBus messageBus, ICouponRepository couponRepository)
        {
            this.repository = repository;
            this.messageBus = messageBus;
            this.response = new ResponseDto();
            this.couponRepository = couponRepository;   
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try {
                CartDto cartDto = await repository.GetCartByUserId(userId);
                response.Result = cartDto;
            } 
            catch(Exception ex) {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };           
            }

            return response;
        }

        [HttpPost("AddCart")]
        public async Task<ResponseDto> AddCart([FromBody]CartDto cartDto)
        {
            try
            {
                CartDto cart = await repository.CreateUpdateCart(cartDto);
                response.Result = cart;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        [HttpPost("UpdateCart")]
        public async Task<ResponseDto> UpdateCart([FromBody] CartDto cartDto)
        {
            try
            {
                CartDto cart = await repository.CreateUpdateCart(cartDto);
                response.Result = cart;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartId)
        {
            try
            {
               bool isSuccess = await repository.RemoveFromCart(cartId);
                response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                bool isSuccess = await repository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] string userId )
        {
            try
            {
                bool isSuccess = await repository.RemoveCoupon(userId);
                response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDto cartDto = await repository.GetCartByUserId(checkoutHeader.UserId);

                if (cartDto == null)
                    return BadRequest();

                if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                {
                    CouponDto coupon = await couponRepository.GetCoupon(checkoutHeader.CouponCode);

                    if (checkoutHeader.DiscountTotal != coupon.DiscountAmount) {
                        response.IsSuccess = true;
                        response.ErrorMessages = new List<string>() {"Coupon price has changed, please confirm." };
                        response.DisplayMessage = "Coupon price has changed, please confirm.";
                        return response;
                    }
                }

                checkoutHeader.CartDetails = cartDto.CartDetails.ToList();

                //add logic for message queue
                messageBus.PublishMessage(checkoutHeader, "checkoutMessageTopic");

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
