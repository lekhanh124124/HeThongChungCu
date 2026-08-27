import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/hoa_don_model.dart';
import '../services/hoa_don_service.dart';

class ThanhToanScreen extends StatefulWidget {
  final int hoaDonId;
  final String maHoaDon;
  final double tongTien;
  final List<int> chiTietHoaDonIds;

  const ThanhToanScreen({
    super.key,
    required this.hoaDonId,
    required this.maHoaDon,
    required this.tongTien,
    required this.chiTietHoaDonIds,
  });

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;
    return ThanhToanScreen(
      hoaDonId: e['hoaDonId'] as int,
      maHoaDon: e['maHoaDon'] as String,
      tongTien: e['tongTien'] as double,
      chiTietHoaDonIds: e['chiTietHoaDonIds'] as List<int>,
    );
  }

  @override
  State<ThanhToanScreen> createState() => _ThanhToanScreenState();
}

class _ThanhToanScreenState extends State<ThanhToanScreen> {
  bool _creatingSession = true;
  Object? _sessionError;
  PhienThanhToan? _phien;

  Timer? _pollingTimer;
  bool _donePolling = false;
  int _pollCount = 0;
  static const _maxPollCount = 100;

  @override
  void initState() {
    super.initState();
    _createSession();
  }

  @override
  void dispose() {
    _pollingTimer?.cancel();
    super.dispose();
  }

