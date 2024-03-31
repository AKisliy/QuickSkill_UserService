using MassTransit;
using UserService.Infrastructure.Consumers;

namespace UserService.WebApi.Extensions
{
    public static class RabbitMQExtensions
    {
        public static void AddMassTransitWithRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<BotsCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://localhost");

                    cfg.ReceiveEndpoint("BotsCreatedQueue", e => e.ConfigureConsumer<BotsCreatedConsumer>(context));
                });
            });
        }
    }
}