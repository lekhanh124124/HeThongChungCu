using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.GhiChiSoTieuThu;

public sealed record GhiChiSoTieuThuCommand(
    int CanHoId,
    int DichVuId,
    double ChiSoCu,
    double ChiSoMoi,
    int Thang,
    int Nam,
    DateTime NgayChot) : ICommand<ChiSoTieuThuResponse>;

public sealed class GhiChiSoTieuThuCommandValidator : AbstractValidator<GhiChiSoTieuThuCommand>
{
    public GhiChiSoTieuThuCommandValidator()
    {
        RuleFor(x => x.CanHoId).NotEmpty();
        RuleFor(x => x.DichVuId).NotEmpty();
        RuleFor(x => x.ChiSoMoi).GreaterThanOrEqualTo(x => x.ChiSoCu);
        RuleFor(x => x.Thang).InclusiveBetween(1, 12);
        RuleFor(x => x.Nam).GreaterThan(2000);
    }
}

internal sealed class GhiChiSoTieuThuCommandHandler : ICommandHandler<GhiChiSoTieuThuCommand, ChiSoTieuThuResponse>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoTieuThuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GhiChiSoTieuThuCommandHandler(IChiSoTieuThuCommandRepository chiSoTieuThuRepository, IUnitOfWork unitOfWork)
    {
        _chiSoTieuThuRepository = chiSoTieuThuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChiSoTieuThuResponse>> Handle(GhiChiSoTieuThuCommand request, CancellationToken cancellationToken)
    {
        var existing = await _chiSoTieuThuRepository.GetByThangNamAsync(
            request.CanHoId, 
            request.DichVuId, 
            request.Thang, 
            request.Nam, 
            cancellationToken);

        if (existing is not null)
        {
            return Result.Failure<ChiSoTieuThuResponse>(new Error("ChiSoTieuThu.AlreadyExists", "Chỉ số tiêu thụ cho tháng/năm này đã tồn tại."));
        }

        var chiSo = new ChiSoTieuThu(
            request.CanHoId,
            request.DichVuId,
            request.ChiSoCu,
            request.ChiSoMoi,
            request.Thang,
            request.Nam,
            request.NgayChot);

        await _chiSoTieuThuRepository.AddAsync(chiSo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChiSoTieuThuResponse(
            chiSo.Id,
            chiSo.CanHoId,
            chiSo.DichVuId,
            chiSo.ChiSoCu,
            chiSo.ChiSoMoi,
            chiSo.SoLuong,
            chiSo.Thang,
            chiSo.Nam,
            chiSo.NgayChot,
            chiSo.IsLock);
    }
}
