using MongoDB.Bson;

namespace Application.Dtos.Consulta;

public class ConsultaOnlineDto
{
    public string usuario { get; set; }
    public string usuarioId { get; set; }
    public Guid venda { get; set; }
    public string? lote { get; set; }
    public int? quantidade { get; set; }
    public string documento { get; set; }
    public string perfil { get; set; }
    public DateTime? dataCadastro { get; set; }
}