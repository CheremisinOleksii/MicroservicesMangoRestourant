using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;


namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IProductService productService;
        private readonly ICartService cartService;

        public HomeController(IProductService productService, ICartService cartService, ILogger<HomeController> logger)
        {
            this.productService = productService;
            this.cartService = cartService;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();
            
            var response = await productService.GetAllProductsAsync<ResponseDto>("");

            if (response?.Result != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));

            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto result = new();

            var response = await productService.GetProductByIdAsync<ResponseDto>(productId, "");

            if (response?.Result != null && response.IsSuccess)
                result = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));

            return View(result);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            var cartDto = new CartDto
            {
                CartHeader = new CartHeaderDto { UserId = User.Claims.Where(c => c.Type == "sub")?.FirstOrDefault().Value }
            };

            var cartDetails = new CartDetailsDto
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };

            var resp = await productService.GetProductByIdAsync<ResponseDto>(productDto.ProductId, string.Empty);

            if (resp != null && resp.IsSuccess) {
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
            }

            cartDto.CartDetails = new List<CartDetailsDto> { cartDetails };

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var addToCartResp = await cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken);

            if (addToCartResp != null && addToCartResp.IsSuccess)
                return RedirectToAction(nameof(Index));


            return View(productDto);
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
