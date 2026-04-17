using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ChinhSuaHoSo;

public class ChinhSuaHoSoCommandHandler : ICommandHandler<ChinhSuaHoSoCommand, UserInfoResponse>
{
    private readonly IQuanHeCuTruCommandRepository _quanHeCuTruRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IDocumentReconciliationService _documentReconciliationService;
    private readonly IUnitOfWork _unitOfWork;

    public ChinhSuaHoSoCommandHandler(
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        INguoiDungCommandRepository userRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IDocumentReconciliationService documentReconciliationService,
        IUnitOfWork unitOfWork)
    {
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _userRepository = userRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _documentReconciliationService = documentReconciliationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserInfoResponse>> Handle(ChinhSuaHoSoCommand request, CancellationToken cancellationToken)
    {
        var relation = await _quanHeCuTruRepository.GetByIdAsync(request.QuanHeCuTruId, cancellationToken);
        if (relation == null)
            return Result.Failure<UserInfoResponse>(QuanHeCuTruErrors.NotFound);

        var user = await _userRepository.GetByIdWithDocumentsAsync(relation.NguoiDungId, cancellationToken);
        if (user == null)
            return Result.Failure<UserInfoResponse>(UserErrors.NotFound);

        // 1. Update personal info
        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.Dob,
            GioiTinh.FromValue(request.GioiTinhId)!,
            request.DiaChi,
            request.IdCard,
            request.PhoneNumber);

        // 2. Update relationship info
        relation.ThayDoiLoaiQuanHe(LoaiQuanHeCuTru.FromValue(request.LoaiQuanHeCuTruId)!);

        // 3. Document Reconciliation Logic via Domain Service
        if (request.TaiLieuCuTrus != null)
        {
            var allFileIds = request.TaiLieuCuTrus.SelectMany(d => d.FileIds).Distinct().ToList();
            var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);

            var proposedDocs = request.TaiLieuCuTrus.Select(d => new DocumentSyncItem(
                d.TaiLieuCuTruId,
                d.LoaiGiayToId,
                d.SoGiayTo,
                d.NgayPhatHanh.HasValue ? new DateTimeOffset(d.NgayPhatHanh.Value, TimeSpan.Zero) : null,
                d.FileIds
            ));

            _documentReconciliationService.ReconcileNguoiDungDocuments(user, proposedDocs, tepTaiLieus);
        }

        _userRepository.Update(user);
        _quanHeCuTruRepository.Update(relation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserInfoResponse
        {
            Id = user.Id,
            FirstName = user.Ten,
            LastName = user.Ho,
            Dob = user.NgaySinh,
            GioiTinhId = user.GioiTinhId.Value,
            GioiTinhName = user.GioiTinhId.Name,
            DiaChi = user.DiaChi.FullAddress,
            IdCard = user.CCCD,
            PhoneNumber = user.SoDienThoai ?? string.Empty,
            LoaiQuanHeCuTruId = relation.LoaiQuanHeCuTruId.Value,
            TenLoaiQuanHeCuTru = relation.LoaiQuanHeCuTruId.Name,
            TaiLieuCuTrus = user.TaiLieu.Select(d => new TaiLieuResponse
            {
                Id = d.Id,
                LoaiGiayToId = d.LoaiGiayToId.Value,
                TenLoaiGiayTo = d.LoaiGiayToId.Name,
                SoGiayTo = d.SoGiayTo,
                NgayPhatHanh = d.NgayPhatHanh,
                Files = d.Files.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
            }).ToList()
        });
    }
}
