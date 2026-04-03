using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public class UpdateCanHoCommandHandler : ICommandHandler<UpdateCanHoCommand, CanHoDetailResponse>
{
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly IQuanHeCuTruCommandRepository _quanHeCuTruRepository;
    private readonly ICanHoDomainService _canHoDomainService;
    private readonly IResidencyService _residencyService;

    public UpdateCanHoCommandHandler(
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        ICanHoDomainService canHoDomainService,
        IResidencyService residencyService)
    {
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _canHoDomainService = canHoDomainService;
        _residencyService = residencyService;
    }

    public async Task<Result<CanHoDetailResponse>> Handle(UpdateCanHoCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (canHo is null)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.NotFoundById(request.Id));

        var loaiCanHo = LoaiCanHo.FromValue(request.LoaiCanHoId);
        var tinhTrangCanHo = TrangThaiCanHo.FromValue(request.TinhTrangCanHoId);

        var relations = await _quanHeCuTruRepository.GetByCanHoIdAsync(canHo.Id, cancellationToken);

        // 1. Kiểm tra logic cư trú (Có được phép sửa/xóa khi có người ở không)
        // Lưu ý: Chỉ chặn nếu có thay đổi cấu trúc quan trọng (Diện tích, số phòng, loại căn hộ)
        bool isStructureChanged = request.DienTich != canHo.DienTich ||
                                  request.SoPhongNgu != canHo.SoPhongNgu ||
                                  request.SoPhongTam != canHo.SoPhongTam ||
                                  loaiCanHo != canHo.LoaiCanHoId;

        if (isStructureChanged)
        {
            var residencyCheck = _residencyService.CheckCanUpdateOrDeleteCanHo(canHo, relations);
            if (residencyCheck.IsFailure)
                return Result.Failure<CanHoDetailResponse>(residencyCheck.Errors);
        }

        // 2. Kiểm tra logic cấu trúc (Mã căn hộ trùng lặp)
        var maExists = false;
        if (request.MaCanHo != canHo.MaCanHo)
        {
            maExists = await _canHoRepository.MaCanHoExistsAsync(request.MaCanHo, cancellationToken);
        }

        var structureCheck = _canHoDomainService.CanUpdateStructure(canHo, request.MaCanHo, maExists, false); // hasActiveResidents truyền false vì đã check ở bước 1
        if (structureCheck.IsFailure)
            return Result.Failure<CanHoDetailResponse>(structureCheck.Errors);

        canHo.UpdateInfo(
            request.TenCanHo, 
            request.MaCanHo,
            request.DienTich, 
            request.SoPhongNgu, 
            request.SoPhongTam, 
            loaiCanHo!);

        canHo.UpdateStatus(tinhTrangCanHo!);

        _canHoRepository.Update(canHo);

        var toaNha = await _toaNhaRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken);
        var tang = toaNha!.Tangs.First(t => t.Id == canHo.TangId);

        return Result.Success(new CanHoDetailResponse
        {
            Id = canHo.Id,
            MaCanHo = canHo.MaCanHo,
            TenCanHo = canHo.TenCanHo,
            DienTich = canHo.DienTich,
            TangId = canHo.TangId,
            TenTang = tang.TenTang,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            LoaiCanHoId = canHo.LoaiCanHoId.Value,
            TenLoaiCanHo = canHo.LoaiCanHoId.Name,
            TinhTrangCanHoId = canHo.TinhTrangCanHoId.Value,
            TenTinhTrangCanHo = canHo.TinhTrangCanHoId.Name
        });
    }
}
