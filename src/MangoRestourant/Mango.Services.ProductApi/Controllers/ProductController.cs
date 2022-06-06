using Mango.Services.ProductApi.Model.Dto;
using Mango.Services.ProductApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private ResponseDto response;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
            this.response = new ResponseDto();
        }

        [HttpGet]
       
        public async Task<ResponseDto> Get() {

            try
            {
                IEnumerable<ProductDto> products = await productRepository.GetProducts();
                response.Result = products;

            }
            catch (System.Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.Message };
            }

            return response;
        }


        [HttpGet]
        [Route("{id}")]
        
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                ProductDto product = await productRepository.GetProductById(id);
                response.Result = product;
            }
            catch (System.Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.Message };
            }

            return response;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ResponseDto> Post([FromBody] ProductDto productDto)
        {

            try
            {
                ProductDto model = await productRepository.CreateUpdateProduct(productDto);
                response.Result = model;

            }
            catch (System.Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.Message };
            }

            return response;
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ResponseDto> Put([FromBody] ProductDto productDto)
        {

            try
            {
                ProductDto model = await productRepository.CreateUpdateProduct(productDto);
                response.Result = model;

            }
            catch (System.Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.Message };
            }

            return response;
        }


        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
              response.Result = await productRepository.DeleteProduct(id);

            }
            catch (System.Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.Message };
            }

            return response;
        }


    }
}
