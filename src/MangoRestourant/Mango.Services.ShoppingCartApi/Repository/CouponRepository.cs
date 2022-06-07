using Mango.Services.ShoppingCartApi.Model.Dto;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient client;

        public CouponRepository(HttpClient client)
        {
            this.client = client;

        }

        public  async Task<CouponDto> GetCoupon(string couponName)
        {
            var response = await client.GetAsync($"/api/coupon/{couponName}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var respResult  = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (respResult.IsSuccess)
                return JsonConvert.DeserializeObject<CouponDto>(respResult.Result.ToString());

            return new CouponDto();

        }
    }
}
