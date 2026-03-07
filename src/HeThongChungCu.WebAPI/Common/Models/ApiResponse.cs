namespace HeThongChungCu.WebAPI.Common.Models;

using HeThongChungCu.Domain.Common;

public class ApiResponse<T>
{
    public T? Result { get; set; }
    public List<string> WarningMessages { get; set; } = new List<string>();
    public List<Error> Errors { get; set; } = new List<Error>();
    public bool IsOk { get; set; }
}
