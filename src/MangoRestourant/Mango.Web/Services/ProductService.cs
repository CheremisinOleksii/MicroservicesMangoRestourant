using Mango.Web.Models;
using Mango.Web.Services.IServices;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private IHttpClientFactory httpClient;

        public ProductService(IHttpClientFactory httpClient):base(httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<T> CreteProductAsync<T>(ProductDto product, string token)
        {
            return  await SendAsync<T>(new ApiRequest { 
                ApiType = SD.ApiType.POST,
                Data = product,
                Url = $"{SD.ProductApiBase}/api/product",
                AccessToken= token
            });
        }

        public async Task<T> DeleteProductAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.ProductApiBase}/api/product/{id}",
                AccessToken = token
            });
        }

        public async Task<T> GetAllProductsAsync<T>(string token)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.ProductApiBase}/api/product",
                AccessToken = token
            }); ;
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.ProductApiBase}/api/product/{id}",
                AccessToken = token
            });
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto product, string token)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.PUT,
                Data = product,
                Url = $"{SD.ProductApiBase}/api/product",
                AccessToken = token
            });
        }
    }
}
