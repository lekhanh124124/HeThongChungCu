using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.KhoaChiSoTieuThu;

public sealed record KhoaChiSoTieuThuCommand(int Id) : ICommand<ChiSoTieuThuResponse>;

internal sealed class KhoaChiSoTieuThuCommandHandler : ICommandHandler<KhoaChiSoTieuThuCommand, ChiSoTieuThuResponse>
{
    private readonly IChiSoTieuThuEFRepository _chiSoTieuThuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public KhoaChiSoTieuThuCommandHandler(IChiSoTieuThuEFRepository chiSoTieuThuRepository, IUnitOfWork unitOfWork)
    {
        _chiSoTieuThuRepository = chiSoTieuThuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChiSoTieuThuResponse>> Handle(KhoaChiSoTieuThuCommand request, CancellationToken cancellationToken)
    {
        var chiSoTieuThu = await _chiSoTieuThuRepository.GetByIdAsync(request.Id, cancellationToken);
        if (chiSoTieuThu is null)
        {
            return Result.Failure<ChiSoTieuThuResponse>(new Error("ChiSoTieuThu.NotFound", "Không tìm thấy chỉ số tiêu thụ."));
        }

        chiSoTieuThu.Lock();

        _chiSoTieuThuRepository.Update(chiSoTieuThu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChiSoTieuThuResponse(
            chiSoTieuThu.Id,
            chiSoTieuThu.CanHoId,
            chiSoTieuThu.DichVuId,
            chiSoTieuThu.ChiSoCu,
            chiSoTieuThu.ChiSoMoi,
            chiSoTieuThu.SoLuong,
            chiSoTieuThu.Thang,
            chiSoTieuThu.Nam,
            chiSoTieuThu.NgayChot,
            chiSoTieuThu.IsLock);
    }
}
