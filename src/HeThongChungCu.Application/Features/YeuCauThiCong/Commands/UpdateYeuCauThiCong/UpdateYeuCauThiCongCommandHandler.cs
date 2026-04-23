using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.UpdateYeuCauThiCong;

public class UpdateYeuCauThiCongCommandHandler : ICommandHandler<UpdateYeuCauThiCongCommand, YeuCauThiCongResponse>
{
    private readonly IYeuCauThiCongCommandRepository _yctcCommandRepository;
    private readonly IYeuCauThiCongQueryRepository _yctcQueryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateYeuCauThiCongCommandHandler(
        IYeuCauThiCongCommandRepository yctcCommandRepository,
        IYeuCauThiCongQueryRepository yctcQueryRepository,
        ICurrentUserService currentUserService,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _yctcCommandRepository = yctcCommandRepository;
        _yctcQueryRepository = yctcQueryRepository;
        _currentUserService = currentUserService;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauThiCongResponse>> Handle(UpdateYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return UserErrors.NotFound;

        var yctc = await _yctcCommandRepository.GetByIdWithAllAsync(command.Id, cancellationToken);

        if (yctc is null)
            return YeuCauThiCongErrors.NotFound;

        if (yctc.CreatedBy != userId)
            return YeuCauThiCongErrors.Forbidden;

        // 1. Update basic info
        var updateResult = yctc.CapNhatThongTinThiCong(
            command.HangMucThiCong,
            command.DuKienBatDau,
            command.DuKienKetThuc,
            command.NoiDung,
            command.TenDonViThiCong,
            command.NguoiDaiDien,
            command.SoDienThoaiDaiDien);

        if (updateResult.IsFailure)
            return updateResult.Errors;

        // 2. Sync Personnel
        if (command.DanhSachNhanSu != null)
        {
            // Remove missing
            var incomingIds = command.DanhSachNhanSu.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToList();
            var staffToRemove = yctc.NhanSuThiCongs.Where(x => !incomingIds.Contains(x.Id)).ToList();
            foreach (var staff in staffToRemove)
            {
                var result = yctc.RemoveNhanSu(staff.Id, "Cập nhật hồ sơ");
                if (result.IsFailure) return result.Errors;
            }

            // Add or Update
            foreach (var ns in command.DanhSachNhanSu)
            {
                if (ns.Id.HasValue && yctc.NhanSuThiCongs.Any(x => x.Id == ns.Id.Value))
                {
                    var result = yctc.UpdateNhanSu(ns.Id.Value, ns.HoTen, ns.SoCCCD, ns.SoDienThoai, ns.VaiTro, ns.GhiChu);
                    if (result.IsFailure) return result.Errors;
                }
                else
                {
                    var result = yctc.AddNhanSu(ns.HoTen, ns.SoCCCD, ns.SoDienThoai, ns.VaiTro, ns.GhiChu);
                    if (result.IsFailure) return result.Errors;
                }
            }
        }

        // 3. Sync Files
        if (command.DanhSachTepIds != null)
        {
            var incomingTepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(command.DanhSachTepIds, cancellationToken);
            var incomingUrls = incomingTepTaiLieus.Select(x => x.FileUrl).ToList();

            // Remove missing files based on URL
            var filesToRemove = yctc.TepYeuCauThiCongs
                .Where(x => !incomingUrls.Contains(x.FileUrl)).ToList();
            
            foreach (var file in filesToRemove)
            {
                var result = yctc.RemoveTep(file.Id);
                if (result.IsFailure) return result.Errors;
            }

            // Add new files based on URL
            var currentUrls = yctc.TepYeuCauThiCongs.Select(x => x.FileUrl).ToList();
            var newTepsToAdd = incomingTepTaiLieus.Where(x => !currentUrls.Contains(x.FileUrl)).ToList();
            
            foreach (var f in newTepsToAdd)
            {
                var newTep = f is TepYeuCauThiCong tysc ? tysc : new TepYeuCauThiCong(f.FileName, f.FileUrl, f.Size, f.ContentType);
                var result = yctc.AddTep(newTep);
                if (result.IsFailure) return result.Errors;
            }
        }

        // 4. Handle Withdraw
        if (command.IsWithdraw)
        {
            var withdrawResult = yctc.Withdraw();
            if (withdrawResult.IsFailure)
                return withdrawResult.Errors;
        }
        // 5. Handle Submit
        else if (command.IsSubmit)
        {
            var submitResult = yctc.Submit();
            if (submitResult.IsFailure)
                return submitResult.Errors;
        }

        _yctcCommandRepository.Update(yctc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _yctcQueryRepository.GetByIdAsync(new GetYeuCauThiCongByIdSpecification(yctc.Id), cancellationToken);
        return response != null
            ? response
            : YeuCauThiCongErrors.NotFound;
    }
}
