using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CreateYeuCauThiCong;

public class CreateYeuCauThiCongCommandHandler : ICommandHandler<CreateYeuCauThiCongCommand, YeuCauThiCongResponse>
{
    private readonly IYeuCauThiCongCommandRepository _yctcCommandRepository;
    private readonly IYeuCauThiCongQueryRepository _yctcQueryRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateYeuCauThiCongCommandHandler(
        IYeuCauThiCongCommandRepository yctcCommandRepository,
        IYeuCauThiCongQueryRepository yctcQueryRepository,
        ICanHoCommandRepository canHoRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _yctcCommandRepository = yctcCommandRepository;
        _yctcQueryRepository = yctcQueryRepository;
        _canHoRepository = canHoRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauThiCongResponse>> Handle(CreateYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        // 1. Domain Existence Validation
        var canHo = await _canHoRepository.GetByIdAsync(command.CanHoId, cancellationToken);
        if (canHo == null)
            return CanHoErrors.NotFoundById(command.CanHoId);

        // 2. Fetch Files
        var tepTaiLieus = command.DanhSachTepIds != null && command.DanhSachTepIds.Count != 0
            ? await _tepTaiLieuRepository.GetByIdsAsync(command.DanhSachTepIds, cancellationToken)
            : [];

        var tepYeuCauThiCongs = tepTaiLieus.Select(f =>
            f is TepYeuCauThiCong tysc ? tysc : new TepYeuCauThiCong(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList();

        // 3. Create Entity
        var initialStatus = command.IsSubmit ? TrangThaiYeuCau.Pending : TrangThaiYeuCau.Saved;
        var yctc = Domain.Entities.YeuCauThiCong.Create(
            command.CanHoId,
            command.HangMucThiCong,
            command.DuKienBatDau,
            command.DuKienKetThuc,
            command.NoiDung,
            command.TenDonViThiCong,
            command.NguoiDaiDien,
            command.SoDienThoaiDaiDien,
            initialStatus);

        // 3.1. Add Files
        foreach (var tep in tepYeuCauThiCongs)
        {
            var result = yctc.AddTep(tep);
            if (result.IsFailure) return result.Errors;
        }

        // 4. Add Personnel
        if (command.DanhSachNhanSu != null)
        {
            foreach (var ns in command.DanhSachNhanSu)
            {
                var result = yctc.AddNhanSu(ns.HoTen, ns.SoCCCD, ns.SoDienThoai, ns.VaiTro, ns.GhiChu);
                if (result.IsFailure) return result.Errors;
            }
        }

        // 5. Persistence
        await _yctcCommandRepository.AddAsync(yctc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build Response using Query Repository
        var response = await _yctcQueryRepository.GetByIdAsync(new GetYeuCauThiCongByIdSpecification(yctc.Id), cancellationToken);

        return response != null
            ? response
            : YeuCauThiCongErrors.NotFound;
    }
}
