using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.CuDanDanhGiaVaDongTicket;

public class CuDanDanhGiaVaDongTicketCommandValidator : AbstractValidator<CuDanDanhGiaVaDongTicketCommand>
{
    public CuDanDanhGiaVaDongTicketCommandValidator()
    {
        RuleFor(x => x.PhanAnhId)
            .NotEmpty().WithMessage("PhanAnhId không được để trống.");

        RuleFor(x => x.DiemDanhGia)
            .InclusiveBetween(1, 5).WithMessage("Điểm đánh giá phải từ 1 đến 5 sao.");

        RuleFor(x => x.NhanXetDanhGia)
            .MaximumLength(500).WithMessage("Nhận xét đánh giá không được dài quá 500 ký tự.");
    }
}
