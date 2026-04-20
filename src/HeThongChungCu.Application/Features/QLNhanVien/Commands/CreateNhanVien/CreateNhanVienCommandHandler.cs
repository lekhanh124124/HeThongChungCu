using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLNhanVien.DTOs;
using HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.ValueObjects;
using System.Linq;

namespace HeThongChungCu.Application.Features.QLNhanVien.Commands.CreateNhanVien;

public class CreateNhanVienCommandHandler : ICommandHandler<CreateNhanVienCommand, NhanVienDetailResponse>
{
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly INhanVienQueryRepository _nhanVienQueryRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IHasherService _hasherService;
    private readonly ICodeGeneratorService _codeGeneratorService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNhanVienCommandHandler(
        INhanVienCommandRepository nhanVienRepository,
        INhanVienQueryRepository nhanVienQueryRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ITaiKhoanCommandRepository taiKhoanRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IHasherService hasherService,
        ICodeGeneratorService codeGeneratorService,
        IUnitOfWork unitOfWork)
    {
        _nhanVienRepository = nhanVienRepository;
        _nhanVienQueryRepository = nhanVienQueryRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _taiKhoanRepository = taiKhoanRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _hasherService = hasherService;
        _codeGeneratorService = codeGeneratorService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NhanVienDetailResponse>> Handle(CreateNhanVienCommand request, CancellationToken cancellationToken)
    {
        // 1. Check account existence
        var emailExists = await _taiKhoanRepository.AnyAsync(a => a.Email.Value == request.Email || a.TenDangNhap == request.Email, cancellationToken);
        if (emailExists)
            return UserErrors.EmailAlreadyExists;

        // 2. Check personal info existence
        if (!string.IsNullOrEmpty(request.CCCD))
        {
            var cccdExists = await _nguoiDungRepository.AnyAsync(u => u.CCCD == request.CCCD, cancellationToken);
            if (cccdExists)
                return UserErrors.IdCardAlreadyExists;
        }

        if (!string.IsNullOrEmpty(request.SoDienThoai))
        {
            var phoneExists = await _nguoiDungRepository.AnyAsync(u => u.SoDienThoai!.Value == request.SoDienThoai, cancellationToken);
            if (phoneExists)
                return UserErrors.PhoneNumberAlreadyExists;
        }

        var loaiNhanVien = LoaiNhanVien.FromValue(request.LoaiNhanVienId);
        if (loaiNhanVien == null)
            return NhanVienErrors.LoaiNhanVienInvalid(LoaiNhanVien.GetAll().Select(l => l.Name));

        var gioiTinh = GioiTinh.FromValue(request.GioiTinhId);
        if (gioiTinh == null)
            return UserErrors.InvalidGender(GioiTinh.GetAll().Select(g => g.Name));

        // 3. Process Documents (TaiLieuNguoiDung)
        var documents = new List<TaiLieuNguoiDung>();
        if (request.TaiLieus != null && request.TaiLieus.Count != 0)
        {
            var allFileIds = request.TaiLieus.SelectMany(d => d.FileIds).Distinct().ToList();
            var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
            var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

            foreach (var docReq in request.TaiLieus)
            {
                var loaiGiayTo = LoaiGiayTo.FromValue(docReq.LoaiGiayToId);
                if (loaiGiayTo is null) continue;

                var files = docReq.FileIds
                    .Where(id => tepTaiLieuDict.ContainsKey(id))
                    .Select(id => tepTaiLieuDict[id])
                    .Select(f => f is TepTaiLieuNguoiDung tp ? tp : new TepTaiLieuNguoiDung(f.FileName, f.FileUrl, f.Size, f.ContentType))
                    .ToList();

                documents.Add(new TaiLieuNguoiDung(
                    null, // NguoiDungId will be populated by EF
                    loaiGiayTo,
                    docReq.SoGiayTo,
                    docReq.NgayPhatHanh,
                    files));
            }
        }

        // 4. Create NguoiDung (User Profile)
        var nguoiDung = NguoiDung.CreateNguoiDung(
            request.Ten,
            request.Ho,
            request.NgaySinh,
            gioiTinh,
            request.DiaChi,
            request.CCCD,
            request.SoDienThoai != null ? new SoDienThoai(request.SoDienThoai) : null,
            documents);

        await _nguoiDungRepository.AddAsync(nguoiDung, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Generate Credentials
        var plainPassword = _codeGeneratorService.GenerateRandomPassword(8);
        var maNhanVien = await _codeGeneratorService.GenerateAsync<NhanVien>("NV", x => x.MaNhanVien);

        // 6. Create NhanVien (Staff)
        var nhanVien = Domain.Entities.NhanVien.CreateNhanVien(
            nguoiDung.Id,
            loaiNhanVien,
            maNhanVien,
            request.NgayVaoLam,
            request.GhiChu);

        await _nhanVienRepository.AddAsync(nhanVien, cancellationToken);

        // 7. Create TaiKhoan (Account)
        var hashedPassword = _hasherService.HashPassword(plainPassword);
        var taiKhoan = TaiKhoan.CreateNhanVienAccount(
            nguoiDung.Id,
            request.Email,
            request.Email,
            hashedPassword,
            request.AnhDaiDienId,
            $"{request.Ho} {request.Ten}",
            plainPassword);

        await _taiKhoanRepository.AddAsync(taiKhoan, cancellationToken);

        // 8. Finalize transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return full response
        var response = await _nhanVienQueryRepository.GetByIdAsync(new GetNhanVienByIdSpecification(nhanVien.Id), cancellationToken);

        return response != null
            ? Result.Success(response)
            : Result.Failure<NhanVienDetailResponse>(NhanVienErrors.NotFound);
    }
}
