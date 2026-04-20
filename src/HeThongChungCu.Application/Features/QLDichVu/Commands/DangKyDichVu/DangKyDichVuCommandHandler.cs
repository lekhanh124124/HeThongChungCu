using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DangKyDichVu;

internal sealed class DangKyDichVuCommandHandler : ICommandHandler<DangKyDichVuCommand, int>
{
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IDangKyDichVuCommandRepository _dangKyDichVuRepository;
    private readonly IDichVuDomainService _dichVuDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public DangKyDichVuCommandHandler(
        IDichVuCommandRepository dichVuRepository,
        IDangKyDichVuCommandRepository dangKyDichVuRepository,
        IDichVuDomainService dichVuDomainService,
        IUnitOfWork unitOfWork)
    {
        _dichVuRepository = dichVuRepository;
        _dangKyDichVuRepository = dangKyDichVuRepository;
        _dichVuDomainService = dichVuDomainService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(DangKyDichVuCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin dịch vụ (đã include KhungGios từ Repo)
        var dichVu = await _dichVuRepository.GetByIdAsync(request.DichVuId, cancellationToken);
        if (dichVu == null)
        {
            return DichVuErrors.NotFoundById(request.DichVuId);
        }

        // 2. Xử lý thông tin khung giờ (nếu có)
        KhungGioDichVu? khungGio = null;
        if (request.KhungGioId.HasValue)
        {
            khungGio = dichVu.KhungGios.FirstOrDefault(x => x.Id == request.KhungGioId.Value);
            if (khungGio == null)
            {
                return DichVuErrors.KhungGioNotFound;
            }
        }

        // 3. Tính toán số lượng đang sử dụng (Capacity)
        var currentPrice = dichVu.GetCurrentPrice(request.NgaySuDung.DateTime);
        bool isTheoKhungGio = currentPrice?.LoaiDinhGiaId == LoaiDinhGia.TheoKhungGio;

        int sumHienTai;
        if (isTheoKhungGio && khungGio != null)
        {
            // Kiểm tra capacity theo Slot + Ngày cụ thể
            sumHienTai = await _dangKyDichVuRepository.GetSumActiveQuantityByKhungGioAsync(
                dichVu.Id, 
                khungGio.GioBatDau,
                khungGio.GioKetThuc,
                request.NgaySuDung.DateTime, 
                cancellationToken);
        }
        else
        {
            // Kiểm tra capacity tổng quát (cho các trường hợp khác)
            sumHienTai = await _dangKyDichVuRepository.GetSumActiveQuantityByDichVuIdAsync(request.DichVuId, cancellationToken);
        }

        // 4. Gọi Domain Service để kiểm tra tính hợp lệ (Trạng thái + Thứ trong tuần + Sức chứa)
        var validationResult = _dichVuDomainService.CanRegister(
            dichVu, 
            sumHienTai, 
            request.SoLuong, 
            request.NgaySuDung, 
            khungGio);

        if (validationResult.IsFailure)
        {
            return validationResult.Errors;
        }

        // 5. Tạo bản ghi đăng ký mới
        var dangKy = new Domain.Entities.DangKyDichVu(
            request.CanHoId,
            request.DichVuId,
            request.NgaySuDung.DateTime, // Chuyển sang DateTime vì Entity mong muốn DateTime
            request.SoLuong,
            khungGio);

        // Kích hoạt (DangSuDung) luôn sau khi kiểm tra OK
        dangKy.UpdateStatus(TrangThaiDangKy.DangSuDung);

        await _dangKyDichVuRepository.AddAsync(dangKy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return dangKy.Id;
    }
}

