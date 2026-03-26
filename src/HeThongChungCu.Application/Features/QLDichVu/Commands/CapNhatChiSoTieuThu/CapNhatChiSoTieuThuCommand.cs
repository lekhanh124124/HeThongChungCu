using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CapNhatChiSoTieuThu;

public sealed record CapNhatChiSoTieuThuCommand(
    int Id,
    double ChiSoCu,
    double ChiSoMoi,
    int Thang,
    int Nam,
    DateTime NgayChot) : ICommand<ChiSoTieuThuResponse>;

public sealed class CapNhatChiSoTieuThuCommandValidator : AbstractValidator<CapNhatChiSoTieuThuCommand>
{
    public CapNhatChiSoTieuThuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ChiSoMoi).GreaterThanOrEqualTo(x => x.ChiSoCu);
    }
}

internal sealed class CapNhatChiSoTieuThuCommandHandler : ICommandHandler<CapNhatChiSoTieuThuCommand, ChiSoTieuThuResponse>
{
    private readonly IChiSoTieuThuEFRepository _chiSoTieuThuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatChiSoTieuThuCommandHandler(IChiSoTieuThuEFRepository chiSoTieuThuRepository, IUnitOfWork unitOfWork)
    {
        _chiSoTieuThuRepository = chiSoTieuThuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChiSoTieuThuResponse>> Handle(CapNhatChiSoTieuThuCommand request, CancellationToken cancellationToken)
    {
        var chiSo = await _chiSoTieuThuRepository.GetByIdAsync(request.Id, cancellationToken);
        if (chiSo is null)
        {
            return Result.Failure<ChiSoTieuThuResponse>(new Error("ChiSoTieuThu.NotFound", "Không tìm thấy chỉ số tiêu thụ."));
        }

        chiSo.Update(request.ChiSoCu, request.ChiSoMoi, request.Thang, request.Nam, request.NgayChot);
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
