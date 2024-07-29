using MassTransit;

namespace Domain.Config;

public static class StartupSetup
{
    public static void AddMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("rabbitmq");
                    h.Password("rabbitmq");
                });

                config.ConfigureEndpoints(provider);
            }));
        });
    }
    
    public static void AddMassTransitConsumer<T>(this IServiceCollection services, string queue1, string queue2, int limit)
        where T : class, IConsumer
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<T>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("rabbitmq");
                    h.Password("rabbitmq");
                });

                cfg.ReceiveEndpoint($"{queue1}", e =>
                {
                    e.ConfigureConsumer<T>(context);
                    e.ConcurrentMessageLimit = limit;
                });
                
                cfg.ReceiveEndpoint($"{queue2}", e =>
                {
                    e.ConfigureConsumer<T>(context);
                    e.ConcurrentMessageLimit = limit;
                });
            });
        });
    }
}