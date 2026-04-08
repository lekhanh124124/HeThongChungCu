using FluentValidation;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateBangGia;

public class CreateBangGiaCommandValidator : AbstractValidator<CreateBangGiaCommand>
{
    public CreateBangGiaCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .NotEmpty().WithMessage(DichVuErrors.DichVuIdRange.Description);

        RuleFor(x => x.TenBangGia)
            .NotEmpty().WithMessage(DichVuErrors.TenBangGiaNotEmpty.Description)
            .MaximumLength(100).WithMessage(DichVuErrors.TenBangGiaMaxLength.Description);

        RuleFor(x => x.NgayApDung)
            .NotEmpty().WithMessage(DichVuErrors.NgayApDungNotEmpty.Description);

        RuleFor(x => x.NgayKetThuc)
            .GreaterThan(x => x.NgayApDung)
            .When(x => x.NgayKetThuc.HasValue)
            .WithMessage(DichVuErrors.NgayKetThucGreaterThanBatDau.Description);

        RuleFor(x => x.LoaiDinhGiaId)
            .Must(v => LoaiDinhGia.FromValue(v) != null).WithMessage(DichVuErrors.LoaiDinhGiaInvalid.Description);

        // Additional validations based on LoaiDinhGiaId
        When(x => x.LoaiDinhGiaId == LoaiDinhGia.CoDinh.Value, () =>
        {
            RuleFor(x => x.DonGiaCoDinh)
                .NotNull().WithMessage(DichVuErrors.DonGiaCoDinhRequired.Description)
                .GreaterThanOrEqualTo(0).WithMessage(DichVuErrors.DonGiaPositive.Description);
            RuleFor(x => x.GiaLuyTiens).Empty().WithMessage(DichVuErrors.CoDinhNoLuyTien.Description);
            RuleFor(x => x.GiaKhungGios).Empty().WithMessage(DichVuErrors.CoDinhNoKhungGio.Description);
            RuleFor(x => x.GiaLoaiCanHos).Empty().WithMessage(DichVuErrors.CoDinhNoLoaiCanHo.Description);
        });

        When(x => x.LoaiDinhGiaId == LoaiDinhGia.LuyTien.Value, () =>
        {
            RuleFor(x => x.GiaLuyTiens)
                .NotEmpty().WithMessage(DichVuErrors.GiaLuyTienNotEmpty.Description);
            RuleForEach(x => x.GiaLuyTiens).SetValidator(new CreateChiTietGiaLuyTienDtoValidator());
            RuleFor(x => x.DonGiaCoDinh).Null().WithMessage(DichVuErrors.LuyTienNoDonGia.Description);
            RuleFor(x => x.GiaKhungGios).Empty().WithMessage(DichVuErrors.LuyTienNoKhungGio.Description);
            RuleFor(x => x.GiaLoaiCanHos).Empty().WithMessage(DichVuErrors.LuyTienNoLoaiCanHo.Description);
        });

        When(x => x.LoaiDinhGiaId == LoaiDinhGia.TheoKhungGio.Value, () =>
        {
            RuleFor(x => x.GiaKhungGios)
                .NotEmpty().WithMessage(DichVuErrors.GiaKhungGioNotEmpty.Description);
            RuleForEach(x => x.GiaKhungGios).SetValidator(new CreateChiTietGiaKhungGioDtoValidator());
            RuleFor(x => x.DonGiaCoDinh).Null().WithMessage(DichVuErrors.KhungGioNoDonGia.Description);
            RuleFor(x => x.GiaLuyTiens).Empty().WithMessage(DichVuErrors.KhungGioNoLuyTien.Description);
            RuleFor(x => x.GiaLoaiCanHos).Empty().WithMessage(DichVuErrors.KhungGioNoLoaiCanHo.Description);
        });

        When(x => x.LoaiDinhGiaId == LoaiDinhGia.TheoDienTich.Value, () =>
        {
            RuleFor(x => x.GiaLoaiCanHos)
                .NotEmpty().WithMessage(DichVuErrors.GiaLoaiCanHoNotEmpty.Description);
            RuleForEach(x => x.GiaLoaiCanHos).SetValidator(new CreateChiTietGiaLoaiCanHoDtoValidator());
            RuleFor(x => x.DonGiaCoDinh).Null().WithMessage(DichVuErrors.DienTichNoDonGia.Description);
            RuleFor(x => x.GiaLuyTiens).Empty().WithMessage(DichVuErrors.DienTichNoLuyTien.Description);
            RuleFor(x => x.GiaKhungGios).Empty().WithMessage(DichVuErrors.DienTichNoKhungGio.Description);
        });
    }
}

public class CreateChiTietGiaLoaiCanHoDtoValidator : AbstractValidator<CreateChiTietGiaLoaiCanHoDto>
{
    public CreateChiTietGiaLoaiCanHoDtoValidator()
    {
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0).WithMessage(DichVuErrors.DonGiaPositive.Description);
    }
}

public class CreateChiTietGiaLuyTienDtoValidator : AbstractValidator<CreateChiTietGiaLuyTienDto>
{
    public CreateChiTietGiaLuyTienDtoValidator()
    {
        RuleFor(x => x.TuMuc).GreaterThanOrEqualTo(0).WithMessage(DichVuErrors.TuMucRange.Description);
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0).WithMessage(DichVuErrors.DonGiaPositive.Description);
    }
}

public class CreateChiTietGiaKhungGioDtoValidator : AbstractValidator<CreateChiTietGiaKhungGioDto>
{
    public CreateChiTietGiaKhungGioDtoValidator()
    {
        RuleFor(x => x.KhungGioId).NotEmpty().WithMessage(DichVuErrors.KhungGioNotFound.Description);
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0).WithMessage(DichVuErrors.DonGiaPositive.Description);
    }
}
