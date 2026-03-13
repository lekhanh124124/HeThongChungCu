using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Infrastructure.Persistence.Helpers;

public static class DapperQueryBuilder
{
    public static (string SqlWhere, DynamicParameters Parameters) BuildWhere(
        IDapperSpecification spec, 
        Dictionary<string, string> propertyToColumnMap)
    {
        var parameters = new DynamicParameters();
        var andClauses = new List<string>();

        // Process standard filters (JOIN with AND)
        foreach (var filter in spec.Filters)
        {
            var clause = BuildFilterClause(filter, parameters, propertyToColumnMap);
            andClauses.Add(clause);
        }

        // Process keyword filters (JOIN with OR)
        var orClauses = new List<string>();
        foreach (var keyword in spec.Keywords)
        {
            var clause = BuildFilterClause(keyword, parameters, propertyToColumnMap);
            orClauses.Add(clause);
        }

        var finalClauses = new List<string>();
        if (andClauses.Count > 0)
        {
            finalClauses.Add(string.Join(" AND ", andClauses));
        }

        if (orClauses.Count > 0)
        {
            finalClauses.Add("(" + string.Join(" OR ", orClauses) + ")");
        }

        var sqlWhere = finalClauses.Count > 0 
            ? "WHERE " + string.Join(" AND ", finalClauses) 
            : "";

        return (sqlWhere, parameters);
    }

    private static string BuildFilterClause(
        FilterCriterion filter, 
        DynamicParameters parameters, 
        Dictionary<string, string> propertyToColumnMap)
    {
        if (!propertyToColumnMap.TryGetValue(filter.PropertyName, out var columnName))
        {
            throw new ArgumentException($"Property '{filter.PropertyName}' is used in a filter but has no mapping in the Infrastructure layer. Please add it to the propertyToColumnMap in the Repository.");
        }

        var paramName = $"p{parameters.ParameterNames.Count()}";
        var sqlOperator = string.Empty;
        object? value = filter.Value;

        switch (filter.Operator)
        {
            case FilterOperator.Equal:
                sqlOperator = "=";
                break;
            case FilterOperator.NotEqual:
                sqlOperator = "<>";
                break;
            case FilterOperator.GreaterThan:
                sqlOperator = ">";
                break;
            case FilterOperator.LessThan:
                sqlOperator = "<";
                break;
            case FilterOperator.GreaterThanOrEqual:
                sqlOperator = ">=";
                break;
            case FilterOperator.LessThanOrEqual:
                sqlOperator = "<=";
                break;
            case FilterOperator.Contains:
                sqlOperator = "LIKE";
                value = $"%{value}%";
                break;
            case FilterOperator.StartsWith:
                sqlOperator = "LIKE";
                value = $"{value}%";
                break;
            case FilterOperator.EndsWith:
                sqlOperator = "LIKE";
                value = $"%{value}";
                break;
            case FilterOperator.In:
                sqlOperator = "IN";
                break;
        }

        parameters.Add(paramName, value);
        return $"{columnName} {sqlOperator} @{paramName}";
    }

    public static string BuildOrderBy(
        IDapperSpecification spec, 
        Dictionary<string, string> propertyToColumnMap,
        string defaultSortCol = nameof(BaseEntity.Id))
    {
        var propertyName = spec.SortCol;
        
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            propertyName = defaultSortCol;
        }

        if (!propertyToColumnMap.TryGetValue(propertyName, out var columnName))
        {
            if (propertyName == defaultSortCol && !propertyName.Contains("."))
            {
                columnName = propertyName;
            }
            else
            {
                throw new ArgumentException($"Property '{propertyName}' has no mapping in the Infrastructure layer. Please add it to the propertyToColumnMap in the Repository.");
            }
        }

        var direction = (spec.IsAsc.HasValue && !spec.IsAsc.Value) ? "DESC" : "ASC";
        return $"ORDER BY {columnName} {direction}";
    }

    public static string BuildPagination(IDapperSpecification spec, DynamicParameters parameters)
    {
        var pageNumber = spec.PageNumber ?? 1;
        var pageSize = spec.PageSize ?? 20;

        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 20;

        var offset = (pageNumber - 1) * pageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", pageSize);

        return "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
    }
}
