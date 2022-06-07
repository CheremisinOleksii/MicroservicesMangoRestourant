using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Mango.Services.OrderApi.Model;
using Mango.Services.OrderApi.DbContexts;

namespace Mango.Services.OrderApi.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private DbContextOptions<ApplicationDbContext> dbContext;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext )
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var db = new ApplicationDbContext(dbContext);
            db.OrderHeaders.Add(orderHeader);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid)
        {
            await using var db = new ApplicationDbContext(dbContext);

            var orderHeaderFromDb = await db.OrderHeaders.FirstOrDefaultAsync(h => h.OrderHeaderId == orderHeaderId);

            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.PaymentStatus = isPaid;
                await db.SaveChangesAsync();
            }
        }
    }
}
