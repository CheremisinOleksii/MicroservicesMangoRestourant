using Mango.Services.Email.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mango.Services.Email.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IAzureServiceBusConsumerEmail ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder builder) {

            ServiceBusConsumer = builder.ApplicationServices.GetService<IAzureServiceBusConsumerEmail>();


            var hostApplicationLife = builder.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopped.Register(OnStop);

            return builder;
        }

        private static void OnStart() {
            ServiceBusConsumer.Start();
        }

        private static void OnStop() {
            ServiceBusConsumer.Stop();
        }
    }
}
