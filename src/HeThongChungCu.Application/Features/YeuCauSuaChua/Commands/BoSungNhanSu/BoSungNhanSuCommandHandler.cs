using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BoSungNhanSu;

public class BoSungNhanSuCommandHandler : ICommandHandler<BoSungNhanSuCommand, YeuCauSuaChuaDetailResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BoSungNhanSuCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        INhanVienCommandRepository nhanVienRepository,
        IYeuCauSuaChuaQueryRepository queryRepository,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _nhanVienRepository = nhanVienRepository;
        _queryRepository = queryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaDetailResponse>> Handle(BoSungNhanSuCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Request
        var ycsc = await _ycscRepository.GetByIdWithPersonnelAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFoundById(request.Id));

        // 2. Logic based on Current Assignment
        if (ycsc.HopDongDoiTacId != null)
        {
            // Currently assigned to Partner
            foreach (var ns in request.NhanSu)
            {
                if (ns.NhanVienId.HasValue)
                    return Result.Failure<YeuCauSuaChuaDetailResponse>(new Error("BoSungNhanSu.InvalidStaff",
                        "Yêu cầu này đang được xử lý bởi đối tác, không thể bổ sung nhân sự nội bộ."));

                ycsc.AddNhanSuPartner(ns.HoTen!, ns.SoCCCD!, ns.SoDienThoai, ns.VaiTro, ns.GhiChu);
            }
        }
        else
        {
            // Currently assigned to Internal Staff
            foreach (var ns in request.NhanSu)
            {
                if (!ns.NhanVienId.HasValue)
                    return Result.Failure<YeuCauSuaChuaDetailResponse>(new Error("BoSungNhanSu.InvalidPartnerStaff",
                        "Yêu cầu này đang được xử lý nội bộ, không thể bổ sung thợ đối tác."));

                var nhanVien = await _nhanVienRepository.GetByIdAsync(ns.NhanVienId.Value, cancellationToken);
                if (nhanVien == null)
                    return Result.Failure<YeuCauSuaChuaDetailResponse>(NhanVienErrors.NotFoundById(ns.NhanVienId.Value));

                ycsc.AddNhanSuNoiBo(ns.NhanVienId.Value);
            }
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
