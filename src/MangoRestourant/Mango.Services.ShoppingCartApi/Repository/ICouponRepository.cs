using Mango.Services.ShoppingCartApi.Model.Dto;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Repository
{
    public interface ICouponRepository
    {
        public Task<CouponDto> GetCoupon(string couponName);
    }
}
