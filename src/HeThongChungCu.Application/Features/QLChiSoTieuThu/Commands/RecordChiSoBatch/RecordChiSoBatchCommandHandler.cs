using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using System.Linq;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.RecordChiSoBatch;

public class RecordChiSoBatchCommandHandler : ICommandHandler<RecordChiSoBatchCommand, ChiSoBatchResultResponse>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordChiSoBatchCommandHandler(
        IChiSoTieuThuCommandRepository chiSoRepository,
        ICanHoCommandRepository canHoRepository,
        IDichVuCommandRepository dichVuRepository,
        IUnitOfWork unitOfWork)
    {
        _chiSoRepository = chiSoRepository;
        _canHoRepository = canHoRepository;
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChiSoBatchResultResponse>> Handle(RecordChiSoBatchCommand request, CancellationToken cancellationToken)
    {
        var response = new ChiSoBatchResultResponse
        {
            TotalItems = request.Items.Count
        };

        // 1. Lấy danh sách các chỉ số đã tồn tại trong DB cho kỳ này
        var existingChiSos = await _chiSoRepository.GetByPeriodAsync(request.Thang, request.Nam, cancellationToken);
        var existingLookup = existingChiSos.ToLookup(x => (x.CanHoId, x.DichVuId));

        // 2. Tải thông tin Căn Hộ và Dịch Vụ để kiểm tra tồn tại & lấy MaCanHo sinh MaTraCuu
        var canHoIds = request.Items.Select(x => x.CanHoId).Distinct().ToList();
        var dichVuIds = request.Items.Select(x => x.DichVuId).Distinct().ToList();

        var canHos = await _canHoRepository.GetByIdsAsync(canHoIds, cancellationToken);
        var dichVus = await _dichVuRepository.GetByIdsAsync(dichVuIds, cancellationToken);

        var canHoMap = canHos.ToDictionary(x => x.Id, x => x.MaCanHo);
        var dichVuMap = dichVus.ToDictionary(x => x.Id, x => x.TenDichVu);

        var processedInRequest = new HashSet<(int CanHoId, int DichVuId)>();
        var newChiSos = new List<ChiSoTieuThu>();

        for (int i = 0; i < request.Items.Count; i++)
        {
            var item = request.Items[i];
            var key = (item.CanHoId, item.DichVuId);

            // Kiểm tra căn hộ tồn tại
            if (!canHoMap.TryGetValue(item.CanHoId, out var maCanHo))
            {
                response.Errors.Add(new ChiSoBatchErrorDetail
                {
                    CanHoId = item.CanHoId,
                    Identifier = $"Căn hộ ID: {item.CanHoId}",
                    Reason = "Căn hộ không tồn tại trong hệ thống."
                });
                continue;
            }

            // Kiểm tra dịch vụ tồn tại
            if (!dichVuMap.TryGetValue(item.DichVuId, out var tenDichVu))
            {
                response.Errors.Add(new ChiSoBatchErrorDetail
                {
                    CanHoId = item.CanHoId,
                    Identifier = $"Dịch vụ ID: {item.DichVuId}",
                    Reason = "Dịch vụ không tồn tại trong hệ thống."
                });
                continue;
            }

            // Bỏ qua nếu đã tồn tại trong DB
            if (existingLookup.Contains(key))
            {
                response.Errors.Add(new ChiSoBatchErrorDetail
                {
                    CanHoId = item.CanHoId,
                    Identifier = $"Căn hộ: {maCanHo} - Dịch vụ: {tenDichVu}",
                    Reason = "Đã tồn tại chỉ số cho kỳ này trong hệ thống."
                });
                continue;
            }

            // Bỏ qua nếu đã có trong chính request này rồi
            if (!processedInRequest.Add(key))
            {
                response.Errors.Add(new ChiSoBatchErrorDetail
                {
                    CanHoId = item.CanHoId,
                    Identifier = $"Căn hộ: {maCanHo} - Dịch vụ: {tenDichVu}",
                    Reason = "Dữ liệu bị trùng lặp trong danh sách gửi lên."
                });
                continue;
            }

            // Kiểm tra chỉ số mới không được nhỏ hơn chỉ số cũ
            if (item.ChiSoMoi < item.ChiSoCu)
            {
                response.Errors.Add(new ChiSoBatchErrorDetail
                {
                    CanHoId = item.CanHoId,
                    Identifier = $"Căn hộ: {maCanHo} - Dịch vụ: {tenDichVu}",
                    Reason = $"Chỉ số mới ({item.ChiSoMoi}) không được nhỏ hơn chỉ số cũ ({item.ChiSoCu})."
                });
                continue;
            }

            // Tạo mã tra cứu đúng chuẩn: {MaCanHo}_{DichVuId}_{Thang}_{Nam}
            var maTraCuu = $"{maCanHo}_{item.DichVuId}_{request.Thang}_{request.Nam}";

            var chiSo = ChiSoTieuThu.Create(
                item.CanHoId,
                item.DichVuId,
                item.ChiSoCu,
                item.ChiSoMoi,
                request.Thang,
                request.Nam,
                request.NgayGhiNhan,
                item.AnhDongHoId,
                item.GhiChu,
                maTraCuu
            );

            newChiSos.Add(chiSo);
        }

        if (newChiSos.Count > 0)
        {
            await _chiSoRepository.AddRangeAsync(newChiSos, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        response.SuccessCount = newChiSos.Count;
        response.FailedCount = response.TotalItems - response.SuccessCount;

        return Result.Success(response);
    }
}
