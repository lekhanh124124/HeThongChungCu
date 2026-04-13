using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Infrastructure.Persistence.Helpers;

public enum JoinType
{
    Inner,
    Left,
    Right
}

public record JoinDefinition(
    string Table,
    string Alias,
    string OnCondition,
    JoinType Type = JoinType.Left,
    bool AddSoftDelete = true,
    IEnumerable<(string Column, object Value)>? Discriminators = null);

public static class DapperQueryBuilder
{
    public static string BuildWhere(
        IQuerySpecification spec,
        Dictionary<string, string> propertyToColumnMap,
        DynamicParameters parameters,
        bool addSoftDeleteFilter = true,
        IEnumerable<(string Column, object Value)>? discriminators = null)
    {
        var andClauses = new List<string>();

        // 0. Automatically add Discriminator filter if provided
        if (discriminators != null)
        {
            int i = 0;
            foreach (var (Column, Value) in discriminators)
            {
                var paramName = $"p_disc_{i++}";
                parameters.Add(paramName, Value);
                andClauses.Add($"{Column} = @{paramName}");
            }
        }

        // 1. Process standard filters (JOIN with AND)
        foreach (var filter in spec.Filters)
        {
            var clause = BuildFilterClause(filter, parameters, propertyToColumnMap);
            andClauses.Add(clause);
        }

        // 2. Automatically add Soft Delete Filter (IsDeleted = 0) if mapped and NOT provided in Spec
        if (addSoftDeleteFilter && propertyToColumnMap.TryGetValue("IsDeleted", out var isDeletedColumn))
        {
            // Check if Spec already has an explicit Filter for IsDeleted
            var hasDirectFilter = spec.Filters.Any(f => f.PropertyName.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase));
            if (!hasDirectFilter)
            {
                andClauses.Add($"{isDeletedColumn} = 0");
            }
        }

        // 3. Process keyword filters (JOIN with OR)
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

        return sqlWhere;
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
            case FilterOperator.IsNull:
                return $"{columnName} IS NULL";
            case FilterOperator.IsNotNull:
                return $"{columnName} IS NOT NULL";
        }

        parameters.Add(paramName, value);
        return $"{columnName} {sqlOperator} @{paramName}";
    }

    public static string BuildOrderBy(
        IQuerySpecification spec,
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
            if (propertyName == defaultSortCol && !propertyName.Contains('.'))
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

    public static string BuildPagination(IQuerySpecification spec, DynamicParameters parameters)
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

    public static string BuildJoin(
        IEnumerable<JoinDefinition> joins)
    {
        var joinClauses = new List<string>();

        foreach (var join in joins)
        {
            var joinType = join.Type switch
            {
                JoinType.Inner => "INNER JOIN",
                JoinType.Right => "RIGHT JOIN",
                _ => "LEFT JOIN"
            };

            var onClauses = new List<string> { join.OnCondition };

            if (join.AddSoftDelete)
            {
                onClauses.Add($"{join.Alias}.IsDeleted = 0");
            }

            if (join.Discriminators != null)
            {
                foreach (var (Column, Value) in join.Discriminators)
                {
                    var formattedValue = Value is string s ? $"'{s}'" : Value.ToString();
                    onClauses.Add($"{join.Alias}.{Column} = {formattedValue}");
                }
            }

            var onSection = string.Join(" AND ", onClauses);
            joinClauses.Add($"{joinType} {join.Table} {join.Alias} ON {onSection}");
        }

        return string.Join("\n", joinClauses);
    }
}
