using Mango.Services.OrderApi.Model;

using System.Threading.Tasks;

namespace Mango.Services.OrderApi.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);

        Task UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid);
    }
}
