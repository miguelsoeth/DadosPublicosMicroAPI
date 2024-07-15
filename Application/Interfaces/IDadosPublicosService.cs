using Application.Dtos;
using Application.Dtos.Consulta;
using Domain.Model;
using MongoDB.Bson;

namespace Application.Interfaces;

public interface IDadosPublicosService
{
    Task<bool> LoginAsync();
    Task<ConsultaResponseDto> GetDadosPrincipaisAsync(string documento);
    Task<Pagina<DadosHistorico>> GetHistorico(int pageNumber, int pageSize);
}
