namespace HeThongChungCu.Application.Common.Models;

public class FilterCriterion
{
    public string PropertyName { get; }
    public FilterOperator Operator { get; }
    public object? Value { get; }

    public FilterCriterion(string propertyName, FilterOperator @operator, object? value)
    {
        PropertyName = propertyName;
        Operator = @operator;
        Value = value;
    }
}
