using System;
using System.Threading.Tasks;

namespace Mango.PaymentProcessor
{
    public interface IPaymentProcessor {
        Task<bool> ProcessPayment();
    }

    public class PaymentProcessor : IPaymentProcessor
    {
        public async Task<bool> ProcessPayment()
        {
            return true;
        }
    }
}
