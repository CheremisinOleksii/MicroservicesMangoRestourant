using System.Net.Http;
using System.Threading.Tasks;

using Mango.Web.Models;
using Mango.Web.Services.IServices;


namespace Mango.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        private IHttpClientFactory httpClient;

        public CouponService(IHttpClientFactory httpClient) : base(httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<T> GetCoupon<T>(string couponCode, string token = null)
        {
            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CouponApiBase}/api/coupon/{couponCode}",
                AccessToken = token
            });
        }
    }
}
