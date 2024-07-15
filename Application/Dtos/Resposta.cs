using System.Net;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.Dtos;

public class  Resposta<T> where T : class
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    public string? perfil { get; set; }
    public string? usuario { get; set; }
    public string? lote { get; set; }
    public int? quantidade { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? dataInicio { get; set; }
    public DateTime? dataFinal { get; set; }
    public T DadosRetorno { get; set; }
}