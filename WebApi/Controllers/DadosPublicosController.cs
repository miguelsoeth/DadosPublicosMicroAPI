using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.Dtos;
using Application.Dtos.Consulta;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DadosPublicosMicroAPI.Controllers;

[ApiController]
[Authorize]
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
    }

    [HttpGet("historico/lote")]
    public async Task<Pagina<DadosHistoricoLote>> HistoricoPesquisasLote([FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        Pagina<DadosHistoricoLote> result;

        if (roles.Contains("User"))
        {
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            result = await _dadosPublicosService.GetHistoricoLote(pageNumber, pageSize, userId);
        }
        else
        {
            result = await _dadosPublicosService.GetHistoricoLote(pageNumber, pageSize, null);
        }

        return result;
    }


    [HttpGet("historico")]
    public async Task<Pagina<DadosHistorico>> HistoricoPesquisa([FromQuery] int pageNumber, [FromQuery] int pageSize,
        [FromQuery] string? client, [FromQuery] string? document)
    {
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        Pagina<DadosHistorico> result;
        if (roles.Contains("User"))
        {
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            result = await _dadosPublicosService.GetHistoricoPesquisa(pageNumber, pageSize, null, document, userId);
        }
        else
        {
            result = await _dadosPublicosService.GetHistoricoPesquisa(pageNumber, pageSize, client, document, null);
        }

        return result;
    }

    [HttpGet("visualizar")]
    public async Task<ConsultaResponseDto> VisualizarPesquisa([FromQuery] string id)
    {
        var result = await _dadosPublicosService.GetPesquisa(id);
        return result;
    }
}