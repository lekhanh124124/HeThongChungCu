using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.DieuPhoiNhanSu;

public class DieuPhoiNhanSuCommandHandler : ICommandHandler<DieuPhoiNhanSuCommand, YeuCauSuaChuaDetailResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly IDoiTacCommandRepository _doiTacRepository;
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DieuPhoiNhanSuCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        IDoiTacCommandRepository doiTacRepository,
        INhanVienCommandRepository nhanVienRepository,
        IYeuCauSuaChuaQueryRepository queryRepository,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _doiTacRepository = doiTacRepository;
        _nhanVienRepository = nhanVienRepository;
        _queryRepository = queryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaDetailResponse>> Handle(DieuPhoiNhanSuCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Request
        var ycsc = await _ycscRepository.GetByIdWithPersonnelAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return YeuCauSuaChuaErrors.NotFoundById(request.Id);

        // 2. Logic based on Assignment Type
        if (request.HopDongDoiTacId.HasValue)
        {
            // PARTNER ASSIGNMENT
            var hopDong = await _doiTacRepository.GetHopDongByIdAsync(request.HopDongDoiTacId.Value, cancellationToken);
            if (hopDong == null)
                return DoiTacErrors.NotFoundById(request.HopDongDoiTacId.Value);

            if (!hopDong.IsActive())
                return new Error("HopDongDoiTac.Inactive", "Hợp đồng đối tác hiện không còn hiệu lực để gán việc.");

            ycsc.AssignPartner(request.HopDongDoiTacId.Value);

            foreach (var ns in request.NhanSu)
            {
                ycsc.AddNhanSuPartner(ns.HoTen!, ns.SoCCCD!, ns.SoDienThoai, ns.VaiTro, ns.GhiChu);
            }
        }
        else
        {
            // INTERNAL ASSIGNMENT
            var nhanVienIds = request.NhanSu
                .Where(x => x.NhanVienId.HasValue)
                .Select(x => x.NhanVienId!.Value)
                .Distinct()
                .ToList();

            if (!nhanVienIds.Any())
                return new Error("DieuPhoiNhanSu.NoStaff", "Cần chọn ít nhất một nhân viên kỹ thuật nội bộ.");

            foreach (var nhanVienId in nhanVienIds)
            {
                var nhanVien = await _nhanVienRepository.GetByIdAsync(nhanVienId, cancellationToken);
                if (nhanVien == null)
                    return NhanVienErrors.NotFoundById(nhanVienId);
            }

            ycsc.AssignInternalStaff(nhanVienIds);
        }

        // 3. Persistence
        _ycscRepository.Update(ycsc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Build Response using Query Repository
        var result = await _queryRepository.GetByIdAsync(new GetYeuCauSuaChuaByIdSpecification(ycsc.Id), cancellationToken);

        return result != null
            ? Result.Success(result)
            : Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFoundById(ycsc.Id));
    }
}
