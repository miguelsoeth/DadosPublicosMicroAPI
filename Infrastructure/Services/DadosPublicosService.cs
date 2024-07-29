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
    private readonly ICacheService _cacheService;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _loginUrl;
    private readonly object _loginData;

    public DadosPublicosService(IMongoRepository<ConsultaResponseDto> mongoConsulta, ICacheService cacheService)
    {
        _mongoConsulta = mongoConsulta;
        _cacheService = cacheService;
        _httpClient = new HttpClient();
        
        _loginData = new
        {
            username = Environment.GetEnvironmentVariable("LOGIN_USER"),
            pwd = Environment.GetEnvironmentVariable("LOGIN_PWD")
        };
        _loginUrl = Environment.GetEnvironmentVariable("LOGIN_URL");
        _baseUrl = Environment.GetEnvironmentVariable("API_URL");
    }

    public async Task<bool> LoginAsync()
    {
        var content = new StringContent(JsonConvert.SerializeObject(_loginData), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_loginUrl, content);
        Console.WriteLine("ENVIADO REQUISIÇÃO PARA ENDPOINT DE LOGIN");

        if (response.IsSuccessStatusCode)
        {
            var responseData = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
            await _cacheService.SetData("login_token", responseData.Data.Token);
            return true;
        }

        return false;
    }

    public async Task<ConsultaResponseDto> GetDadosPrincipaisAsync(string documento)
    {
        //Console.WriteLine("****************BASE URL"+_baseUrl);
        string token = _cacheService.GetData<string>("login_token");
        
        if (string.IsNullOrEmpty(token))
        {
            if (!await LoginAsync())
            {
                throw new Exception("Unable to login.");
            }
            token = _cacheService.GetData<string>("login_token");
        }

        var requestUrl = $"{_baseUrl}/pessoas?documento={documento}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = new HttpResponseMessage();
        try
        {
            response = await _httpClient.GetAsync(requestUrl);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return JsonConvert.DeserializeObject<ConsultaResponseDto>(await response.Content.ReadAsStringAsync());
        
    }
    
    public async Task<Pagina<DadosHistoricoLote>> GetHistoricoLote(int pageNumber, int pageSize, string? userId)
    {
        var totalCount = await _mongoConsulta.GetTotalBatchCountAsync(userId);
        
        var results = await _mongoConsulta.GetHistoricoLoteAsync(pageNumber, pageSize, userId);

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

    public async Task<ConsultaResponseDto> GetPesquisa(string id)
    {
        var results = await _mongoConsulta.GetPesquisa(id);

        return results.DadosRetorno;
    }
}
