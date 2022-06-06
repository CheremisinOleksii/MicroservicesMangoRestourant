using Mango.Web.Models;
using System.Threading.Tasks;

namespace Mango.Web.Services.IServices
{
    public interface ICartService
    {
        Task<T> GetCartByUserAsync<T>(string userId, string token=null);
        Task<T> AddToCartAsync<T>(CartDto cartDto, string token=null);
        Task<T> UpdateCartAsync<T>(CartDto cartDto, string token=null);
        Task<T> RemoveCartAsync<T>(int cartId, string token = null);
        Task<T> CheckoutAsync<T>(CartHeaderDto cartHeaderDto, string token = null);
        Task<T> ApplyCouponAsync<T>(CartDto cartDto, string token = null);
        Task<T> RemoveCouponAsync<T>(string userId, string token = null);
    }
}
