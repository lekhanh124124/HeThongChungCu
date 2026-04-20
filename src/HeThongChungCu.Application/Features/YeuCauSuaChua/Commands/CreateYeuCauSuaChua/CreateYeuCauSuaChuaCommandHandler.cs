using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using YeuCauSuaChuaEntity = HeThongChungCu.Domain.Entities.YeuCauSuaChua;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;

public class CreateYeuCauSuaChuaCommandHandler : ICommandHandler<CreateYeuCauSuaChuaCommand, YeuCauSuaChuaDetailResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateYeuCauSuaChuaCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        ICanHoCommandRepository canHoRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IYeuCauSuaChuaQueryRepository queryRepository,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _canHoRepository = canHoRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _queryRepository = queryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaDetailResponse>> Handle(CreateYeuCauSuaChuaCommand request, CancellationToken cancellationToken)
    {
        // 1. Domain Existence Validation
        var canHo = await _canHoRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        if (canHo == null)
            return Result.Failure<YeuCauSuaChuaDetailResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        // 2. Fetch Files
        var tepTaiLieus = request.DanhSachTepIds != null && request.DanhSachTepIds.Count != 0
            ? await _tepTaiLieuRepository.GetByIdsAsync(request.DanhSachTepIds, cancellationToken)
            : [];

        var tepYeuCauSuaChuas = tepTaiLieus.Select(f =>
            f is TepYeuCauSuaChua tysc ? tysc : new TepYeuCauSuaChua(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList();

        // 3. Create Entity
        var initialStatus = request.IsSubmit ? TrangThaiYeuCau.Pending : TrangThaiYeuCau.Saved;
        var ycsc = YeuCauSuaChuaEntity.Create(
            request.CanHoId,
            PhamViSuaChua.FromValue(request.PhamViId)!,
            LoaiSuCoKyThuat.FromValue(request.LoaiSuCoId)!,
            request.NoiDung!,
            request.MoTaViTri,
            tepYeuCauSuaChuas,
            initialStatus);

        // 4. Persistence
        await _ycscRepository.AddAsync(ycsc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build Response using Query Repository
        var result = await _queryRepository.GetByIdAsync(new GetYeuCauSuaChuaByIdSpecification(ycsc.Id), cancellationToken);

        return result != null
            ? Result.Success(result)
            : Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFoundById(ycsc.Id));
    }
}
