using FluentValidation;
using FluentValidation.Results;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

namespace HeThongChungCu.Application.Common.Models;

public abstract class BaseSpecification : IQuerySpecification
{
    public string? SortCol { get; set; }
    public bool? IsAsc { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public virtual HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase) { nameof(BaseEntity.Id) };

    private readonly List<FilterCriterion> _filters = [];
    public IReadOnlyList<FilterCriterion> Filters => _filters;

    private readonly List<FilterCriterion> _keywords = [];
    public IReadOnlyList<FilterCriterion> Keywords => _keywords;

    protected BaseSpecification(string? sortCol, bool? isAsc, int? pageNumber, int? pageSize)
    {
        SortCol = sortCol;
        IsAsc = isAsc;
        PageNumber = (pageNumber == null || pageNumber <= 0) ? 1 : pageNumber;
        PageSize = (pageSize == null || pageSize <= 0) ? 20 : pageSize;

        if (!string.IsNullOrWhiteSpace(SortCol) && !AllowedSortColumns.Contains(SortCol))
        {
            throw new ValidationException(
            [
                new(nameof(SortCol), $"Hệ thống không hỗ trợ sắp xếp theo trường '{SortCol}'")
            ]);
        }
    }

    // Thêm trường tìm kiếm vào bộ lọc
    protected void AddFilter(string propertyName, FilterOperator @operator, object? value = null)
    {
        if (value is null && @operator != FilterOperator.IsNull && @operator != FilterOperator.IsNotNull)
            return;

        _filters.Add(new FilterCriterion(propertyName, @operator, value));
    }

    // Thêm trường tìm kiếm vào từ khóa
    protected void AddKeyword(string propertyName, FilterOperator @operator, object? value = null)
    {
        if (value is null && @operator != FilterOperator.IsNull && @operator != FilterOperator.IsNotNull)
            return;

        _keywords.Add(new FilterCriterion(propertyName, @operator, value));
    }
}
