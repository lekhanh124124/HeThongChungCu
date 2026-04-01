namespace HeThongChungCu.Application.Features.Catalog.DTOs;

public record ItemForSelectorResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
