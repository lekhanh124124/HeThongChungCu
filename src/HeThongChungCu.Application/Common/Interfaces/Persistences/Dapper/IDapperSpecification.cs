namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IDapperSpecification
{
    string? SortCol { get; }
    bool? IsAsc { get; }
    int? PageNumber { get; }
    int? PageSize { get; }
    HashSet<string> AllowedSortColumns { get; }
    IReadOnlyList<FilterCriterion> Filters { get; }
    IReadOnlyList<FilterCriterion> Keywords { get; }
}
