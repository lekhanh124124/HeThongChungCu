namespace HeThongChungCu.Application.Features.Catalog.DTOs;

public record ItemForSelectorResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
