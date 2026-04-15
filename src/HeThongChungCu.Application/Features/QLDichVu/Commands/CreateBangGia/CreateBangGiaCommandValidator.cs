using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateBangGia;

public class CreateBangGiaCommandValidator : AbstractValidator<CreateBangGiaCommand>
{
    public CreateBangGiaCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .NotEmpty().WithMessage("Giá trị Dịch vụ phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.TenBangGia)
            .NotEmpty().WithMessage("Tên bảng giá không được để trống.")
            .MaximumLength(100).WithMessage("Tên bảng giá không được vượt quá 100 ký tự.");

        RuleFor(x => x.NgayApDung)
            .NotEmpty().WithMessage("Ngày áp dụng không được để trống.");

        RuleFor(x => x.NgayKetThuc)
            .GreaterThan(x => x.NgayApDung)
            .When(x => x.NgayKetThuc.HasValue)
            .WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu.");

        RuleFor(x => x.LoaiDinhGiaId)
            .Must(v => LoaiDinhGia.FromValue(v) != null).WithMessage("Loại định giá không hợp lệ.");

        // Additional validations based on LoaiDinhGiaId
        When(x => x.LoaiDinhGiaId == LoaiDinhGia.CoDinh.Value, () =>
        {
            RuleFor(x => x.DonGiaCoDinh)
                .NotNull().WithMessage("Đơn giá không được để trống cho bảng giá cố định.")
                .GreaterThanOrEqualTo(0).WithMessage("Đơn giá phải >= 0.");
            RuleFor(x => x.GiaLuyTiens).Empty().WithMessage("Bảng giá cố định không được có chi tiết lũy tiến.");
            RuleFor(x => x.GiaKhungGios).Empty().WithMessage("Bảng giá cố định không được có chi tiết khung giờ.");
            RuleFor(x => x.GiaLoaiCanHos).Empty().WithMessage("Bảng giá cố định không được có chi tiết loại căn hộ.");
        });

        When(x => x.LoaiDinhGiaId == LoaiDinhGia.LuyTien.Value, () =>
        {
            RuleFor(x => x.GiaLuyTiens)
                .NotEmpty().WithMessage("Bảng giá lũy tiến phải có ít nhất một bậc giá.");
            RuleForEach(x => x.GiaLuyTiens).SetValidator(new CreateChiTietGiaLuyTienDtoValidator());
            RuleFor(x => x.DonGiaCoDinh).Null().WithMessage("Bảng giá lũy tiến không dùng đơn giá cố định.");
            RuleFor(x => x.GiaKhungGios).Empty().WithMessage("Bảng giá lũy tiến không được có chi tiết khung giờ.");
            RuleFor(x => x.GiaLoaiCanHos).Empty().WithMessage("Bảng giá lũy tiến không được có chi tiết loại căn hộ.");
        });

        When(x => x.LoaiDinhGiaId == LoaiDinhGia.TheoKhungGio.Value, () =>
        {
            RuleFor(x => x.GiaKhungGios)
                .NotEmpty().WithMessage("Bảng giá khung giờ phải có ít nhất một thông tin giá.");
            RuleForEach(x => x.GiaKhungGios).SetValidator(new CreateChiTietGiaKhungGioDtoValidator());
            RuleFor(x => x.DonGiaCoDinh).Null().WithMessage("Bảng giá khung giờ không dùng đơn giá cố định.");
            RuleFor(x => x.GiaLuyTiens).Empty().WithMessage("Bảng giá khung giờ không được có chi tiết lũy tiến.");
            RuleFor(x => x.GiaLoaiCanHos).Empty().WithMessage("Bảng giá khung giờ không được có chi tiết loại căn hộ.");
        });

        When(x => x.LoaiDinhGiaId == LoaiDinhGia.TheoDienTich.Value, () =>
        {
            RuleFor(x => x.GiaLoaiCanHos)
                .NotEmpty().WithMessage("Bảng giá theo loại căn hộ phải có ít nhất một thông tin giá.");
            RuleForEach(x => x.GiaLoaiCanHos).SetValidator(new CreateChiTietGiaLoaiCanHoDtoValidator());
            RuleFor(x => x.DonGiaCoDinh).Null().WithMessage("Bảng giá theo diện tích không dùng đơn giá cố định.");
            RuleFor(x => x.GiaLuyTiens).Empty().WithMessage("Bảng giá theo diện tích không được có chi tiết lũy tiến.");
            RuleFor(x => x.GiaKhungGios).Empty().WithMessage("Bảng giá theo diện tích không được có chi tiết khung giờ.");
        });
    }
}

public class CreateChiTietGiaLoaiCanHoDtoValidator : AbstractValidator<CreateChiTietGiaLoaiCanHoDto>
{
    public CreateChiTietGiaLoaiCanHoDtoValidator()
    {
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0).WithMessage("Đơn giá phải >= 0.");
    }
}

public class CreateChiTietGiaLuyTienDtoValidator : AbstractValidator<CreateChiTietGiaLuyTienDto>
{
    public CreateChiTietGiaLuyTienDtoValidator()
    {
        RuleFor(x => x.TuMuc).GreaterThanOrEqualTo(0).WithMessage("Từ số phải >= 0.");
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0).WithMessage("Đơn giá phải >= 0.");
    }
}

public class CreateChiTietGiaKhungGioDtoValidator : AbstractValidator<CreateChiTietGiaKhungGioDto>
{
    public CreateChiTietGiaKhungGioDtoValidator()
    {
        RuleFor(x => x.KhungGioId).NotEmpty().WithMessage("Không tìm thấy khung giờ của dịch vụ.");
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0).WithMessage("Đơn giá phải >= 0.");
    }
}
