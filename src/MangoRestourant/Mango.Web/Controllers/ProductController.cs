using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        public IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new();

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await productService.GetAllProductsAsync<ResponseDto>(accessToken);

            if (response?.Result != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));

            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await productService.CreteProductAsync<ResponseDto>(model, accessToken);

                if (response != null && response.IsSuccess)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        public async Task<IActionResult> ProductEdit(int productId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await productService.GetProductByIdAsync<ResponseDto>(productId,accessToken);

            if (response != null && response.IsSuccess)
            {

                var model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ProductEdit(ProductDto model)
        {
            if (ModelState.IsValid)
            {

                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await productService.UpdateProductAsync<ResponseDto>(model,accessToken);

                if (response != null && response.IsSuccess)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }


        public async Task<IActionResult> ProductDelete(int productId)
        {

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await productService.GetProductByIdAsync<ResponseDto>(productId,accessToken);

            if (response != null && response.IsSuccess)
            {

                var model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await productService.DeleteProductAsync<ResponseDto>(model.ProductId,accessToken);

            return RedirectToAction(nameof(ProductIndex));

        }
    }
}
