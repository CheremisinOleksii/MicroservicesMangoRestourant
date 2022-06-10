
using Mango.Services.Email.Messages;
using System.Threading.Tasks;

namespace Mango.Services.Email.Repository
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage resultMessage);
    }
}
