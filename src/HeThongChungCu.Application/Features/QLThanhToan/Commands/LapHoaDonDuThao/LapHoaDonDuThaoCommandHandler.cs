using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Common;

using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public class LapHoaDonDuThaoCommandHandler : ICommandHandler<LapHoaDonDuThaoCommand, LapHoaDonDuThaoResponse>
{
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IDangKyDichVuCommandRepository _dangKyRepository;
    private readonly IQuanHeCuTruCommandRepository _cuTruRepository;
    private readonly IDotThanhToanCommandRepository _dotRepository;
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IPhuongTienCommandRepository _phuongTienRepository;
    private readonly IBillingDomainService _billingService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    private readonly List<IChargeSource> _chargeSources;

    public LapHoaDonDuThaoCommandHandler(
        ICanHoCommandRepository canHoRepository,
        IDichVuCommandRepository dichVuRepository,
        IChiSoTieuThuCommandRepository chiSoRepository,
        IDangKyDichVuCommandRepository dangKyRepository,
        IQuanHeCuTruCommandRepository cuTruRepository,
        IDotThanhToanCommandRepository dotRepository,
        IHoaDonCommandRepository hoaDonRepository,
        IPhuongTienCommandRepository phuongTienRepository,
        IBillingDomainService billingService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _canHoRepository = canHoRepository;
        _dichVuRepository = dichVuRepository;
        _chiSoRepository = chiSoRepository;
        _dangKyRepository = dangKyRepository;
        _cuTruRepository = cuTruRepository;
        _dotRepository = dotRepository;
        _hoaDonRepository = hoaDonRepository;
        _phuongTienRepository = phuongTienRepository;
        _billingService = billingService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;

        // Initialize Internal Strategies
        _chargeSources = new List<IChargeSource>
        {
            new MandatoryChargeSource(_billingService),
            new ConsumptionChargeSource(_billingService, _chiSoRepository),
            new SubscriptionChargeSource(_billingService),
            new VehicleChargeSource(_billingService)
        };
    }

    public async Task<Result<LapHoaDonDuThaoResponse>> Handle(LapHoaDonDuThaoCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin đợt thanh toán
        var dot = await _dotRepository.GetByIdAsync(request.DotThanhToanId, cancellationToken);
        if (dot == null)
            return DotThanhToanErrors.NotFound;

        if (dot.TrangThaiDotThanhToanId != TrangThaiDotThanhToan.DaDuyet)
            return new Error("DotThanhToan.InvalidStatus", "Không thể tạo hóa đơn khi đợt thanh toán không ở trạng thái đã duyệt.");

        var ky = dot.KyThanhToan;
        // Thời điểm bắt đầu kỳ thanh toán — dùng để xác định hóa đơn nào chưa được tính lãi trong kỳ này
        var dotStartDate = new DateTimeOffset(ky.Nam, ky.Thang, 1, 0, 0, 0, TimeSpan.Zero);

        // 2. Load dữ liệu nguồn (Batch loading)
        var canHos = await _canHoRepository.GetAllActiveAsync(cancellationToken);
        var activeCanHoIds = canHos.Select(c => c.Id).ToList();

        var billingData = await LoadBillingDataAsync(activeCanHoIds, ky, dotStartDate, cancellationToken);

        // 3. Khởi tạo danh sách hóa đơn mới
        var newInvoices = new List<HoaDon>();
        var ngayHan = _dateTimeProvider.Now.AddDays(15);
        // Danh sách hóa đơn quá hạn đã được tính lãi trong lần này — để cập nhật NgayTinhLaiCuoi sau khi save
        var overdueInvoicesToUpdate = new List<HoaDon>();

        foreach (var canHo in canHos)
        {
            if (billingData.ExistingInvoiceCanHoIds.Contains(canHo.Id))
                continue;

            string maHoaDon = $"HD-{canHo.MaCanHo}-{ky.Thang:D2}{ky.Nam}";
            var hoaDonResult = _billingService.CreateInvoiceHeader(canHo, dot, ky, maHoaDon, ngayHan);
            if (hoaDonResult.IsFailure) continue;

            var hoaDon = hoaDonResult.Value;

            // Apply Internal Strategy Pattern
            bool hasDetails = false;
            foreach (var source in _chargeSources)
            {
                if (source.AttachCharges(hoaDon, canHo, billingData))
                {
                    hasDetails = true;
                }
            }

            // Gắn lãi trễ hạn cho các hóa đơn quá hạn của căn hộ này
            if (billingData.LateInterestBangGia != null)
            {
                var overdueForCanHo = billingData.OverdueInvoices[canHo.Id].ToList();
                foreach (var overdueInvoice in overdueForCanHo)
                {
                    _billingService.AttachLateInterestDetail(hoaDon, overdueInvoice, billingData.LateInterestBangGia, _dateTimeProvider.Now);
                    overdueInvoicesToUpdate.Add(overdueInvoice);
                    hasDetails = true;
                }
            }

            if (hasDetails)
            {
                newInvoices.Add(hoaDon);
            }
        }

        // 4. Lưu dữ liệu 
        if (newInvoices.Count != 0)
        {
            await _hoaDonRepository.AddRangeAsync(newInvoices, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Sau khi đã có ID hóa đơn, cập nhật trạng thái các bản ghi chỉ số tiêu thu
            // Lấy danh sách cần link từ ConsumptionChargeSource
            var consumptionSource = _chargeSources.OfType<ConsumptionChargeSource>().FirstOrDefault();
            if (consumptionSource != null && consumptionSource.ConsumptionsToLink.Count != 0)
            {
                var allConsumptionRecords = billingData.ConsumptionRecords.SelectMany(x => x).ToList();
                var updatedRecords = new List<ChiSoTieuThu>();

                foreach (var link in consumptionSource.ConsumptionsToLink)
                {
                    var record = allConsumptionRecords.FirstOrDefault(r => r.Id == link.ChiSoId);
                    if (record != null)
                    {
                        record.MarkAsBilled(link.HoaDon.Id);
                        _chiSoRepository.Update(record);
                        updatedRecords.Add(record);
                    }
                }

                if (updatedRecords.Count != 0)
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            // 6. Đánh dấu NgayTinhLaiCuoi trên các hóa đơn quá hạn đã được tính lãi
            if (overdueInvoicesToUpdate.Count != 0)
            {
                var now = _dateTimeProvider.Now;
                foreach (var overdueInvoice in overdueInvoicesToUpdate)
                {
                    overdueInvoice.SetNgayTinhLai(now);
                    _hoaDonRepository.Update(overdueInvoice);
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        // Cập nhật trạng thái đợt thanh toán sang Đã lập dự thảo
        dot.MarkAsDraftGenerated();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new LapHoaDonDuThaoResponse
        {
            SoLuongHoaDonTaoMoi = newInvoices.Count,
            DotThanhToanId = dot.Id,
            TenDotThanhToan = dot.TenDot
        });
    }

    private async Task<BillingDataBundle> LoadBillingDataAsync(
        List<int> activeCanHoIds,
        KyThanhToan ky,
        DateTimeOffset dotStartDate,
        CancellationToken cancellationToken)
    {
        var periodicServices = await _dichVuRepository.GetActivePeriodicServicesWithPriceListsAsync(cancellationToken);

        // Lấy DichVu lãi trễ hạn và BangGia hiện hành (nếu đã được seed)
        var lateInterestDichVu = periodicServices.FirstOrDefault(s => s.LoaiDichVuId == LoaiDichVu.PhatTreHan);
        var lateInterestBangGia = lateInterestDichVu?.GetCurrentPrice(_dateTimeProvider.Now);

        return new BillingDataBundle(
            PeriodicServiceDict: periodicServices.ToDictionary(s => s.Id),
            MandatoryServices: periodicServices.Where(s => s.IsBatBuoc && s.LoaiDichVuId != LoaiDichVu.PhatTreHan).ToList(),
            ResidencyRelations: (await _cuTruRepository.GetByCanHoIdsAsync(activeCanHoIds, cancellationToken)).ToLookup(x => x.CanHoId),
            ConsumptionRecords: (await _chiSoRepository.GetLockedUnbilledByPeriodAsync(ky, cancellationToken)).ToLookup(x => x.CanHoId),
            Subscriptions: (await _dangKyRepository.GetActiveByCanHoIdsAsync(activeCanHoIds, cancellationToken)).ToLookup(x => x.CanHoId),
            ActiveVehicles: (await _phuongTienRepository.GetActiveByCanHoIdsAsync(activeCanHoIds, cancellationToken)).ToLookup(x => x.CanHoId),
            ExistingInvoiceCanHoIds: await _hoaDonRepository.GetExistingCanHoIdsByKyAsync(ky, cancellationToken),
            OverdueInvoices: await _hoaDonRepository.GetOverdueByCanHoIdsAsync(activeCanHoIds, dotStartDate, cancellationToken),
            LateInterestBangGia: lateInterestBangGia
        );
    }
}
