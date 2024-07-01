using Application.Dtos.Consulta;
using Domain.Model;

namespace Application.Interfaces;

public interface IDadosPublicosService
{
    Task<bool> LoginAsync();
    Task<ConsultaResponseDto> GetDadosPrincipaisAsync(string documento);
}
