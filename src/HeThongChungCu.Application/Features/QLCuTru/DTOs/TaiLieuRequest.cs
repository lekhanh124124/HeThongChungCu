namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public record TaiLieuRequest(
    int LoaiGiayToId,
    string SoGiayTo,
    DateTime? NgayPhatHanh,
    List<int> FileIds);
