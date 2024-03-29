using MassTransit;
using RabbitMQ.Client;

namespace UserService.WebApi.Extensions
{
    public static class RabbitMQExtensions
    {
        public static void AddMassTransitWithRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(x => x.UsingRabbitMq());
        }
    }
}