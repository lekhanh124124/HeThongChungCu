using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.QuetLichBaoTriVaSinhPhieu;

public class QuetLichBaoTriVaSinhPhieuCommandHandler : ICommandHandler<QuetLichBaoTriVaSinhPhieuCommand, int>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IPhieuBaoTriCommandRepository _phieuBaoTriRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuetLichBaoTriVaSinhPhieuCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IPhieuBaoTriCommandRepository phieuBaoTriRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _phieuBaoTriRepository = phieuBaoTriRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(QuetLichBaoTriVaSinhPhieuCommand request, CancellationToken cancellationToken)
    {
        var today = DateTimeOffset.UtcNow;
        var dueSchedules = await _thietBiRepository.GetActiveLichBaoTrisAsync(today, cancellationToken);

        int count = 0;
        foreach (var schedule in dueSchedules)
        {
            var dueDate = schedule.NgayBaoTriTiepTheo;
            
            // Tránh tạo trùng phiếu cho cùng một ngày bảo trì dự kiến
            var exists = await _phieuBaoTriRepository.ExistsForScheduleOnDateAsync(schedule.Id, dueDate, cancellationToken);
            if (exists) continue;

            var thietBi = await _thietBiRepository.GetThietBiByIdAsync(schedule.ThietBiId, cancellationToken);
            var deviceCode = thietBi?.MaThietBi ?? schedule.ThietBiId.ToString();
            var maPhieu = $"BTDK-{deviceCode}-{dueDate:yyyyMMdd}";

            // Check if the auto generated code already exists by some reason
            var codeExists = await _phieuBaoTriRepository.MaPhieuExistsAsync(maPhieu, cancellationToken);
            if (codeExists)
            {
                maPhieu += $"-{Guid.NewGuid().ToString()[..4]}";
            }

            var standardChecklists = new List<string>
            {
                "Kiểm tra hiện trạng thiết bị ngoại quan (rỉ sét, biến dạng, lỏng ốc...)",
                "Kiểm tra thông số kỹ thuật vận hành cơ bản",
                "Vệ sinh thiết bị, bôi trơn linh kiện (nếu cần)",
                "Chạy thử nghiệm thiết bị và ghi chú hiệu năng"
            };

            var phieu = PhieuBaoTri.Create(
                maPhieu,
                schedule.ThietBiId,
                schedule.HangMucBaoTriId,
                schedule.Id,
                today, // NgayLapPhieu
                dueDate, // NgayDuKien
                standardChecklists);

            await _phieuBaoTriRepository.AddPhieuBaoTriAsync(phieu, cancellationToken);

            // Ghi nhận thực thi trên lịch trình (Cập nhật ngày gần nhất và tính ngày tiếp theo)
            schedule.RecordExecution(dueDate);
            _thietBiRepository.UpdateLichBaoTri(schedule);

            count++;
        }

        if (count > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(count);
    }
}
