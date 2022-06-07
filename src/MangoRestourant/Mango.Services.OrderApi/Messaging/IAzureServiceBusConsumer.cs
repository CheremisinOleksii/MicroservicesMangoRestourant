using System.Threading.Tasks;

namespace Mango.Services.OrderApi.Messaging
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
