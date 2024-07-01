using System.Net;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.Dtos;

public class Resposta<T> where T : class
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    public string? Mensagem { get; set; }
    public bool DadosHistorico { get; set; } = false;
    public DateTime Date { get; set; } = DateTime.Now;
    public string Documento { get; set; }
    public T? DadosRetorno { get; set; }
}