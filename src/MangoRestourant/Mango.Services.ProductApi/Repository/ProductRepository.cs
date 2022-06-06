using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;
using Mango.Services.ProductApi.DbContexts;
using Mango.Services.ProductApi.Model;
using Mango.Services.ProductApi.Model.Dto;
using Microsoft.EntityFrameworkCore;


namespace Mango.Services.ProductApi.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;

        public ProductRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            var product = mapper.Map<Product>(productDto);

            if (product.ProductId > 0)
                dbContext.Products.Update(product);
            else
                dbContext.Products.Add(product);

            await dbContext.SaveChangesAsync();

            return mapper.Map<ProductDto>(product);
        }

        public async  Task<bool> DeleteProduct(int productId)
        {
            try
            {
                var product = await dbContext.Products.Where(x => x.ProductId == productId).FirstOrDefaultAsync(); ;

                if (product == null)
                    return false;

                dbContext.Products.Remove(product);
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            var product = await dbContext.Products.Where(x => x.ProductId == productId).FirstOrDefaultAsync();

            return mapper.Map<ProductDto>(product);
        }

        public async  Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await dbContext.Products.ToListAsync();

            return mapper.Map<List<ProductDto>>(products);
        }
    }
}
