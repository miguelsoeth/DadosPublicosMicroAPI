using System;
using System.Collections.Generic;

namespace Application.Dtos;
public class DadosHistorico
{
    public string Id { get; set; }
    public string Profile { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Quantity { get; set; }
    public int Concluded { get; set; }
    public string Status { get; set; }
}


