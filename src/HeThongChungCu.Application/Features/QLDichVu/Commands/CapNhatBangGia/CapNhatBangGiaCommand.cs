using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CapNhatBangGia;

public sealed record CapNhatBangGiaCommand(
    int Id,
    string TenBangGia,
    DateTime NgayApDung,
    DateTime? NgayKetThuc,
    decimal DonGia) : ICommand<BangGiaResponse>;

public sealed class CapNhatBangGiaCommandValidator : AbstractValidator<CapNhatBangGiaCommand>
{
    public CapNhatBangGiaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TenBangGia).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NgayApDung).NotEmpty();
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0);
        
        RuleFor(x => x)
            .Must(x => x.NgayKetThuc == null || x.NgayKetThuc > x.NgayApDung)
            .WithMessage("Ngày kết thúc phải lớn hơn ngày áp dụng.");
    }
}

internal sealed class CapNhatBangGiaCommandHandler : ICommandHandler<CapNhatBangGiaCommand, BangGiaResponse>
{
    private readonly IBangGiaEFRepository _bangGiaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatBangGiaCommandHandler(IBangGiaEFRepository bangGiaRepository, IUnitOfWork unitOfWork)
    {
        _bangGiaRepository = bangGiaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BangGiaResponse>> Handle(CapNhatBangGiaCommand request, CancellationToken cancellationToken)
    {
        var bangGia = await _bangGiaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (bangGia is null)
        {
            return Result.Failure<BangGiaResponse>(new Error("BangGia.NotFound", "Không tìm thấy bảng giá."));
        }

        // Check overlaps excluding current
        var existingPrices = await _bangGiaRepository.GetByDichVuIdAsync(bangGia.DichVuId, cancellationToken);
        if (existingPrices.Any(p => p.Id != request.Id && p.IsOverlapping(request.NgayApDung, request.NgayKetThuc)))
        {
            return Result.Failure<BangGiaResponse>(new Error("BangGia.Overlap", "Thời gian áp dụng bảng giá bị chồng lấn với bảng giá hiện có."));
        }

        bangGia.UpdateInfo(
            request.TenBangGia,
            request.NgayApDung,
            request.NgayKetThuc,
            request.DonGia,
            bangGia.LoaiDinhGiaId);

        _bangGiaRepository.Update(bangGia);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BangGiaResponse(
            bangGia.Id,
            bangGia.DichVuId,
            bangGia.TenBangGia,
            bangGia.NgayApDung,
            bangGia.NgayKetThuc,
            bangGia.DonGia,
            bangGia.LoaiDinhGiaId.Value,
            bangGia.IsActive,
            bangGia.BangGiaLuyTiens.Select(l => new BangGiaLuyTienResponse(l.TuMuc, l.DenMuc, l.DonGia)).ToList());
    }
}
