using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateLichBaoTri;

public class CreateLichBaoTriCommandHandler : ICommandHandler<CreateLichBaoTriCommand, LichBaoTriResponse>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLichBaoTriCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LichBaoTriResponse>> Handle(CreateLichBaoTriCommand request, CancellationToken cancellationToken)
    {
        var thietBi = await _thietBiRepository.GetThietBiByIdAsync(request.ThietBiId, cancellationToken);
        if (thietBi == null)
            return BaoTriHaTangErrors.ThietBiNotFoundById(request.ThietBiId);

        var hangMuc = await _thietBiRepository.GetHangMucByIdAsync(request.HangMucBaoTriId, cancellationToken);
        if (hangMuc == null)
            return BaoTriHaTangErrors.HangMucNotFoundById(request.HangMucBaoTriId);

        var tanSuat = TanSuatBaoTri.FromValue(request.TanSuatBaoTriId, null);
        if (tanSuat == null)
            return new Error("LichBaoTri.TanSuatInvalid", "Tần suất kiểm tra không hợp lệ.");

        var lichBaoTri = LichBaoTri.Create(
            request.ThietBiId,
            request.HangMucBaoTriId,
            tanSuat,
            request.NgayBatDau,
            request.NgayKetThuc);

        await _thietBiRepository.AddLichBaoTriAsync(lichBaoTri, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new LichBaoTriResponse
        {
            Id = lichBaoTri.Id,
            ThietBiId = lichBaoTri.ThietBiId,
            TenThietBi = thietBi.TenThietBi,
            MaThietBi = thietBi.MaThietBi,
            HangMucBaoTriId = lichBaoTri.HangMucBaoTriId,
            TenHangMuc = hangMuc.TenHangMuc,
            MaHangMuc = hangMuc.MaHangMuc,
            TanSuatBaoTriId = lichBaoTri.TanSuatBaoTriId.Value,
            TenTanSuatBaoTri = lichBaoTri.TanSuatBaoTriId.Name,
            NgayBatDau = lichBaoTri.NgayBatDau,
            NgayKetThuc = lichBaoTri.NgayKetThuc,
            NgayBaoTriGanNhat = lichBaoTri.NgayBaoTriGanNhat,
            NgayBaoTriTiepTheo = lichBaoTri.NgayBaoTriTiepTheo,
            IsActive = lichBaoTri.IsActive
        });
    }
}
