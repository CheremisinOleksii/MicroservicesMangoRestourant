using Mango.Web.Models;
using System.Threading.Tasks;

namespace Mango.Web.Services.IServices
{
    public interface IProductService
    {
        Task<T> GetAllProductsAsync<T>(string token);
        Task<T> GetProductByIdAsync<T>(int id, string token);
        Task<T> CreteProductAsync<T>(ProductDto product, string token);
        Task<T> UpdateProductAsync<T>(ProductDto product, string token);
        Task<T> DeleteProductAsync<T>(int id, string token);


    }
}
