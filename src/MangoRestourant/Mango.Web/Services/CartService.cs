using Mango.Web.Models;

using System.Net.Http;
using System.Threading.Tasks;

using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class CartService : BaseService, ICartService
    {
        private IHttpClientFactory httpClient;

        public CartService(IHttpClientFactory httpClient) : base(httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShopingCartApiBase}/api/cart/AddCart",
                AccessToken = token
            });
        }

        public async Task<T> GetCartByUserAsync<T>(string userId, string token = null)
        {

            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.ShopingCartApiBase}/api/cart/GetCart/{userId}",
                AccessToken = token
            });
        }

        public async Task<T> RemoveCartAsync<T>(int cartId, string token = null)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartId,
                Url = $"{SD.ShopingCartApiBase}/api/cart/RemoveCart",
                AccessToken = token
            });
        }

        public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShopingCartApiBase}/api/cart/UpdateCart",
                AccessToken = token
            });
        }

        public async Task<T> CheckoutAsync<T>(CartHeaderDto cartHeaderDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartHeaderDto,
                Url = $"{SD.ShopingCartApiBase}/api/cart/Checkout",
                AccessToken = token
            });
        }

        public async Task<T> ApplyCouponAsync<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShopingCartApiBase}/api/cart/ApplyCoupon",
                AccessToken = token
            });
        }

        public async Task<T> RemoveCouponAsync<T>(string userId, string token = null)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = userId,
                Url = $"{SD.ShopingCartApiBase}/api/cart/RemoveCoupon",
                AccessToken = token
            });
        }

    }
}
