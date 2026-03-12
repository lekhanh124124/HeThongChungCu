using FluentValidation.Results;

namespace HeThongChungCu.Application.Common.Models;

public abstract class BaseSpecification : IDapperSpecification
{
    public string? SortCol { get; set; }
    public bool? IsAsc { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public virtual HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase) { "Id" };

    private readonly List<FilterCriterion> _filters = new();
    public IReadOnlyList<FilterCriterion> Filters => _filters;

    private readonly List<FilterCriterion> _keywords = new();
    public IReadOnlyList<FilterCriterion> Keywords => _keywords;

    protected BaseSpecification(string? sortCol, bool? isAsc, int? pageNumber, int? pageSize)
    {
        SortCol = sortCol;
        IsAsc = isAsc;
        PageNumber = (pageNumber == null || pageNumber <= 0) ? 1 : pageNumber;
        PageSize = (pageSize == null || pageSize <= 0) ? 20 : pageSize;

        if (!string.IsNullOrWhiteSpace(SortCol) && !AllowedSortColumns.Contains(SortCol))
        {
            throw new ValidationException(new List<ValidationFailure>
            {
                new("SortCol", $"Property '{SortCol}' is not allowed for sorting content. Allowed properties: {string.Join(", ", AllowedSortColumns)}")
            });
        }
    }

    protected void AddFilter(string propertyName, FilterOperator @operator, object? value = null)
    {
        _filters.Add(new FilterCriterion(propertyName, @operator, value));
    }

    protected void AddKeyword(string propertyName, FilterOperator @operator, object? value = null)
    {
        _keywords.Add(new FilterCriterion(propertyName, @operator, value));
    }
}
