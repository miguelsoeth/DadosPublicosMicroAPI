using System.Diagnostics;
using Application.Dtos;
using Application.Dtos.Consulta;
using Application.Interfaces;
using MassTransit;

namespace WebApi.Services;

public class ConsumerOnlineService : IConsumer<ConsultaOnlineDto>
{
    private readonly IMongoRepository<ConsultaResponseDto> _mongoConsultaOnline;
    private readonly IDadosPublicosService _dadosPublicosService;

    public ConsumerOnlineService(IDadosPublicosService dadosPublicosService, IMongoRepository<ConsultaResponseDto> mongoConsultaOnline)
    {
        _dadosPublicosService = dadosPublicosService;
        _mongoConsultaOnline = mongoConsultaOnline;
    }

    public async Task Consume(ConsumeContext<ConsultaOnlineDto> context)
    {
        var result = await _dadosPublicosService.GetDadosPrincipaisAsync(context.Message.documento);
        var random = new Random();
        await Task.Delay(random.Next(5) * 1000);
        Console.WriteLine("Mensagem processada");
        
        if (result.Success)
        {
            var resposta = new Resposta<ConsultaResponseDto>
            {
                DadosRetorno = result
            };
    
            await _mongoConsultaOnline.CreateAsync(resposta);
        }

        await context.RespondAsync(result);
    }
}