using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.ValueObjects;

public record DocumentSyncItem(
    int? Id,
    int LoaiGiayToId,
    string SoGiayTo,
    DateTimeOffset? NgayPhatHanh,
    IEnumerable<int> FileIds
);
