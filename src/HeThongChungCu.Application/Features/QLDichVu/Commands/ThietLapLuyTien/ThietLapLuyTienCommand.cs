using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ThietLapLuyTien;

public record TierRequest(double TuMuc, double? DenMuc, decimal DonGia);

public sealed record ThietLapLuyTienCommand(
    int BangGiaId,
    List<TierRequest> Tiers) : ICommand<BangGiaResponse>;

public sealed class ThietLapLuyTienCommandValidator : AbstractValidator<ThietLapLuyTienCommand>
{
    public ThietLapLuyTienCommandValidator()
    {
        RuleFor(x => x.BangGiaId).NotEmpty();
        RuleFor(x => x.Tiers).NotEmpty();
        RuleForEach(x => x.Tiers).ChildRules(tier =>
        {
            tier.RuleFor(t => t.TuMuc).GreaterThanOrEqualTo(0);
            tier.RuleFor(t => t.DonGia).GreaterThanOrEqualTo(0);
            tier.RuleFor(t => t)
                .Must(t => t.DenMuc == null || t.DenMuc > t.TuMuc)
                .WithMessage("Đến số phải lớn hơn Từ số.");
        });
    }
}

internal sealed class ThietLapLuyTienCommandHandler : ICommandHandler<ThietLapLuyTienCommand, BangGiaResponse>
{
    private readonly IBangGiaCommandRepository _bangGiaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ThietLapLuyTienCommandHandler(IBangGiaCommandRepository bangGiaRepository, IUnitOfWork unitOfWork)
    {
        _bangGiaRepository = bangGiaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BangGiaResponse>> Handle(ThietLapLuyTienCommand request, CancellationToken cancellationToken)
    {
        var bangGia = await _bangGiaRepository.GetByIdAsync(request.BangGiaId, cancellationToken);
        if (bangGia is null)
        {
            return Result.Failure<BangGiaResponse>(new Error("BangGia.NotFound", "Không tìm thấy bảng giá."));
        }

        if (bangGia.LoaiDinhGiaId != Domain.Enums.LoaiDinhGia.LuyTien)
        {
            return Result.Failure<BangGiaResponse>(new Error("BangGia.InvalidType", "Bảng giá này không hỗ trợ định giá lũy tiến."));
        }

        bangGia.ClearLuyTien();

        foreach (var tier in request.Tiers.OrderBy(t => t.TuMuc))
        {
            bangGia.AddLuyTien(tier.TuMuc, tier.DenMuc, tier.DonGia);
        }

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
