import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import 'package:klks_app/features/cu_tru/quan_he/widgets/shared_widget.dart';

import '../models/thanh_vien_model.dart';
import '../services/thanh_vien_service.dart';
import '../widgets/tv_shared_widgets.dart';

class XoaYeuCauThanhVienScreen extends StatefulWidget {
  final ThanhVienCuTruModel thanhVien;
  final QuanHeCuTruModel canHoInfo;

  const XoaYeuCauThanhVienScreen({
    super.key,
    required this.thanhVien,
    required this.canHoInfo,
  });

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;
    return XoaYeuCauThanhVienScreen(
      thanhVien: e['thanhVien'],
      canHoInfo: e['canHoInfo'],
    );
  }

  @override
  State<XoaYeuCauThanhVienScreen> createState() =>
      _XoaYeuCauThanhVienScreenState();
}

class _XoaYeuCauThanhVienScreenState extends State<XoaYeuCauThanhVienScreen> {
  final _service = ThanhVienService.instance;
  final _noiDungCtrl = TextEditingController();

  bool _isSubmitting = false;

  @override
  void dispose() {
    _noiDungCtrl.dispose();
    super.dispose();
  }

  Future<void> _submit(bool isSubmit) async {
    if (_isSubmitting) return;

    if (isSubmit) {
      final confirmed = await AppConfirmDialog.show(
        context,
        title: 'Xác nhận yêu cầu xóa thành viên',
        message:
            'Bạn đang tạo yêu cầu XÓA thành viên '
            '"${widget.thanhVien.fullName}" khỏi căn hộ. '
            'Sau khi nộp, yêu cầu sẽ chờ BQL phê duyệt.',
        confirmLabel: 'Xác nhận xóa',
        isDangerous: true,
      );
      if (confirmed != true || !mounted) return;
    }

    setState(() => _isSubmitting = true);

    try {
      await _service.createYeuCau(
        TaoYeuCauCuTruRequest(
          canHoId: widget.canHoInfo.canHoId,
          loaiYeuCauId: 3,
          isSubmit: isSubmit,
          targetQuanHeCuTruId: widget.thanhVien.quanHeCuTruId,
          noiDung: _noiDungCtrl.text.trim().isEmpty
              ? null
              : _noiDungCtrl.text.trim(),
        ),
      );

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            isSubmit
                ? 'Đã nộp yêu cầu xóa thành viên'
                : 'Đã lưu nháp yêu cầu xóa',
          ),
        ),
      );
      Navigator.pop(context, true);
    } on Exception catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Yêu cầu xóa thành viên',
      body: _isSubmitting
          ? const Center(
              child: CircularProgressIndicator(color: AppColors.primary),
            )
          : SingleChildScrollView(
              padding: AppSpacing.insetAll16,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _WarningBanner(thanhVien: widget.thanhVien),
                  const SizedBox(height: AppSpacing.lg),

                  TvMemberReadonlyCard(
                    thanhVien: widget.thanhVien,
                    diaChiCanHo: widget.canHoInfo.diaChiDayDu,
                    badgeLabel: 'Xóa',
                    badgeVariant: AppBadgeVariant.error,
                  ),
                  const SizedBox(height: AppSpacing.lg),

                  Text(
                    'Lý do yêu cầu xóa (tùy chọn)',
                    style: AppTypography.subhead,
                  ),
                  const SizedBox(height: AppSpacing.sm),
                  Field(
                    controller: _noiDungCtrl,
                    label: 'Ghi chú / lý do',
                    maxLines: 4,
                    hint: 'Ví dụ: Thành viên đã chuyển đi nơi khác...',
                  ),
                  const SizedBox(height: AppSpacing.xl),

                  Row(
                    children: [
                      Expanded(
                        child: AppButton(
                          label: 'Lưu nháp',
                          variant: AppButtonVariant.outline,
                          onPressed: () => _submit(false),
                        ),
                      ),
                      const SizedBox(width: AppSpacing.sm2),
                      Expanded(
                        child: AppButton(
                          label: 'Nộp yêu cầu xóa',
                          variant: AppButtonVariant.danger,
                          onPressed: () => _submit(true),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: AppSpacing.md),
                ],
              ),
            ),
    );
  }
}

class _WarningBanner extends StatelessWidget {
  final ThanhVienCuTruModel thanhVien;
  const _WarningBanner({required this.thanhVien});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: AppSpacing.insetAll16,
      decoration: BoxDecoration(
        color: AppColors.errorLight,
        borderRadius: AppRadius.card,
        border: Border.all(color: AppColors.error.withAlpha(80)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Icon(
            Icons.warning_amber_rounded,
            color: AppColors.error,
            size: 24,
          ),
          const SizedBox(width: AppSpacing.sm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Yêu cầu xóa thành viên',
                  style: AppTypography.subhead.error,
                ),
                const SizedBox(height: 4),
                Text(
                  'Thao tác này sẽ tạo yêu cầu xóa '
                  '"${thanhVien.fullName}" khỏi danh sách cư trú. '
                  'Yêu cầu cần được BQL phê duyệt trước khi có hiệu lực.',
                  style: AppTypography.caption.copyWith(color: AppColors.error),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
