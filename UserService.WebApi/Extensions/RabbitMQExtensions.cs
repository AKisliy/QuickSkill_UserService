using MassTransit;
using RabbitMQ.Client;
using UserService.Infrastructure.Events;

namespace UserService.WebApi.Extensions
{
    public static class RabbitMQExtensions
    {
        public static void AddMassTransitWithRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(x => x.UsingRabbitMq((context, cfg) => {
                cfg.Message<UserCreatedEvent>(x => x.SetEntityName("UserCreatedExchange"));
                cfg.Publish<UserCreatedEvent>(x => x.ExchangeType = ExchangeType.Fanout);
            }));
        }
    }
}