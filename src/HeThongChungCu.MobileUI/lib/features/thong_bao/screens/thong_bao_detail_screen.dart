import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/core/navigation/app_navigation.dart';
import 'package:klks_app/design/design.dart';

import '../models/thong_bao_model.dart';
import '../services/thong_bao_service.dart';

enum _LoaiThongBao {
  cuTru(1, 'Yêu cầu cư trú'),
  phuongTien(2, 'Yêu cầu phương tiện'),
  thanhToan(3, 'Thanh toán'),
  thiCong(4, 'Yêu cầu thi công'),
  heTang(5, 'Hệ thống'),
  khac(6, 'Khác'),
  suaChua(7, 'Yêu cầu sửa chữa'),
  phanAnh(8, 'Yêu cầu phản ánh');

  const _LoaiThongBao(this.id, this.label);
  final int id;
  final String label;

  static _LoaiThongBao? fromId(int id) {
    for (final v in values) {
      if (v.id == id) return v;
    }
    return null;
  }

  bool get hasNavigation => switch (this) {
    cuTru || phuongTien || thanhToan || thiCong || suaChua || phanAnh => true,
    heTang || khac => false,
  };

  IconData get icon => switch (this) {
    cuTru => Icons.apartment_outlined,
    phuongTien => Icons.two_wheeler_outlined,
    thanhToan => Icons.receipt_long_outlined,
    thiCong => Icons.construction_outlined,
    suaChua => Icons.build_outlined,
    phanAnh => Icons.campaign_outlined,
    heTang => Icons.settings_outlined,
    khac => Icons.notifications_outlined,
  };

  AppBadgeVariant get badgeVariant => switch (this) {
    thanhToan => AppBadgeVariant.success,
    heTang || khac => AppBadgeVariant.info,
    _ => AppBadgeVariant.warning,
  };
}

class ThongBaoDetailScreen extends StatefulWidget {
  final ThongBaoItem item;

  const ThongBaoDetailScreen({super.key, required this.item});

  static Widget fromRoute(BuildContext context, GoRouterState state) =>
      ThongBaoDetailScreen(item: state.extra! as ThongBaoItem);

  @override
  State<ThongBaoDetailScreen> createState() => _ThongBaoDetailScreenState();
}

class _ThongBaoDetailScreenState extends State<ThongBaoDetailScreen> {
  final _service = ThongBaoService.instance;
  late ThongBaoItem _item;
  bool _isMarkingRead = false;

  @override
  void initState() {
    super.initState();
    _item = widget.item;
    if (!_item.isRead) _markAsRead();
  }

  Future<void> _markAsRead() async {
    setState(() => _isMarkingRead = true);

    final result = await _service.daDoc(phanBoThongBaoId: _item.id);

    if (!mounted) return;
    setState(() => _isMarkingRead = false);

    if (result.isOk) {
      setState(() {
        _item = _item.copyWith(isRead: true, readAt: DateTime.now());
      });
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(result.errorMessage!)));
    }
  }

  void _navigateToSource() {
    final loai = _LoaiThongBao.fromId(_item.loaiThongBaoId);
    if (loai == null || !loai.hasNavigation) return;

    switch (loai) {
      case _LoaiThongBao.cuTru:
      case _LoaiThongBao.phuongTien:
        AppNavigation.goResidence();

      case _LoaiThongBao.thanhToan:
        AppNavigation.goTabThenPush(2, '/dich-vu/hoa-don');

      case _LoaiThongBao.thiCong:
        AppNavigation.goTabThenPush(2, '/dich-vu/thi-cong');

      case _LoaiThongBao.suaChua:
        AppNavigation.goTabThenPush(2, '/dich-vu/sua-chua');

      case _LoaiThongBao.phanAnh:
        AppNavigation.goTabThenPush(2, '/dich-vu/phan-anh');

      case _LoaiThongBao.heTang:
      case _LoaiThongBao.khac:
        break;
    }
  }

  @override
  Widget build(BuildContext context) {
    final loai = _LoaiThongBao.fromId(_item.loaiThongBaoId);

    return AppScaffold(
      appBar: AppTopBar(
        title: 'Chi tiết thông báo',
        actions: [
          if (_isMarkingRead)
            const Padding(
              padding: EdgeInsets.all(14),
              child: SizedBox(
                width: AppConstants.spinnerSize,
                height: AppConstants.spinnerSize,
                child: CircularProgressIndicator(
                  strokeWidth: AppConstants.spinnerStrokeWidth,
                  color: AppColors.primary,
                ),
              ),
            )
          else
            Padding(
              padding: const EdgeInsets.only(right: 12),
              child: Icon(
                _item.isRead ? Icons.done_all : Icons.circle_outlined,
                color: _item.isRead
                    ? AppColors.success
                    : AppColors.textDisabled,
              ),
            ),
        ],
      ),
      body: SingleChildScrollView(
        padding: AppSpacing.insetAll16,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (loai != null)
              Row(
                children: [
                  Icon(loai.icon, size: 16, color: AppColors.textSecondary),
                  const SizedBox(width: 6),
                  AppStatusBadge(label: loai.label, variant: loai.badgeVariant),
                ],
              )
            else if (_item.tenLoaiThongBao.isNotEmpty)
              AppStatusBadge(
                label: _item.tenLoaiThongBao,
                variant: AppBadgeVariant.info,
              ),

            const SizedBox(height: AppSpacing.md),

            Text(_item.tieuDe, style: AppTypography.headline),
            const SizedBox(height: AppSpacing.sm),

            Row(
              children: [
                const Icon(
                  Icons.access_time_outlined,
                  size: 14,
                  color: AppColors.textSecondary,
                ),
                const SizedBox(width: 4),
                Text(
                  _item.thoiGianHienThi,
                  style: AppTypography.captionSmall.secondary,
                ),
                if (_item.isRead) ...[
                  const SizedBox(width: AppSpacing.md),
                  const Icon(
                    Icons.done_all,
                    size: 14,
                    color: AppColors.success,
                  ),
                  const SizedBox(width: 4),
                  Text(
                    'Đã đọc',
                    style: AppTypography.captionSmall.copyWith(
                      color: AppColors.success,
                    ),
                  ),
                ],
              ],
            ),

            const Divider(height: 32),

            Text(
              _item.noiDung,
              style: AppTypography.body.copyWith(height: 1.7),
            ),

            if (loai != null && loai.hasNavigation) ...[
              const SizedBox(height: AppSpacing.xl),
              AppButton(
                label: 'Xem ${loai.label}',
                variant: AppButtonVariant.outline,
                leadingIcon: loai.icon,
                onPressed: _navigateToSource,
              ),
            ],

            const SizedBox(height: AppSpacing.md),
          ],
        ),
      ),
    );
  }
}
