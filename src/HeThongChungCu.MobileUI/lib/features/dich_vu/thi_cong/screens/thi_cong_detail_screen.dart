import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/services/cu_tru_service.dart';
import 'package:klks_app/features/shared/widgets/full_screen_image_viewer.dart';

import '../models/thi_cong_model.dart';
import '../services/thi_cong_service.dart';

class ThiCongDetailScreen extends StatefulWidget {
  final int id;
  const ThiCongDetailScreen({super.key, required this.id});

  @override
  State<ThiCongDetailScreen> createState() => _ThiCongDetailScreenState();
}

class _ThiCongDetailScreenState extends State<ThiCongDetailScreen> {
  final _service = YeuCauThiCongService.instance;
  final _cuTruService = CuTruService.instance;

  YeuCauThiCongDetail? _detail;
  bool _isLoading = false;
  bool _isActioning = false;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });
    try {
      final detail = await _service.getById(widget.id);
      setState(() => _detail = detail);
    } on Exception catch (e) {
      setState(() => _errorMessage = e.toString());
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _navigateToEdit() async {
    if (_detail == null) return;
    final dsCanHo = await _cuTruService.getQuanHeCuTruList();
    if (!mounted) return;
    final changed = await context.push<bool>(
      '/dich-vu/thi-cong/form',
      extra: {'dsCanHo': dsCanHo, 'existingDetail': _detail},
    );
    if (changed == true) _load();
  }

  Future<void> _thuHoi() async {
    final d = _detail!;
    final confirm = await AppConfirmDialog.show(
      context,
      title: 'Thu hồi yêu cầu',
      message: 'Bạn chắc chắn muốn thu hồi?\nHành động này không thể hoàn tác.',
      confirmLabel: 'Thu hồi',
      cancelLabel: 'Hủy',
      isDangerous: true,
    );
    if (confirm != true || !mounted) return;

    setState(() => _isActioning = true);
    try {
      await _service.withdraw(d);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Thu hồi thành công',
            style: AppTypography.body.onPrimary,
          ),
          backgroundColor: AppColors.success,
        ),
      );
      Navigator.pop(context, true);
    } on Exception catch (e) {
      if (!mounted) return;
      ErrorDisplay.showSnackBar(context, error: e);
    } finally {
      if (mounted) setState(() => _isActioning = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Chi tiết #${widget.id}',
      actions: [
        if (_detail?.coTheChinhSua == true)
          IconButton(
            icon: const Icon(Icons.edit_outlined),
            tooltip: 'Chỉnh sửa',
            onPressed: _navigateToEdit,
          ),
        IconButton(icon: const Icon(Icons.refresh), onPressed: _load),
      ],
      body: _buildBody(),
      bottomNavigationBar: _buildBottom(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_errorMessage != null) {
      return ErrorDisplay.fullScreen(error: _errorMessage!, onRetry: _load);
    }

    if (_detail == null) return const SizedBox.shrink();

    final d = _detail!;
    final df = DateFormat('dd/MM/yyyy');
    final dtf = DateFormat('dd/MM/yyyy HH:mm');

    return RefreshIndicator(
      onRefresh: _load,
      color: AppColors.primary,
      child: ListView(
        padding: AppSpacing.insetAll16,
        children: [
          _StatusBanner(detail: d),
          AppSpacing.sm.verticalSpace,

          if (d.isReturned && d.lyDo.isNotEmpty) ...[
            _SectionCard(
              title: 'Phản hồi từ BQL',
              titleColor: AppColors.warning,
              children: [_InfoRow('Lý do', d.lyDo)],
            ),
            AppSpacing.sm.verticalSpace,
          ],

          _SectionCard(
            title: 'Thông tin yêu cầu',
            children: [
              _InfoRow('Căn hộ', d.tenCanHo),
              _InfoRow('Hạng mục', d.hangMucThiCong),
              _InfoRow(
                'Dự kiến bắt đầu',
                d.duKienBatDau != null ? df.format(d.duKienBatDau!) : '-',
              ),
              _InfoRow(
                'Dự kiến kết thúc',
                d.duKienKetThuc != null ? df.format(d.duKienKetThuc!) : '-',
              ),
              _InfoRow('Đơn vị thi công', d.tenDonViThiCong),
              _InfoRow('Người đại diện', d.nguoiDaiDien),
              _InfoRow('Điện thoại ĐD', d.soDienThoaiDaiDien),
              if (d.noiDung.isNotEmpty) _InfoRow('Nội dung', d.noiDung),
            ],
          ),
          AppSpacing.sm.verticalSpace,

          _SectionCard(
            title: 'Trạng thái',
            children: [
              _InfoRow('Hành chính', d.trangThaiYeuCauTen),
              _InfoRow('Thi công', d.trangThaiThiCongTen),
              _InfoRow(
                'Ngày tạo',
                d.createdAt != null ? dtf.format(d.createdAt!) : '-',
              ),
              _InfoRow('Người gửi', d.tenNguoiGui),
            ],
          ),
          AppSpacing.sm.verticalSpace,

          if (d.tienDatCoc > 0) ...[
            _SectionCard(
              title: 'Thông tin tiền cọc',
              children: [
                _InfoRow('Tiền đặt cọc', d.tienDatCocFormatted),
                _InfoRow('Đã thu cọc', d.isDaThuCoc ? 'Đã thu' : 'Chưa thu'),
                if (d.ghiChuThuCoc.isNotEmpty)
                  _InfoRow('Ghi chú', d.ghiChuThuCoc),
                if (d.tienKhauTru > 0) ...[
                  _InfoRow(
                    'Tiền khấu trừ',
                    '${d.tienKhauTru.toStringAsFixed(0)} đ',
                  ),
                  if (d.lyDoKhauTru.isNotEmpty)
                    _InfoRow('Lý do khấu trừ', d.lyDoKhauTru),
                ],
                _InfoRow(
                  'Đã hoàn cọc',
                  d.isDaHoanCoc ? 'Đã hoàn' : 'Chưa hoàn',
                ),
              ],
            ),
            AppSpacing.sm.verticalSpace,
          ],

          _SectionCard(
            title: 'Danh sách nhân sự (${d.nhanSuThiCongs.length})',
            children: d.nhanSuThiCongs.isEmpty
                ? [Text('Chưa có nhân sự', style: AppTypography.body.secondary)]
                : d.nhanSuThiCongs.map(_buildNhanSuRow).toList(),
          ),
          AppSpacing.sm.verticalSpace,

          _SectionCard(
            title: 'Hồ sơ đính kèm (${d.danhSachTep.length})',
            children: d.danhSachTep.isEmpty
                ? [Text('Chưa có tệp', style: AppTypography.body.secondary)]
                : d.danhSachTep
                      .asMap()
                      .entries
                      .map(
                        (entry) => _TepTile(
                          tep: entry.value,
                          onTap: entry.value.isImage
                              ? () => FullScreenImageViewer.show(
                                  context,
                                  files: d.danhSachTep
                                      .where((t) => t.isImage)
                                      .toList(),
                                  initialIndex: d.danhSachTep
                                      .where((t) => t.isImage)
                                      .toList()
                                      .indexWhere(
                                        (t) => t.id == entry.value.id,
                                      ),
                                )
                              : null,
                        ),
                      )
                      .toList(),
          ),

          80.0.verticalSpace,
        ],
      ),
    );
  }

  Widget? _buildBottom() {
    final d = _detail;
    if (d == null || _isLoading) return null;
    if (!d.coTheChinhSua && !d.coTheThuHoi) return null;

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.md,
          AppSpacing.sm,
          AppSpacing.md,
          AppSpacing.md,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            if (d.coTheChinhSua)
              AppButton(
                label: 'Chỉnh sửa & Gửi lại',
                leadingIcon: Icons.edit_outlined,
                isLoading: _isActioning,
                onPressed: _navigateToEdit,
              ),
            if (d.coTheChinhSua && d.coTheThuHoi) AppSpacing.sm.verticalSpace,
            if (d.coTheThuHoi)
              AppButton(
                label: 'Thu hồi yêu cầu',
                variant: AppButtonVariant.danger,
                leadingIcon: Icons.undo,
                isLoading: _isActioning,
                onPressed: _thuHoi,
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildNhanSuRow(NhanSuThiCong ns) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          CircleAvatar(
            radius: 18,
            backgroundColor: AppColors.primaryLight,
            child: const Icon(Icons.person, size: 18, color: AppColors.primary),
          ),
          AppSpacing.sm.horizontalSpace,
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(ns.hoTen, style: AppTypography.bodyMedium),
                if (ns.vaiTro.isNotEmpty)
                  Text(ns.vaiTro, style: AppTypography.captionSmall.secondary),
                if (ns.soCCCD.isNotEmpty)
                  Text(
                    'CCCD: ${ns.soCCCD}',
                    style: AppTypography.captionSmall.secondary,
                  ),
                if (ns.soDienThoai.isNotEmpty)
                  Text(
                    ns.soDienThoai,
                    style: AppTypography.captionSmall.primary,
                  ),
                if (ns.ghiChu.isNotEmpty)
                  Text(
                    'Ghi chú: ${ns.ghiChu}',
                    style: AppTypography.captionSmall.secondary,
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _StatusBanner extends StatelessWidget {
  final YeuCauThiCongDetail detail;
  const _StatusBanner({required this.detail});

  ({Color color, IconData icon, String? hint}) get _style {
    switch (detail.trangThaiYeuCauId) {
      case TrangThaiYeuCau.saved:
        return (
          color: AppColors.textSecondary,
          icon: Icons.drafts_outlined,
          hint: null,
        );
      case TrangThaiYeuCau.pending:
        return (
          color: AppColors.warning,
          icon: Icons.hourglass_top,
          hint: null,
        );
      case TrangThaiYeuCau.approved:
        return (
          color: AppColors.primary,
          icon: Icons.check_circle_outline,
          hint: null,
        );
      case TrangThaiYeuCau.returned:
        return (
          color: AppColors.warning,
          icon: Icons.assignment_return_outlined,
          hint: 'Vui lòng bổ sung thông tin và gửi lại',
        );
      case TrangThaiYeuCau.completed:
        return (color: AppColors.success, icon: Icons.task_alt, hint: null);
      case TrangThaiYeuCau.withdrawn:
      case TrangThaiYeuCau.expired:
        return (color: AppColors.secondary, icon: Icons.undo, hint: null);
      case TrangThaiYeuCau.rejected:
      case TrangThaiYeuCau.cancelled:
        return (
          color: AppColors.error,
          icon: Icons.cancel_outlined,
          hint: null,
        );
      default:
        return (
          color: AppColors.secondary,
          icon: Icons.help_outline,
          hint: null,
        );
    }
  }

  @override
  Widget build(BuildContext context) {
    final s = _style;
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: 14,
      ),
      decoration: BoxDecoration(
        color: s.color.withValues(alpha: 0.1),
        borderRadius: AppRadius.card,
        border: Border.all(color: s.color.withValues(alpha: 0.4)),
      ),
      child: Row(
        children: [
          Icon(s.icon, color: s.color, size: 30),
          AppSpacing.sm.horizontalSpace,
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  detail.trangThaiYeuCauTen,
                  style: AppTypography.subhead.withColor(s.color),
                ),
                if (s.hint != null)
                  Text(s.hint!, style: AppTypography.captionSmall.secondary),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _SectionCard extends StatelessWidget {
  final String title;
  final List<Widget> children;
  final Color? titleColor;

  const _SectionCard({
    required this.title,
    required this.children,
    this.titleColor,
  });

  @override
  Widget build(BuildContext context) {
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: AppTypography.caption.withColor(
              titleColor ?? AppColors.textSecondary,
            ),
          ),
          const Divider(height: 16),
          ...children,
        ],
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;
  const _InfoRow(this.label, this.value);

  @override
  Widget build(BuildContext context) {
    if (value.isEmpty) return const SizedBox.shrink();
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 130,
            child: Text(label, style: AppTypography.captionSmall.secondary),
          ),
          Expanded(child: Text(value, style: AppTypography.body)),
        ],
      ),
    );
  }
}

class _TepTile extends StatelessWidget {
  final FileAttachment tep;
  final VoidCallback? onTap;
  const _TepTile({required this.tep, this.onTap});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      dense: true,
      contentPadding: EdgeInsets.zero,
      leading: Icon(
        tep.isImage ? Icons.image_outlined : Icons.insert_drive_file_outlined,
        color: AppColors.primary,
      ),
      title: Text(
        tep.fileName.isNotEmpty ? tep.fileName : 'Tệp #${tep.id}',
        style: AppTypography.captionSmall,
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
      ),
      subtitle: tep.contentType.isNotEmpty
          ? Text(tep.contentType, style: AppTypography.captionSmall.secondary)
          : null,
      onTap: onTap,
    );
  }
}
