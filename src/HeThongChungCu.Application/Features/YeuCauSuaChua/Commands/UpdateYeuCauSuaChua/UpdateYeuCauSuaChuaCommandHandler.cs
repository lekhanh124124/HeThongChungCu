using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.UpdateYeuCauSuaChua;

public class UpdateYeuCauSuaChuaCommandHandler : ICommandHandler<UpdateYeuCauSuaChuaCommand, YeuCauSuaChuaDetailResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateYeuCauSuaChuaCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IYeuCauSuaChuaQueryRepository queryRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _queryRepository = queryRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaDetailResponse>> Handle(UpdateYeuCauSuaChuaCommand request, CancellationToken cancellationToken)
    {
        // 1. Xác thực người dùng
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<YeuCauSuaChuaDetailResponse>(UserErrors.NotFound);

        // 2. Fetch aggregate
        var ycsc = await _ycscRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFoundById(request.Id));

        // 3. Guard quyền: chỉ người tạo mới được sửa
        if (ycsc.CreatedBy != userId)
            return Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.Forbidden);

        if (request.IsWithdraw)
        {
            // Thu hồi yêu cầu đã gửi (Pending -> Withdrawn)
            ycsc.Withdraw();
        }
        else
        {
            // Fetch file đính kèm mới nếu có
            List<TepYeuCauSuaChua>? danhSachTep = null;
            if (request.DanhSachTepIds != null)
            {
                var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(request.DanhSachTepIds, cancellationToken);
                danhSachTep = tepTaiLieus
                    .Select(f => f is TepYeuCauSuaChua tysc ? tysc : new TepYeuCauSuaChua(f.FileName, f.FileUrl, f.Size, f.ContentType))
                    .ToList();
            }

            // Resolve các Enum (null nếu không được cung cấp)
            var phamVi = request.PhamViId.HasValue ? PhamViSuaChua.FromValue(request.PhamViId.Value, null) : null;
            var loaiSuCo = request.LoaiSuCoId.HasValue ? LoaiSuCoKyThuat.FromValue(request.LoaiSuCoId.Value, null) : null;

            // Cập nhật nội dung (chỉ khi TrangThaiId == Saved)
            ycsc.Update(phamVi, loaiSuCo, request.NoiDung, request.MoTaViTri, danhSachTep);

            // Gửi yêu cầu (Saved -> Pending) nếu IsSubmit
            if (request.IsSubmit)
            {
                ycsc.Submit();
            }
        }

        // 4. Persistence
        _ycscRepository.Update(ycsc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build Response using Query Repository
        var result = await _queryRepository.GetByIdAsync(new GetYeuCauSuaChuaByIdSpecification(ycsc.Id), cancellationToken);

        return result != null
            ? Result.Success(result)
            : Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFoundById(ycsc.Id));
    }
}
