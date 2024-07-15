using System.Diagnostics;
using Application.Dtos;
using Application.Dtos.Consulta;
using Application.Interfaces;
using MassTransit;

namespace WebApi.Services;

public class ConsumerOnlineService : IConsumer<ConsultaOnlineDto>
{
    private readonly IMongoRepository<ConsultaResponseDto> _mongoConsulta;
    private readonly IDadosPublicosService _dadosPublicosService;

    public ConsumerOnlineService(IDadosPublicosService dadosPublicosService, IMongoRepository<ConsultaResponseDto> mongoConsulta)
    {
        _dadosPublicosService = dadosPublicosService;
        _mongoConsulta = mongoConsulta;
    }

    public async Task Consume(ConsumeContext<ConsultaOnlineDto> context)
    {
        DateTime dataInicio = DateTime.UtcNow;
        var result = await _dadosPublicosService.GetDadosPrincipaisAsync(context.Message.documento);
        var random = new Random();
        await Task.Delay(random.Next(5) * 1000);
        Console.WriteLine("Mensagem processada");
        
        if (result.Success)
        {
            DateTime dataFinal = DateTime.UtcNow;
            var resposta = new Resposta<ConsultaResponseDto>
            {
                perfil = context.Message.perfil,
                usuario = context.Message.usuario,
                lote = context.Message.lote,
                quantidade = context.Message.quantidade,
                Date = context.Message.dataCadastro,
                dataInicio = dataInicio,
                dataFinal = dataFinal,
                DadosRetorno = result
            };
    
            await _mongoConsulta.CreateAsync(resposta);
        }

        await context.RespondAsync(result);
    }
}