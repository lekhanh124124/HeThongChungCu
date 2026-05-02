using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Constants;
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
        _billingService = billingService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;

        // Initialize Internal Strategies
        _chargeSources = new List<IChargeSource>
        {
            new MandatoryChargeSource(_billingService),
            new ConsumptionChargeSource(_billingService, _chiSoRepository),
            new SubscriptionChargeSource(_billingService)
        };
    }

    public async Task<Result<LapHoaDonDuThaoResponse>> Handle(LapHoaDonDuThaoCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin đợt thanh toán
        var dot = await _dotRepository.GetByIdAsync(request.DotThanhToanId, cancellationToken);
        if (dot == null)
            return DotThanhToanErrors.NotFound;

        var ky = dot.KyThanhToan;

        // 2. Load dữ liệu nguồn (Batch loading)
        var canHos = await _canHoRepository.GetAllActiveAsync(cancellationToken);
        var activeCanHoIds = canHos.Select(c => c.Id).ToList();

        var billingData = await LoadBillingDataAsync(activeCanHoIds, ky, cancellationToken);

        // 3. Khởi tạo danh sách hóa đơn mới
        var newInvoices = new List<HoaDon>();
        var ngayHan = _dateTimeProvider.Now.AddDays(15);

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

            if (hasDetails)
            {
                newInvoices.Add(hoaDon);
            }
        }

        // 4. Lưu dữ liệu
        if (newInvoices.Count != 0)
        {
            await _hoaDonRepository.AddRangeAsync(newInvoices, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new LapHoaDonDuThaoResponse
        {
            SoLuongHoaDonTaoMoi = newInvoices.Count,
            DotThanhToanId = dot.Id,
            TenDotThanhToan = dot.TenDot
        });
    }

    private async Task<BillingDataBundle> LoadBillingDataAsync(List<int> activeCanHoIds, KyThanhToan ky, CancellationToken cancellationToken)
    {
        var periodicServices = await _dichVuRepository.GetActivePeriodicServicesWithPriceListsAsync(cancellationToken);

        return new BillingDataBundle(
            PeriodicServiceDict: periodicServices.ToDictionary(s => s.Id),
            MandatoryServices: periodicServices.Where(s => s.IsBatBuoc).ToList(),
            ResidencyRelations: (await _cuTruRepository.GetByCanHoIdsAsync(activeCanHoIds, cancellationToken)).ToLookup(x => x.CanHoId),
            ConsumptionRecords: (await _chiSoRepository.GetLockedUnbilledByPeriodAsync(ky, cancellationToken)).ToLookup(x => x.CanHoId),
            Subscriptions: (await _dangKyRepository.GetActiveByCanHoIdsAsync(activeCanHoIds, cancellationToken)).ToLookup(x => x.CanHoId),
            ExistingInvoiceCanHoIds: await _hoaDonRepository.GetExistingCanHoIdsByKyAsync(ky, cancellationToken)
        );
    }
}
