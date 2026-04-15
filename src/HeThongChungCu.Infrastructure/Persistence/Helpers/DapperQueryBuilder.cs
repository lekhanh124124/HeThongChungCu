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
    Dictionary<string, string>? Mapping = null);

public static class DapperQueryBuilder
{
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
        IQuerySpecification spec,
        IEnumerable<JoinDefinition> joins,
        DynamicParameters parameters)
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

            if (join.Mapping != null)
            {
                // 1. Process Filters
                foreach (var filter in spec.Filters)
                {
                    if (join.Mapping.TryGetValue(filter.PropertyName, out var columnName))
                    {
                        var clause = BuildFilterClause(filter, parameters, new Dictionary<string, string> { { filter.PropertyName, columnName } });
                        onClauses.Add(clause);
                    }
                }

                // 2. Process Keywords
                foreach (var keyword in spec.Keywords)
                {
                    if (join.Mapping.TryGetValue(keyword.PropertyName, out var columnName))
                    {
                        var clause = BuildFilterClause(keyword, parameters, new Dictionary<string, string> { { keyword.PropertyName, columnName } });
                        onClauses.Add(clause);
                    }
                }
            }

            var onSection = string.Join(" AND ", onClauses);
            joinClauses.Add($"{joinType} {join.Table} {join.Alias} ON {onSection}");
        }

        return string.Join("\n", joinClauses);
    }

    public static string BuildWhere(
        IQuerySpecification spec,
        Dictionary<string, string> propertyToColumnMap,
        DynamicParameters parameters)
    {
        var andClauses = new List<string>();

        // 1. Process standard filters (JOIN with AND)
        foreach (var filter in spec.Filters)
        {
            if (propertyToColumnMap.ContainsKey(filter.PropertyName))
            {
                var clause = BuildFilterClause(filter, parameters, propertyToColumnMap);
                andClauses.Add(clause);
            }
        }

        // 2. Process keyword filters (JOIN with OR)
        var orClauses = new List<string>();
        foreach (var keyword in spec.Keywords)
        {
            if (propertyToColumnMap.ContainsKey(keyword.PropertyName))
            {
                var clause = BuildFilterClause(keyword, parameters, propertyToColumnMap);
                orClauses.Add(clause);
            }
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

        return finalClauses.Count > 0
            ? "WHERE " + string.Join(" AND ", finalClauses)
            : "";
    }

    public static string BuildOrderBy(
        IQuerySpecification spec,
        Dictionary<string, string> propertyToColumnMap,
        string defaultSortCol = "Id")
    {
        var propertyName = spec.SortCol;

        // Ưu tiên lấy mapping từ SortCol của Spec, nếu không có thì fallback về defaultSortCol
        if (string.IsNullOrWhiteSpace(propertyName) || !propertyToColumnMap.TryGetValue(propertyName, out var columnName))
        {
            columnName = propertyToColumnMap.GetValueOrDefault(defaultSortCol, defaultSortCol);
        }

        var direction = (spec.IsAsc.HasValue && !spec.IsAsc.Value) ? "DESC" : "ASC";
        return $"ORDER BY {columnName} {direction}";
    }
}
