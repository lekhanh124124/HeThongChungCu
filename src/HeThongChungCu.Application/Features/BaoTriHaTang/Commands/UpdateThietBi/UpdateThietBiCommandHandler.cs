using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateThietBi;

public class UpdateThietBiCommandHandler : ICommandHandler<UpdateThietBiCommand, ThietBiResponse>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateThietBiCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ThietBiResponse>> Handle(UpdateThietBiCommand request, CancellationToken cancellationToken)
    {
        var thietBi = await _thietBiRepository.GetThietBiByIdAsync(request.Id, cancellationToken);
        if (thietBi == null)
            return BaoTriHaTangErrors.ThietBiNotFoundById(request.Id);

        thietBi.Update(
            request.TenThietBi,
            request.LoaiThietBi,
            request.ViTri,
            request.NgayMua,
            request.NgayHetHanBaoHanh,
            request.GiaTriBanDau,
            request.GhiChu,
            request.ToaNhaId);

        if (request.TrangThaiThietBiId.HasValue)
        {
            var status = TrangThaiThietBi.FromValue(request.TrangThaiThietBiId.Value, null);
            if (status != null)
            {
                thietBi.UpdateTrangThai(status);
            }
        }

        _thietBiRepository.UpdateThietBi(thietBi);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ThietBiResponse
        {
            Id = thietBi.Id,
            MaThietBi = thietBi.MaThietBi,
            TenThietBi = thietBi.TenThietBi,
            LoaiThietBi = thietBi.LoaiThietBi,
            ViTri = thietBi.ViTri,
            NgayMua = thietBi.NgayMua,
            NgayHetHanBaoHanh = thietBi.NgayHetHanBaoHanh,
            GiaTriBanDau = thietBi.GiaTriBanDau,
            TrangThaiThietBiId = thietBi.TrangThaiThietBiId.Value,
            TenTrangThaiThietBi = thietBi.TrangThaiThietBiId.Name,
            GhiChu = thietBi.GhiChu,
            ToaNhaId = thietBi.ToaNhaId
        });
    }
}
