namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public record TaiLieuRequest(
    int? TaiLieuCuTruId,
    int LoaiGiayToId,
    string SoGiayTo,
    DateTime? NgayPhatHanh,
    List<int> FileIds);
