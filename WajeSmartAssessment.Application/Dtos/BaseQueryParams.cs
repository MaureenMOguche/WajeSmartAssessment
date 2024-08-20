namespace WajeSmartAssessment.Application.Dtos;
public class BaseQueryParams
{
    private const int _maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _maxPageSize;
        set => _ = value > _maxPageSize ? _maxPageSize : value;
    }
    public string? Search { get; set; }
}
