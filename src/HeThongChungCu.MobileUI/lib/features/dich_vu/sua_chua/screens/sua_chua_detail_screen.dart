import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/services/cu_tru_service.dart';
import 'package:klks_app/features/shared/widgets/full_screen_image_viewer.dart';

import '../models/sua_chua_model.dart';
import '../services/sua_chua_service.dart';

class SuaChuaDetailScreen extends StatefulWidget {
  final int yeuCauId;
  const SuaChuaDetailScreen({super.key, required this.yeuCauId});

  @override
  State<SuaChuaDetailScreen> createState() => _SuaChuaDetailScreenState();
}

class _SuaChuaDetailScreenState extends State<SuaChuaDetailScreen> {
  final _service = YeuCauSuaChuaService.instance;
  final _cuTruService = CuTruService.instance;

  YeuCauSuaChua? _data;
  bool _isLoading = false;
  bool _isActioning = false;
  Object? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final data = await _service.getById(widget.yeuCauId);
      setState(() => _data = data);
    } catch (e) {
      setState(() => _error = e);
    } finally {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _thuHoi() async {
    final d = _data!;
    final confirm = await AppConfirmDialog.show(
      context,
      title: 'Thu hồi yêu cầu',
      message: 'Bạn chắc chắn muốn thu hồi?\nHành động này không thể hoàn tác.',
      confirmLabel: 'Thu hồi',
      isDangerous: true,
    );
    if (confirm != true || !mounted) return;

    setState(() => _isActioning = true);
    try {
      await _service.thuHoiYeuCau(
        id: d.id,
        phamViId: d.phamViId ?? 1,
        loaiSuCoId: d.loaiSuCoId ?? 1,
        noiDung: d.noiDung,
      );
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Thu hồi thành công'),
          backgroundColor: AppColors.success,
        ),
      );
      Navigator.pop(context, true);
    } catch (e) {
      if (!mounted) return;
      ErrorDisplay.showSnackBar(context, error: e);
    } finally {
      if (mounted) setState(() => _isActioning = false);
    }
  }

  Future<void> _navigateToEdit() async {
    final d = _data!;
    final dsCanHo = await _cuTruService.getQuanHeCuTruList();
    if (!mounted) return;
    final changed = await context.push<bool>(
      '/dich-vu/sua-chua/create',
      extra: {'dsCanHo': dsCanHo, 'editData': d},
    );
    if (changed == true) _load();
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Chi tiết #${widget.yeuCauId}',
      actions: [
        if (_data?.coTheChinhSua == true)
          IconButton(
            icon: const Icon(Icons.edit_outlined),
            onPressed: _navigateToEdit,
          ),
        IconButton(icon: const Icon(Icons.refresh), onPressed: _load),
      ],
      body: _buildBody(),
      bottomNavigationBar: _buildBottom(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) return const Center(child: CircularProgressIndicator());
    if (_error != null) {
      return ErrorDisplay.fullScreen(error: _error, onRetry: _load);
    }
    if (_data == null) return const SizedBox.shrink();

    final d = _data!;

    return ListView(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.md,
        AppSpacing.md,
        AppSpacing.md,
        80,
      ),
      children: [
        _StatusBanner(yeuCau: d),
        AppSpacing.md.verticalSpace,

        if (d.lyDo != null && d.lyDo!.isNotEmpty) ...[
          _SectionCard(
            title: 'Phản hồi từ BQL',
            titleColor: AppColors.warning,
            children: [_InfoRow('Lý do', d.lyDo!)],
          ),
          AppSpacing.sm.verticalSpace,
        ],

        _SectionCard(
          title: 'Thông tin yêu cầu',
          children: [
            _InfoRow('Căn hộ', d.diaChiDayDu),
            if (d.phamViTen != null) _InfoRow('Phạm vi', d.phamViTen!),
            if (d.loaiSuCoTen != null) _InfoRow('Loại sự cố', d.loaiSuCoTen!),
            _InfoRow('Nội dung', d.noiDung),
            if (d.moTaViTri != null && d.moTaViTri!.isNotEmpty)
              _InfoRow('Vị trí', d.moTaViTri!),
            if (d.tenNguoiGui != null) _InfoRow('Người gửi', d.tenNguoiGui!),
            if (d.createdAt != null)
              _InfoRow('Ngày tạo', _fmtDate(d.createdAt!)),
          ],
        ),
        AppSpacing.sm.verticalSpace,

        if (d.nhanSuSuaChuas.isNotEmpty) ...[
          _SectionCard(
            title: 'Nhân sự tác nghiệp',
            children: d.nhanSuSuaChuas.map(_buildNhanSuRow).toList(),
          ),
          AppSpacing.sm.verticalSpace,
        ],

        if (d.tenNguoiXuLy != null) ...[
          _SectionCard(
            title: 'Người xử lý',
            children: [
              _InfoRow('Nhân viên', d.tenNguoiXuLy!),
              if (d.ngayXuLy != null)
                _InfoRow('Ngày xử lý', _fmtDate(d.ngayXuLy!)),
            ],
          ),
          AppSpacing.sm.verticalSpace,
        ],

        if (d.henTu != null) ...[
          _SectionCard(
            title: 'Lịch hẹn kỹ thuật viên',
            children: [
              _InfoRow('Từ', _fmtDate(d.henTu!)),
              if (d.henDen != null) _InfoRow('Đến', _fmtDate(d.henDen!)),
            ],
          ),
          AppSpacing.sm.verticalSpace,
        ],

        if (d.chiPhiDuKien != null || d.chiPhiThucTe != null) ...[
          _SectionCard(
            title: 'Chi phí',
            children: [
              if (d.isMienPhi == true)
                const _InfoRow('Loại', 'Miễn phí (bảo trì / bảo hành)'),
              if (d.chiPhiDuKien != null)
                _InfoRow('Dự kiến', _fmtCurrency(d.chiPhiDuKien!)),
              if (d.chiPhiThucTe != null)
                _InfoRow('Thực tế', _fmtCurrency(d.chiPhiThucTe!)),
              if (d.ghiChuBaoGia != null && d.ghiChuBaoGia!.isNotEmpty)
                _InfoRow('Ghi chú', d.ghiChuBaoGia!),
            ],
          ),
          AppSpacing.sm.verticalSpace,
        ],

        if (d.tenDoiTac != null) ...[
          _SectionCard(
            title: 'Đối tác thực hiện',
            children: [_InfoRow('Đối tác', d.tenDoiTac!)],
          ),
          AppSpacing.sm.verticalSpace,
        ],

        if (d.ketQuaXuLy != null && d.ketQuaXuLy!.isNotEmpty) ...[
          _SectionCard(
            title: 'Kết quả xử lý',
            titleColor: AppColors.success,
            children: [_InfoRow('Kết quả', d.ketQuaXuLy!)],
          ),
          AppSpacing.sm.verticalSpace,
        ],

        if (d.lyDoHuy != null && d.lyDoHuy!.isNotEmpty) ...[
          _SectionCard(
            title: 'Lý do hủy',
            titleColor: AppColors.error,
            children: [_InfoRow('Lý do', d.lyDoHuy!)],
          ),
          AppSpacing.sm.verticalSpace,
        ],

        if (d.danhSachTep.isNotEmpty)
          _SectionCard(
            title: 'Ảnh hiện trạng (${d.danhSachTep.length})',
            children: [_ImageStrip(files: d.danhSachTep)],
          ),
      ],
    );
  }

  Widget? _buildBottom() {
    final d = _data;
    if (d == null || !d.coTheThuHoi || _isLoading) return null;

    return SafeArea(
      child: Padding(
        padding: AppSpacing.insetAll16,
        child: AppButton(
          label: _isActioning ? 'Đang thu hồi...' : 'Thu hồi yêu cầu',
          variant: AppButtonVariant.danger,
          isLoading: _isActioning,
          leadingIcon: Icons.undo,
          onPressed: _isActioning ? null : _thuHoi,
        ),
      ),
    );
  }

  Widget _buildNhanSuRow(NhanSuSuaChua ns) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.xs),
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
                Text(ns.displayName, style: AppTypography.bodyMedium),
                Text(ns.vaiTro, style: AppTypography.captionSmall.secondary),
                if (ns.soDienThoai != null)
                  Text(
                    ns.soDienThoai!,
                    style: AppTypography.captionSmall.primary,
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  String _fmtDate(DateTime dt) {
    final l = dt.toLocal();
    return '${l.day}/${l.month}/${l.year} '
        '${l.hour.toString().padLeft(2, '0')}:${l.minute.toString().padLeft(2, '0')}';
  }

  String _fmtCurrency(double v) => NumberFormat.currency(
    locale: 'vi_VN',
    symbol: 'đ',
    decimalDigits: 0,
  ).format(v);
}

