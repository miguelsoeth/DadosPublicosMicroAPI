using Application.Dtos;
using Application.Dtos.Consulta;
using Domain.Model;
using MongoDB.Bson;

namespace Application.Interfaces;

public interface IDadosPublicosService
{
    Task<bool> LoginAsync();
    Task<ConsultaResponseDto> GetDadosPrincipaisAsync(string documento);
    Task<Pagina<DadosHistoricoLote>> GetHistoricoLote(int pageNumber, int pageSize, string? userId);
    Task<Pagina<DadosHistorico>> GetHistoricoPesquisa(int pageNumber, int pageSize, string usuarioFilter,
        string cnpjFilter, string? userId);
    
    Task<ResultData> GetPesquisa(string id);
}
