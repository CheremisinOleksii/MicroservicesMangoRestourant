using AutoMapper;
using Mango.Services.ShoppingCartApi.Model;
using Mango.Services.ShoppingCartApi.Model.Dto;

namespace Mango.Services.ShoppingCartApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {

            var mappingConfig = new MapperConfiguration(conf =>
            {
                conf.CreateMap<ProductDto, Product>().ReverseMap();
                conf.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                conf.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                conf.CreateMap<Cart, CartDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
