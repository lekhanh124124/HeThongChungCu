using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateLichBaoTri;

public class UpdateLichBaoTriCommandHandler : ICommandHandler<UpdateLichBaoTriCommand, LichBaoTriResponse>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLichBaoTriCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LichBaoTriResponse>> Handle(UpdateLichBaoTriCommand request, CancellationToken cancellationToken)
    {
        var lichBaoTri = await _thietBiRepository.GetLichBaoTriByIdAsync(request.Id, cancellationToken);
        if (lichBaoTri == null)
            return BaoTriHaTangErrors.LichBaoTriNotFoundById(request.Id);

        var thietBi = await _thietBiRepository.GetThietBiByIdAsync(lichBaoTri.ThietBiId, cancellationToken);
        if (thietBi == null)
            return BaoTriHaTangErrors.ThietBiNotFoundById(lichBaoTri.ThietBiId);

        var hangMuc = await _thietBiRepository.GetHangMucByIdAsync(lichBaoTri.HangMucBaoTriId, cancellationToken);
        if (hangMuc == null)
            return BaoTriHaTangErrors.HangMucNotFoundById(lichBaoTri.HangMucBaoTriId);

        var tanSuat = TanSuatBaoTri.FromValue(request.TanSuatBaoTriId, null);
        if (tanSuat == null)
            return new Error("LichBaoTri.TanSuatInvalid", "Tần suất kiểm tra không hợp lệ.");

        lichBaoTri.Update(
            tanSuat,
            request.NgayBatDau,
            request.NgayKetThuc,
            request.IsActive);

        _thietBiRepository.UpdateLichBaoTri(lichBaoTri);
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
