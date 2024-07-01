using Application.Dtos;
using Application.Dtos.Consulta;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DadosPublicosMicroAPI.Controllers;

[ApiController]
[Route("api/dados-publicos")]
public class DadosPublicosController : ControllerBase
{
    private readonly IMongoRepository<ConsultaOnlineDto> _mongoConsultaOnline;
    private readonly IDadosPublicosService _dadosPublicosService;
    
    public DadosPublicosController(IMongoRepository<ConsultaOnlineDto> mongoConsultaOnline, IDadosPublicosService dadosPublicosService)
    {
        _mongoConsultaOnline = mongoConsultaOnline;
        _dadosPublicosService = dadosPublicosService;
    }

    [HttpPost]
    public async Task<IActionResult> NovaPesquisa(ConsultaOnlineDto consulta)
    {
        var result = await _dadosPublicosService.GetDadosPrincipaisAsync(consulta.documento);
        return Ok(result);
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