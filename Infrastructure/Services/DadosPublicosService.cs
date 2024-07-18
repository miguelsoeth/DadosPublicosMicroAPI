using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Dtos.Consulta;
using Application.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;

public class DadosPublicosService : IDadosPublicosService
{
    private readonly IMongoRepository<ConsultaResponseDto> _mongoConsulta;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string _token;

    public DadosPublicosService(IMongoRepository<ConsultaResponseDto> mongoConsulta)
    {
        _mongoConsulta = mongoConsulta;
        _httpClient = new HttpClient();
        _baseUrl = "https://qa-dados-publicos.deps.com.br/api/v1";
    }

    public async Task<bool> LoginAsync()
    {
        var loginUrl = "https://qa-dados-publicos.deps.com.br/api/auth/login?api-version=1.0";
        var loginData = new
        {
            username = "deps",
            password = "deps"
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(loginUrl, content);
        Console.WriteLine("ENVIADO REQUISIÇÃO PARA ENDPOINT DE LOGIN");

        if (response.IsSuccessStatusCode)
        {
            var responseData = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
            _token = responseData.Data.Token;
            return true;
        }

        return false;
    }

    public async Task<ConsultaResponseDto> GetDadosPrincipaisAsync(string documento)
    {
        if (string.IsNullOrEmpty(_token))
        {
            if (!await LoginAsync())
            {
                throw new Exception("Unable to login.");
            }
        }

        var requestUrl = $"{_baseUrl}/pessoas/dados-principais?documento={documento}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var response = await _httpClient.GetAsync(requestUrl);
        
        return JsonConvert.DeserializeObject<ConsultaResponseDto>(await response.Content.ReadAsStringAsync());
        
    }
    
    public async Task<Pagina<DadosHistoricoLote>> GetHistoricoLote(int pageNumber, int pageSize)
    {
        var totalCount = await _mongoConsulta.GetTotalBatchCountAsync();
        
        var results = await _mongoConsulta.GetHistoricoLoteAsync(pageNumber, pageSize);

        return new Pagina<DadosHistoricoLote>
        {
            Items = results,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Pagina<DadosHistorico>> GetHistoricoPesquisa(int pageNumber, int pageSize, string usuarioFilter,
        string cnpjFilter, string? userId)
    {
        var totalCount = await _mongoConsulta.GetTotalCountAsync(usuarioFilter, cnpjFilter, userId);
        
        var results = await _mongoConsulta.GetHistoricoPesquisa(pageNumber, pageSize, usuarioFilter, cnpjFilter, userId);

        return new Pagina<DadosHistorico>
        {
            Items = results,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ResultData> GetPesquisa(string id)
    {
        var results = await _mongoConsulta.GetPesquisa(id);

        return results.DadosRetorno.Data;
    }
}
