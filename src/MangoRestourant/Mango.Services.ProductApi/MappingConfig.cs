using AutoMapper;
using Mango.Services.ProductApi.Model;
using Mango.Services.ProductApi.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ProductApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps() {
            
            var mappingConfig = new MapperConfiguration(conf => {
                conf.CreateMap<ProductDto, Product>();
                conf.CreateMap<Product, ProductDto>();
            });
            
            return mappingConfig;
        }
    }
}
