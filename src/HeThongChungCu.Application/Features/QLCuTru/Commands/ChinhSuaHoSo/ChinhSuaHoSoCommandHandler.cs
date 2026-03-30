using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ChinhSuaHoSo;

public class ChinhSuaHoSoCommandHandler : ICommandHandler<ChinhSuaHoSoCommand, UserInfoResponse>
{
    private readonly IQuanHeCuTruEFRepository _quanHeCuTruRepository;
    private readonly INguoiDungEFRepository _userRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChinhSuaHoSoCommandHandler(
        IQuanHeCuTruEFRepository quanHeCuTruRepository,
        INguoiDungEFRepository userRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _userRepository = userRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserInfoResponse>> Handle(ChinhSuaHoSoCommand request, CancellationToken cancellationToken)
    {
        var relation = await _quanHeCuTruRepository.GetByIdAsync(request.QuanHeCuTruId, cancellationToken);
        if (relation == null)
            return Result.Failure<UserInfoResponse>(GeneralErrors.NotFoundById(request.QuanHeCuTruId));

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

        // 2. Document Reconciliation Logic (similar to PheDuyetYeuCauCuTru)
        if (request.TaiLieuCuTrus != null)
        {
            var currentDocs = user.TaiLieu.ToList();
            var proposedDocs = request.TaiLieuCuTrus;

            // 1. Remove documents not in the request
            var proposedOriginalIds = proposedDocs.Where(d => d.TaiLieuCuTruId.HasValue)
                                                .Select(d => d.TaiLieuCuTruId!.Value)
                                                .ToList();
            
            foreach (var doc in currentDocs)
            {
                if (!proposedOriginalIds.Contains(doc.Id))
                {
                    user.RemoveDocument(doc.Id);
                }
            }

            // 2. Update existing or Add new
            var allFileIds = proposedDocs.SelectMany(d => d.FileIds).Distinct().ToList();
            var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
            var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

            foreach (var propDoc in proposedDocs)
            {
                var files = propDoc.FileIds
                    .Where(id => tepTaiLieuDict.ContainsKey(id))
                    .Select(id => tepTaiLieuDict[id])
                    .ToList();

                if (propDoc.TaiLieuCuTruId.HasValue)
                {
                    // Update existing
                    var existingDoc = user.TaiLieu.FirstOrDefault(d => d.Id == propDoc.TaiLieuCuTruId.Value);
                    if (existingDoc != null)
                    {
                        existingDoc.UpdateInfo(LoaiGiayTo.FromValue(propDoc.LoaiGiayToId)!, propDoc.SoGiayTo, propDoc.NgayPhatHanh);
                        existingDoc.SyncFiles(files);
                    }
                }
                else
                {
                    // Add new
                    var newDoc = new TaiLieuNguoiDung(
                        user.Id,
                        LoaiGiayTo.FromValue(propDoc.LoaiGiayToId)!,
                        propDoc.SoGiayTo,
                        propDoc.NgayPhatHanh,
                        files);
                    user.AddDocument(newDoc);
                }
            }
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserInfoResponse
        {
            Id = user.Id,
            FirstName = user.Ten,
            LastName = user.Ho,
            Dob = user.NgaySinh,
            GioiTinhId = user.GioiTinhId.Value,
            GioiTinhName = user.GioiTinhId.Name,
            DiaChi = user.DiaChi,
            IdCard = user.CCCD,
            PhoneNumber = user.SoDienThoai ?? string.Empty,
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
