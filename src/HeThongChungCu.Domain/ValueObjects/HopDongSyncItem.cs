namespace HeThongChungCu.Domain.ValueObjects;

public record HopDongSyncItem(
    int? Id,
    string SoHopDong,
    DateTimeOffset NgayKy,
    DateTimeOffset NgayHetHan,
    decimal GiaTri,
    int DichVuId,
    string? NoiDung,
    IEnumerable<int>? TepFileIds
);
