using Application.Dtos.Consulta;
using Application.Interfaces;
using Domain.Config;
using Infrastructure.Repository;
using WebApi;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddSingleton<IMongoRepository<ConsultaResponseDto>>(new MongoRepository<ConsultaResponseDto>("consultas"));
builder.Services.AddSingleton<IDadosPublicosService, DadosPublicosService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransitConsumer<ConsumerOnlineService>("teste", 10);
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();