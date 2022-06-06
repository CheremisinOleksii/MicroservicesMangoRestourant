using AutoMapper;
using Mango.Services.CouponApi.Model;
using Mango.Services.CouponApi.Model.Dto;

namespace Mango.Services.CouponApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {

            var mappingConfig = new MapperConfiguration(conf =>
            {
                conf.CreateMap<CouponDto, Coupon>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
