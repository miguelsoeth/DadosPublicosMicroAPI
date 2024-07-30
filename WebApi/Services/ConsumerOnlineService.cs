using System.Diagnostics;
using Application.Dtos;
using Application.Dtos.Consulta;
using Application.Interfaces;
using MassTransit;
using Newtonsoft.Json.Linq;

namespace WebApi.Services;

public class ConsumerOnlineService : IConsumer<ConsultaOnlineDto>
{
    private readonly IMongoRepository<ConsultaResponseDto> _mongoConsulta;
    private readonly IDadosPublicosService _dadosPublicosService;

    public ConsumerOnlineService(IDadosPublicosService dadosPublicosService,
        IMongoRepository<ConsultaResponseDto> mongoConsulta)
    {
        _dadosPublicosService = dadosPublicosService;
        _mongoConsulta = mongoConsulta;
    }

    public async Task Consume(ConsumeContext<ConsultaOnlineDto> context)
    {
        DateTime dataInicio = DateTime.UtcNow;
        var result = await _dadosPublicosService.GetDadosPrincipaisAsync(context.Message.documento);


        if (result.Errors is JArray jArray)
        {
            result.Errors = jArray.ToObject<List<string>>();
        }

        var random = new Random();
        await Task.Delay(random.Next(8) * 1000);

        if (result.Success || context.Message.lote != null)
        {
            DateTime dataFinal = DateTime.UtcNow;
            var resposta = new Resposta<ConsultaResponseDto>
            {
                _id = null,
                perfil = context.Message.perfil,
                usuario = context.Message.usuario,
                usuarioId = context.Message.usuarioId,
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