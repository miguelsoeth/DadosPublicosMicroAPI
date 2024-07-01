namespace Application.Dtos.Consulta;

public class ConsultaOnlineDto
{
    public Guid usuario { get; set; }
    public Guid venda { get; set; }
    public string documento { get; set; }
    public string perfil { get; set; }
}