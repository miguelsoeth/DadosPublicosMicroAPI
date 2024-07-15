namespace Application.Dtos;

public class Pagina<T>
{
    public List<T> Items { get; set; }
    public long TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}