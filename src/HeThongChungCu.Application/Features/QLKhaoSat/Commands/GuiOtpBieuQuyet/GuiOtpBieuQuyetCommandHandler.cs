using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.GuiOtpBieuQuyet;

public class GuiOtpBieuQuyetCommandHandler : ICommandHandler<GuiOtpBieuQuyetCommand, string>
{
    private readonly IKhaoSatCommandRepository _khaoSatRepository;
    private readonly IBieuQuyetCuDanCommandRepository _bieuQuyetRepository;
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _memoryCache;

    public GuiOtpBieuQuyetCommandHandler(
        IKhaoSatCommandRepository khaoSatRepository,
        IBieuQuyetCuDanCommandRepository bieuQuyetRepository,
        ITaiKhoanCommandRepository taiKhoanRepository,
        IEmailService emailService,
        IMemoryCache memoryCache)
    {
        _khaoSatRepository = khaoSatRepository;
        _bieuQuyetRepository = bieuQuyetRepository;
        _taiKhoanRepository = taiKhoanRepository;
        _emailService = emailService;
        _memoryCache = memoryCache;
    }

    public async Task<Result<string>> Handle(GuiOtpBieuQuyetCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch Campaign
        var khaoSat = await _khaoSatRepository.GetByIdAsync(command.KhaoSatId, cancellationToken);
        if (khaoSat == null)
            return KhaoSatErrors.NotFound;

        // 1.1. Validate Campaign status
        if (khaoSat.TrangThaiId != TrangThaiKhaoSat.DangDienRa)
            return Result.Failure<string>(KhaoSatErrors.InvalidStatus);

        // 2. Check if Resident has already voted
        var hasVoted = await _bieuQuyetRepository.HasResidentVotedAsync(command.KhaoSatId, command.CanHoId, cancellationToken);
        if (hasVoted)
            return Result.Failure<string>(KhaoSatErrors.AlreadyVoted);

        // 3. Fetch User Account Email
        var taiKhoan = await _taiKhoanRepository.GetByNguoiDungIdAsync(command.NguoiDungId, cancellationToken);
        if (taiKhoan == null || string.IsNullOrWhiteSpace(taiKhoan.Email?.Value))
            return Result.Failure<string>(new Error("TaiKhoan.NotFound", "Không tìm thấy tài khoản liên kết hoặc địa chỉ email hợp lệ."));

        // 4. Generate 6-digit OTP Code
        var otpCode = new Random().Next(100000, 999999).ToString();

        // 5. Save OTP in memory cache expiring in 5 minutes
        var cacheKey = $"OTP_KhaoSat_{command.KhaoSatId}_{command.CanHoId}";
        _memoryCache.Set(cacheKey, otpCode, TimeSpan.FromMinutes(5));

        // 6. Send OTP via Email Service
        var emailAddress = taiKhoan.Email.Value;
        var subject = $"Mã xác thực OTP biểu quyết trực tuyến - {khaoSat.TieuDe}";
        var body = $"""
            <h3>HỆ THỐNG QUẢN LÝ CHUNG CƯ TRỰC TUYẾN</h3>
            <p>Xin chào cư dân,</p>
            <p>Hệ thống nhận được yêu cầu biểu quyết/bỏ phiếu cho chiến dịch: <strong>{khaoSat.TieuDe}</strong>.</p>
            <p>Mã xác thực OTP của bạn là: <strong style='font-size: 18px; color: #ff0000;'>{otpCode}</strong></p>
            <p>Mã OTP này có thời hạn sử dụng là <strong>5 phút</strong>. Vui lòng tuyệt đối không chia sẻ mã này cho bất kỳ ai.</p>
            <p>Trân trọng,</p>
            <p>Ban Quản Trị Chung Cư.</p>
            """;

        await _emailService.SendAsync(emailAddress, subject, body, cancellationToken);

        // 7. Return anonymous/masked email for confirmation UI
        var maskedEmail = _emailService.MaskEmail(emailAddress);
        return Result.Success(maskedEmail);
    }
}
