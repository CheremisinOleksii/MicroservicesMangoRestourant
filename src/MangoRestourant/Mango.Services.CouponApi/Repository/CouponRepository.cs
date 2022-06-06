using AutoMapper;

using Mango.Services.CouponApi.DbContexts;
using Mango.Services.CouponApi.Model.Dto;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Mango.Services.CouponApi.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;

        public CouponRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var couponFromDb = await dbContext.Coupons.FirstOrDefaultAsync(c=>c.CouponCode == couponCode);

            return mapper.Map<CouponDto>(couponFromDb);
        }
    }
}
