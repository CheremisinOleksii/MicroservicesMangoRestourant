using AutoMapper;
using Mango.Services.ShoppingCartApi.DbContexts;
using Mango.Services.ShoppingCartApi.Model;
using Mango.Services.ShoppingCartApi.Model.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;

        public CartRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            try
            {
                var cartFromDb = await dbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
                cartFromDb.CouponCode = couponCode;
                dbContext.CartHeaders.Update(cartFromDb);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ClearCart(string userId)
        {
            var dbCartHeader = dbContext.CartHeaders.FirstOrDefault(c => c.UserId == userId);

            if (dbCartHeader != null)
            {
                dbContext.CartDetails.RemoveRange(dbContext.CartDetails.Where(u => u.CartHeaderId == dbCartHeader.CartHeaderId));
                return true;
            }
            return false;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            var cart = mapper.Map<Cart>(cartDto);

            var productInDb = await dbContext.Products.FirstOrDefaultAsync(
                p => cart.CartDetails.FirstOrDefault().ProductId == p.ProductId);

            if (productInDb == null)
            {
                dbContext.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await  dbContext.SaveChangesAsync();
            }

            var dbCartHeader = await dbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
                h => h.UserId == cart.CartHeader.UserId);

            if (dbCartHeader == null)
            {

                dbContext.CartHeaders.Add(cart.CartHeader);
                await dbContext.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await dbContext.SaveChangesAsync();

            }
            else {

                var dbcartDetails  = await dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId
                    && u.CartHeaderId == dbCartHeader.CartHeaderId);

                if (dbcartDetails == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = dbCartHeader.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += dbcartDetails.Count;
                    dbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                   await dbContext.SaveChangesAsync();
                }


            }

            return mapper.Map<CartDto>(cart);

        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new() {
                CartHeader = await dbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId)
            };

            cart.CartDetails = dbContext.CartDetails
                .Where(cd => cd.CartHeaderId == cart.CartHeader.CartHeaderId).Include(u => u.Product);

            return mapper.Map<CartDto>(cart);

        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartFromDb = await dbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = string.Empty;
            dbContext.CartHeaders.Update(cartFromDb);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                var cartDetails = await dbContext.CartDetails
                       .FirstOrDefaultAsync(cd => cd.CartDetailsId == cartDetailsId);

                var totalCount = dbContext.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();

                dbContext.CartDetails.Remove(cartDetails);

                if (totalCount == 1)
                {
                    var cartHeaderToRemove = await dbContext.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    dbContext.CartHeaders.Remove(cartHeaderToRemove);
                }

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
