using MassTransit;
using UserService.Infrastructure.Consumers;
using UserService.Infrastructure.Options;

namespace UserService.WebApi.Extensions
{
    public static class RabbitMQExtensions
    {
        public static void AddMassTransitWithRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new RabbitMQOptions();
            configuration.GetSection(nameof(RabbitMQOptions)).Bind(options);
            services.AddMassTransit(x =>
            {
                x.AddConsumer<BotsCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(options.Host,h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    cfg.ReceiveEndpoint("BotsCreatedQueue", e => e.ConfigureConsumer<BotsCreatedConsumer>(context));
                });
            });
        }
    }
}