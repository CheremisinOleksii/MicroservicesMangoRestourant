using System;
using System.Linq;
using System.Threading.Tasks;

using Mango.Web.Models;
using Mango.Web.Services.IServices;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;


namespace Mango.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IProductService productService;
        private readonly ICouponService couponService;
        
        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            this.productService = productService;
            this.cartService = cartService;
            this.couponService = couponService;
        }

        public async Task<IActionResult> CartIndex()
        {
            return View( await LoadCartDtoBaseOnLoggedInUser());
        }

        public async Task<IActionResult> Checkout(int cartDetailsId)
        {
            return View(await LoadCartDtoBaseOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try {

                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await cartService.CheckoutAsync<ResponseDto>(cartDto.CartHeader, accessToken);
                if (!response.IsSuccess) {

                    TempData["Error"] = response.DisplayMessage;
                    return RedirectToAction(nameof(Checkout));
                }                
                
                return RedirectToAction(nameof(Confirmation));

            } catch {

                return View(cartDto);
            }
            
        }


        public async Task<IActionResult> Confirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await cartService.ApplyCouponAsync<ResponseDto>(cartDto, accessToken);

            if (response != null && response.IsSuccess)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await cartService.RemoveCouponAsync<ResponseDto>(cartDto.CartHeader.UserId, accessToken);

            if (response != null && response.IsSuccess)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await cartService.RemoveCartAsync<ResponseDto>(cartDetailsId, accessToken);

            if (response != null && response.IsSuccess)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        private async Task<CartDto> LoadCartDtoBaseOnLoggedInUser()
        {

            var userId = User.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await cartService.GetCartByUserAsync<ResponseDto>(userId, accessToken);

            CartDto cartDto = new();

            if (response != null && response.IsSuccess)
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));

            if (cartDto?.CartHeader == null)
          
                return cartDto;

            if (cartDto?.CartHeader?.CouponCode != null)
            {
                var coupon = await couponService.GetCoupon<ResponseDto>(cartDto?.CartHeader?.CouponCode, accessToken);

                if (coupon != null && response.IsSuccess)
                {
                    var couponObj = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(coupon.Result));
                    cartDto.CartHeader.DiscountTotal = couponObj.DiscountAmount;
                }
            }

            foreach (var detail in cartDto.CartDetails)
                cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);

            cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;

            return cartDto;
        }
    }
}
