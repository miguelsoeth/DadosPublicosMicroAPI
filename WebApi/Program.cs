using Application.Dtos.Consulta;
using Application.Interfaces;
using Domain.Config;
using Infrastructure.Repository;
using WebApi;
using WebApi.Services;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMongoRepository<ConsultaResponseDto>>(new MongoRepository<ConsultaResponseDto>("Online"));
        services.AddSingleton<IDadosPublicosService, DadosPublicosService>();
        services.AddMassTransitConsumer<ConsumerOnlineService>("teste", 10);
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();