using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Dtos.Consulta;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DadosPublicosMicroAPI.Controllers;

[ApiController]
[Route("api/dados-publicos")]
public class DadosPublicosController : ControllerBase
{
    private readonly IDadosPublicosService _dadosPublicosService;
    
    public DadosPublicosController(IDadosPublicosService dadosPublicosService)
    {
        _dadosPublicosService = dadosPublicosService;
    }

    [HttpPost("consultar")]
    public async Task<IActionResult> NovaPesquisa(ConsultaOnlineDto consulta)
    {
        var result = await _dadosPublicosService.GetDadosPrincipaisAsync(consulta.documento);
        return Ok(result);
        //return Ok(ObjectId.GenerateNewId().ToString());
    }
    
    [HttpGet("historico/lote")]
    public async Task<Pagina<DadosHistoricoLote>> HistoricoPesquisasLote([FromQuery]int pageNumber, [FromQuery]int pageSize)
    {
        var result = await _dadosPublicosService.GetHistoricoLote(pageNumber, pageSize);
        return result;
    }
    
    
    [HttpGet("historico")]
    public async Task<Pagina<DadosHistorico>> HistoricoPesquisa([FromQuery]int pageNumber, [FromQuery]int pageSize, [FromQuery]string? client, [FromQuery]string? document)
    {
        var result = await _dadosPublicosService.GetHistoricoPesquisa(pageNumber, pageSize, client, document);
        return result;
    }
    
    [HttpGet("visualizar")]
    public async Task<ResultData> VisualizarPesquisa([FromQuery]string id)
    {
        var result = await _dadosPublicosService.GetPesquisa(id);
        return result;
    }
    
}

#region ALPHA 0.0.1

/*[HttpPost]
public async Task<IActionResult> NovaPesquisaOnline(ConsultaOnlineDto consulta)
{
    var resposta = new Resposta<ConsultaOnlineDto>();
    resposta.Documento = consulta.documento;
    _mongoConsultaOnline.CreateAsync(resposta);
    return Ok(consulta);
}
    
[HttpGet]
public async Task<Resposta<ConsultaOnlineDto>> BuscarPesquisaOnline([FromQuery] string documento)
{
    return await _mongoConsultaOnline.GetLastAsync(documento);
}*/

#endregion