class _StatusBanner extends StatelessWidget {
  final YeuCauSuaChua yeuCau;
  const _StatusBanner({required this.yeuCau});

  Color get _color {
    switch (yeuCau.trangThaiYeuCauId) {
      case TrangThaiYeuCau.completed:
        return AppColors.success;
      case TrangThaiYeuCau.pending:
      case TrangThaiYeuCau.returned:
        return AppColors.warning;
      case TrangThaiYeuCau.saved:
      case TrangThaiYeuCau.approved:
        return AppColors.primary;
      default:
        return AppColors.error;
    }
  }

  IconData get _icon {
    switch (yeuCau.trangThaiYeuCauId) {
      case TrangThaiYeuCau.pending:
        return Icons.hourglass_top_outlined;
      case TrangThaiYeuCau.approved:
        return Icons.check_circle_outline;
      case TrangThaiYeuCau.returned:
        return Icons.assignment_return_outlined;
      case TrangThaiYeuCau.completed:
        return Icons.task_alt;
      case TrangThaiYeuCau.saved:
        return Icons.drafts_outlined;
      default:
        return Icons.cancel_outlined;
    }
  }

  @override
  Widget build(BuildContext context) {
    final c = _color;
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm2,
      ),
      decoration: BoxDecoration(
        color: c.withAlpha(25),
        borderRadius: AppRadius.card,
        border: Border.all(color: c.withAlpha(100)),
      ),
      child: Row(
        children: [
          Icon(_icon, color: c, size: 30),
          AppSpacing.sm.horizontalSpace,
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  yeuCau.trangThaiYeuCauTen ?? '',
                  style: AppTypography.subhead.withColor(c),
                ),
                if (yeuCau.trangThaiYeuCauId == TrangThaiYeuCau.returned)
                  Text(
                    'Vui lòng bổ sung thông tin và gửi lại',
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
          const Divider(height: AppSpacing.md),
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
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 90,
            child: Text(label, style: AppTypography.captionSmall.secondary),
          ),
          Expanded(child: Text(value, style: AppTypography.body)),
        ],
      ),
    );
  }
}

class _ImageStrip extends StatelessWidget {
  final List<FileAttachment> files;
  const _ImageStrip({required this.files});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 100,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: files.length,
        separatorBuilder: (_, _) => AppSpacing.sm.horizontalSpace,
        itemBuilder: (ctx, i) {
          final f = files[i];
          return GestureDetector(
            onTap: () =>
                FullScreenImageViewer.show(ctx, files: files, initialIndex: i),
            child: Hero(
              tag: 'img_${f.id}',
              child: ClipRRect(
                borderRadius: AppRadius.buttonSmall,
                child: Image.network(
                  f.fileUrl,
                  width: 100,
                  height: 100,
                  fit: BoxFit.cover,
                  errorBuilder: (_, _, _) => Container(
                    width: 100,
                    height: 100,
                    color: AppColors.inputFill,
                    child: const Icon(
                      Icons.broken_image_outlined,
                      color: AppColors.textDisabled,
                    ),
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}
