using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateThietBi;

public class CreateThietBiCommandHandler : ICommandHandler<CreateThietBiCommand, ThietBiResponse>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateThietBiCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ThietBiResponse>> Handle(CreateThietBiCommand request, CancellationToken cancellationToken)
    {
        var maExists = await _thietBiRepository.MaThietBiExistsAsync(request.MaThietBi, cancellationToken);
        if (maExists)
            return BaoTriHaTangErrors.MaThietBiAlreadyExists;

        var thietBi = ThietBi.Create(
            request.MaThietBi,
            request.TenThietBi,
            request.LoaiThietBi,
            request.ViTri,
            request.NgayMua,
            request.NgayHetHanBaoHanh,
            request.GiaTriBanDau,
            request.GhiChu,
            request.ToaNhaId);

        await _thietBiRepository.AddThietBiAsync(thietBi, cancellationToken);
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