  Future<void> _createSession() async {
    setState(() {
      _creatingSession = true;
      _sessionError = null;
    });
    try {
      final phien = await HoaDonService.instance.taoPhienThanhToan(
        hoaDonId: widget.hoaDonId,
        chiTietHoaDonIds: widget.chiTietHoaDonIds,
      );
      if (!mounted) return;
      setState(() {
        _phien = phien;
        _creatingSession = false;
      });
      _startPolling();
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _sessionError = e;
        _creatingSession = false;
      });
    }
  }

  void _startPolling() {
    _pollingTimer?.cancel();
    _pollingTimer = Timer.periodic(const Duration(seconds: 3), (_) {
      _checkStatus();
    });
  }

  Future<void> _checkStatus() async {
    if (_donePolling) return;
    _pollCount++;
    if (_pollCount >= _maxPollCount) {
      _pollingTimer?.cancel();
      return;
    }
    try {
      final detail = await HoaDonService.instance.getById(widget.hoaDonId);
      if (!mounted) return;
      if (detail.laDaThanhToan) {
        _pollingTimer?.cancel();
        setState(() => _donePolling = true);
        _showSuccess();
      }
    } catch (_) {}
  }

  void _showSuccess() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => _SuccessDialog(
        tongTien: widget.tongTien,
        onClose: () {
          Navigator.of(context).pop();
          context.pop();
        },
      ),
    );
  }

  void _copyMa() {
    final ma = _phien?.maThanhToan;
    if (ma == null) return;
    Clipboard.setData(ClipboardData(text: ma));
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Đã sao chép mã thanh toán'),
        duration: Duration(seconds: 2),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(title: 'Thanh toán', body: _buildBody());
  }

  Widget _buildBody() {
    if (_creatingSession) {
      return const Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            CircularProgressIndicator(),
            SizedBox(height: AppSpacing.md),
            Text('Đang tạo mã thanh toán...'),
          ],
        ),
      );
    }

    if (_sessionError != null) {
      return ErrorDisplay.fullScreen(
        error: _sessionError,
        onRetry: _createSession,
      );
    }

    final phien = _phien!;

    return SingleChildScrollView(
      padding: AppSpacing.insetAll16,
      child: Column(
        children: [
          Container(
            width: double.infinity,
            padding: AppSpacing.insetAll16,
            decoration: BoxDecoration(
              gradient: const LinearGradient(
                colors: [AppColors.primary, AppColors.primaryDark],
              ),
              borderRadius: AppRadius.card,
            ),
            child: Column(
              children: [
                Text(
                  'Số tiền cần thanh toán',
                  style: AppTypography.caption.copyWith(color: Colors.white70),
                ),
                const SizedBox(height: 6),
                Text(
                  formatTien(phien.soTien),
                  style: AppTypography.display.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w900,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  widget.maHoaDon,
                  style: AppTypography.captionSmall.copyWith(
                    color: Colors.white60,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: AppSpacing.md),

          AppCard(
            child: Column(
              children: [
                ClipRRect(
                  borderRadius: AppRadius.buttonSmall,
                  child: Image.network(
                    phien.vietQrUrl,
                    width: 240,
                    height: 240,
                    fit: BoxFit.contain,
                    loadingBuilder: (_, child, progress) {
                      if (progress == null) return child;
                      return const SizedBox(
                        width: 240,
                        height: 240,
                        child: Center(child: CircularProgressIndicator()),
                      );
                    },
                    errorBuilder: (_, _, _) => Container(
                      width: 240,
                      height: 240,
                      color: AppColors.primaryLight,
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Icon(
                            Icons.qr_code_rounded,
                            size: 64,
                            color: AppColors.textDisabled,
                          ),
                          const SizedBox(height: AppSpacing.sm),
                          Text(
                            'Không tải được QR',
                            style: AppTypography.captionSmall.secondary,
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: AppSpacing.sm2),
                Text(
                  'Quét mã QR bằng ứng dụng ngân hàng',
                  style: AppTypography.body.secondary,
                  textAlign: TextAlign.center,
                ),
              ],
            ),
          ),
          const SizedBox(height: AppSpacing.md),

          Container(
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: AppRadius.card,
              border: Border.all(color: AppColors.border),
            ),
            padding: const EdgeInsets.symmetric(
              horizontal: AppSpacing.md,
              vertical: AppSpacing.sm2,
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Nội dung chuyển khoản',
                  style: AppTypography.captionSmall.secondary,
                ),
                const SizedBox(height: 6),
                Row(
                  children: [
                    Expanded(
                      child: Text(
                        phien.maThanhToan,
                        style: AppTypography.headline.copyWith(
                          letterSpacing: 0.5,
                        ),
                      ),
                    ),
                    IconButton(
                      onPressed: _copyMa,
                      icon: const Icon(
                        Icons.copy_rounded,
                        color: AppColors.primary,
                        size: 20,
                      ),
                      tooltip: 'Sao chép',
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  '⚠️ Nhập đúng nội dung để hệ thống tự xác nhận',
                  style: AppTypography.captionSmall.copyWith(
                    color: AppColors.warning,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: AppSpacing.md),

          if (!_donePolling)
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.md,
                vertical: AppSpacing.sm2,
              ),
              decoration: BoxDecoration(
                color: AppColors.successLight,
                borderRadius: AppRadius.card,
                border: Border.all(color: AppColors.success.withAlpha(80)),
              ),
              child: Row(
                children: [
                  const SizedBox(
                    width: 16,
                    height: 16,
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      valueColor: AlwaysStoppedAnimation(AppColors.success),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Text(
                    'Đang chờ xác nhận thanh toán...',
                    style: AppTypography.body.copyWith(
                      color: AppColors.success,
                    ),
                  ),
                ],
              ),
            ),

          const SizedBox(height: AppSpacing.xl),
          const _HuongDanWidget(),
          const SizedBox(height: AppSpacing.xl),
        ],
      ),
    );
  }
}

class _HuongDanWidget extends StatelessWidget {
  const _HuongDanWidget();

  static const _steps = [
    (
      icon: Icons.phone_android_rounded,
      text: 'Mở ứng dụng ngân hàng trên điện thoại',
    ),
    (
      icon: Icons.qr_code_scanner_rounded,
      text: 'Chọn chức năng quét mã QR / chuyển tiền',
    ),
    (
      icon: Icons.edit_note_rounded,
      text: 'Kiểm tra nội dung chuyển khoản khớp với mã trên',
    ),
    (
      icon: Icons.check_circle_outline_rounded,
      text: 'Xác nhận & hệ thống tự động ghi nhận',
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: AppSpacing.insetAll16,
      decoration: BoxDecoration(
        color: AppColors.background,
        borderRadius: AppRadius.card,
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Hướng dẫn thanh toán', style: AppTypography.subhead),
          const SizedBox(height: AppSpacing.sm2),
          ..._steps.asMap().entries.map(
            (e) => Padding(
              padding: const EdgeInsets.only(bottom: 10),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Container(
                    width: 24,
                    height: 24,
                    decoration: const BoxDecoration(
                      color: AppColors.primary,
                      shape: BoxShape.circle,
                    ),
                    child: Center(
                      child: Text(
                        '${e.key + 1}',
                        style: AppTypography.captionSmall.copyWith(
                          color: AppColors.textOnPrimary,
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Text(
                      e.value.text,
                      style: AppTypography.body.secondary,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _SuccessDialog extends StatelessWidget {
  final double tongTien;
  final VoidCallback onClose;

  const _SuccessDialog({required this.tongTien, required this.onClose});

  @override
  Widget build(BuildContext context) {
    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: AppRadius.card),
      child: Padding(
        padding: const EdgeInsets.all(28),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              padding: AppSpacing.insetAll16,
              decoration: const BoxDecoration(
                color: AppColors.successLight,
                shape: BoxShape.circle,
              ),
              child: const Icon(
                Icons.check_circle_rounded,
                color: AppColors.success,
                size: 56,
              ),
            ),
            const SizedBox(height: AppSpacing.md),
            Text('Thanh toán thành công!', style: AppTypography.headline),
            const SizedBox(height: AppSpacing.sm),
            Text(
              formatTien(tongTien),
              style: AppTypography.display.copyWith(color: AppColors.success),
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              'Hóa đơn đã được ghi nhận.\nCảm ơn bạn đã thanh toán!',
              textAlign: TextAlign.center,
              style: AppTypography.body.secondary,
            ),
            const SizedBox(height: AppSpacing.xl),
            AppButton(
              label: 'Hoàn tất',
              onPressed: onClose,
              backgroundColor: AppColors.success,
            ),
          ],
        ),
      ),
    );
  }
}
