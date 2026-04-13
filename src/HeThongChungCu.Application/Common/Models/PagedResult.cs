namespace HeThongChungCu.Application.Common.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T>? Items { get; set; } = [];
        public PagingInfo PagingInfo { get; set; } = new();
    }

    public class PagingInfo
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public int TotalItems { get; set; }
    }
}
