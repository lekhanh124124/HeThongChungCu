namespace HeThongChungCu.Application.Features.QLDichVu.DTOs
{
    public class DichVuDetailResponse : DichVuResponse
    {
        public IReadOnlyList<KhungGioDichVuResponse> KhungGioDichVu { get; set; } = [];
        public BangGiaResponse BangGia { get; set; } = null!;
    }
}
