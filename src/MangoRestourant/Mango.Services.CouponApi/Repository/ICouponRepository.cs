using Mango.Services.CouponApi.Model.Dto;
using System.Threading.Tasks;

namespace Mango.Services.CouponApi.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
