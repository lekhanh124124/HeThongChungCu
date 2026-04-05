using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Events;
using System.Linq;

namespace HeThongChungCu.Application.Features.NhanVien.Commands.CreateNhanVien;

public class CreateNhanVienCommandHandler : ICommandHandler<CreateNhanVienCommand, NhanVienResponse>
{
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly INhanVienQueryRepository _nhanVienQueryRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IHasherService _hasherService;
    private readonly ICodeGeneratorService _codeGeneratorService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNhanVienCommandHandler(
        INhanVienCommandRepository nhanVienRepository,
        INhanVienQueryRepository nhanVienQueryRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ITaiKhoanCommandRepository taiKhoanRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
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

    public async Task<Result<NhanVienResponse>> Handle(CreateNhanVienCommand request, CancellationToken cancellationToken)
    {
        // 1. Check account existence
        var emailExists = await _taiKhoanRepository.AnyAsync(a => a.Email == request.Email || a.TenDangNhap == request.Email, cancellationToken);
        if (emailExists)
            return Result.Failure<NhanVienResponse>(UserErrors.EmailAlreadyExists);

        // 2. Check personal info existence
        if (!string.IsNullOrEmpty(request.CCCD))
        {
            var cccdExists = await _nguoiDungRepository.AnyAsync(u => u.CCCD == request.CCCD, cancellationToken);
            if (cccdExists)
                return Result.Failure<NhanVienResponse>(UserErrors.IdCardAlreadyExists);
        }

        if (!string.IsNullOrEmpty(request.SoDienThoai))
        {
            var phoneExists = await _nguoiDungRepository.AnyAsync(u => u.SoDienThoai == request.SoDienThoai, cancellationToken);
            if (phoneExists)
                return Result.Failure<NhanVienResponse>(UserErrors.PhoneNumberAlreadyExists);
        }

        var loaiNhanVien = LoaiNhanVien.FromValue(request.LoaiNhanVienId);
        if (loaiNhanVien == null)
            return Result.Failure<NhanVienResponse>(NhanVienErrors.LoaiNhanVienInvalid(LoaiNhanVien.GetAll().Select(l => l.Name)));

        var gioiTinh = GioiTinh.FromValue(request.GioiTinhId);
        if (gioiTinh == null)
            return Result.Failure<NhanVienResponse>(UserErrors.InvalidGender(GioiTinh.GetAll().Select(g => g.Name)));

        // 3. Create NguoiDung (User Profile)
        var nguoiDung = new NguoiDung(
            request.Ten,
            request.Ho,
            request.NgaySinh,
            gioiTinh,
            request.DiaChi,
            request.CCCD,
            request.SoDienThoai);

        await _nguoiDungRepository.AddAsync(nguoiDung, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken); // Need ID for TaiLieu/Account

        // 4. Process Documents (TaiLieuNguoiDung)
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
                    .ToList();

                var document = new TaiLieuNguoiDung(
                    nguoiDung.Id,
                    loaiGiayTo,
                    docReq.SoGiayTo,
                    docReq.NgayPhatHanh,
                    files.Select(f => f is TepTaiLieuNguoiDung tp ? tp : new TepTaiLieuNguoiDung(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList());

                nguoiDung.AddDocument(document);
            }
        }

        // 5. Generate Credentials
        var plainPassword = _codeGeneratorService.GenerateRandomPassword(8);
        var maNhanVien = await _codeGeneratorService.GenerateAsync<Domain.Entities.NhanVien>("NV", x => x.MaNhanVien);

        // 6. Create TaiKhoan (Account)
        var hashedPassword = _hasherService.HashPassword(plainPassword);
        var taiKhoan = new TaiKhoan(
            nguoiDung.Id,
            request.Email, // Use Email as username for employees by default
            request.Email,
            hashedPassword);
        
        taiKhoan.AddRole(Role.Staff);
        await _taiKhoanRepository.AddAsync(taiKhoan, cancellationToken);

        // 7. Create NhanVien (Staff)
        var nhanVien = new Domain.Entities.NhanVien(
            nguoiDung.Id,
            loaiNhanVien,
            maNhanVien,
            request.NgayVaoLam,
            request.GhiChu);

        // Add Domain Event for Email Notification
        nhanVien.AddDomainEvent(new NhanVienCreatedEvent(
            request.Email,
            $"{request.Ho} {request.Ten}",
            request.Email,
            plainPassword));

        await _nhanVienRepository.AddAsync(nhanVien, cancellationToken);
        
        // 8. Finalize transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return full response
        var response = await _nhanVienQueryRepository.GetByIdAsync(new GetNhanVienByIdSpecification(nhanVien.Id), cancellationToken);
        
        return response != null 
            ? Result.Success(response) 
            : Result.Failure<NhanVienResponse>(NhanVienErrors.NotFound);
    }
}
