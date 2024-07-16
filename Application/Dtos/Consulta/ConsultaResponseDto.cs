namespace Application.Dtos.Consulta;

public class ConsultaResponseDto
{
    public ResultData Data { get; set; }
    public bool Success { get; set; }
    public object Errors { get; set; }
    public string Message { get; set; }
}

public class ResultData
{
    public string Cnpj { get; set; }
    public string RazaoSocial { get; set; }
    public string Uf { get; set; }
    public string Municipio { get; set; }
}