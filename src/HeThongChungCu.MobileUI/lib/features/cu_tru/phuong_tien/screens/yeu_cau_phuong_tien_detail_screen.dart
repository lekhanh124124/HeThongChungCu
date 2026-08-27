import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/shared/widgets/full_screen_image_viewer.dart';

import '../models/phuong_tien_model.dart';
import '../services/phuong_tien_service.dart';

class YeuCauPhuongTienDetailScreen extends StatefulWidget {
  final int yeuCauId;
  final YeuCauPhuongTien? initialData;

  const YeuCauPhuongTienDetailScreen({
    super.key,
    required this.yeuCauId,
    this.initialData,
  });

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final extra = state.extra as Map<String, dynamic>;
    return YeuCauPhuongTienDetailScreen(
      yeuCauId: extra['yeuCauId'] as int,
      initialData: extra['initialData'] as YeuCauPhuongTien?,
    );
  }

  @override
  State<YeuCauPhuongTienDetailScreen> createState() =>
      _YeuCauPhuongTienDetailScreenState();
}

class _YeuCauPhuongTienDetailScreenState
    extends State<YeuCauPhuongTienDetailScreen> {
  final _service = PhuongTienService.instance;

  bool _isLoading = false;
  YeuCauPhuongTien? _data;

  @override
  void initState() {
    super.initState();
    if (widget.initialData != null) {
      _data = widget.initialData;
    } else {
      _loadData();
    }
  }

  Future<void> _loadData() async {
    setState(() => _isLoading = true);
    try {
      final result = await _service.getYeuCauById(widget.yeuCauId);
      if (mounted) setState(() => _data = result);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Chi tiết yêu cầu',
      actions: [
        if (_data != null)
          IconButton(
            icon: const Icon(Icons.refresh_rounded),
            tooltip: 'Làm mới',
            onPressed: _isLoading ? null : _loadData,
          ),
      ],
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading && _data == null) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_data == null) return const SizedBox.shrink();

    final d = _data!;

    return RefreshIndicator(
      color: AppColors.primary,
      onRefresh: _loadData,
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.md,
          vertical: AppSpacing.md,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _StatusBanner(data: d),
            AppSpacing.md.verticalSpace,
            _SectionCard(
              title: 'Thông tin yêu cầu',
              icon: Icons.info_outline_rounded,
              children: [
                _InfoRow(label: 'Mã yêu cầu', value: '#${d.id}'),
                _InfoRow(label: 'Loại yêu cầu', value: d.tenLoaiYeuCau),
                _InfoRow(label: 'Người gửi', value: d.tenNguoiGui),
                _InfoRow(label: 'Căn hộ', value: d.diaChiCanHo),
                if (d.createdAt != null)
                  _InfoRow(
                    label: 'Ngày tạo',
                    value: _fmtDateTime(d.createdAt!),
                  ),
                if (d.noiDung != null && d.noiDung!.isNotEmpty)
                  _InfoRow(
                    label: 'Nội dung',
                    value: d.noiDung!,
                    multiLine: true,
                  ),
              ],
            ),
            AppSpacing.sm.verticalSpace,
            _SectionCard(
              title: 'Thông tin phương tiện',
              icon: Icons.directions_car_outlined,
              children: [
                if (d.tenYeuCauLoaiPhuongTien != null)
                  _InfoRow(label: 'Loại xe', value: d.tenYeuCauLoaiPhuongTien!),
                if (d.yeuCauTenPhuongTien != null)
                  _InfoRow(label: 'Tên xe', value: d.yeuCauTenPhuongTien!),
                if (d.yeuCauBienSo != null)
                  _InfoRow(label: 'Biển số', value: d.yeuCauBienSo!),
                if (d.yeuCauMauXe != null)
                  _InfoRow(label: 'Màu xe', value: d.yeuCauMauXe!),
                if (d.tenYeuCauLoaiPhuongTien == null &&
                    d.yeuCauTenPhuongTien == null &&
                    d.yeuCauBienSo == null &&
                    d.yeuCauMauXe == null)
                  Text(
                    'Không có thông tin',
                    style: AppTypography.body.secondary,
                  ),
              ],
            ),
            AppSpacing.sm.verticalSpace,
            _SectionCard(
              title: 'Xử lý',
              icon: Icons.manage_accounts_outlined,
              children: [
                _InfoRow(
                  label: 'Trạng thái',
                  value: d.tenTrangThai,
                  valueColor: _trangThaiColor(d.trangThaiId),
                ),
                if (d.tenNguoiXuLy != null)
                  _InfoRow(label: 'Người xử lý', value: d.tenNguoiXuLy!),
                if (d.ngayXuLy != null)
                  _InfoRow(
                    label: 'Ngày xử lý',
                    value: _fmtDateTime(d.ngayXuLy!),
                  ),
                if (d.lyDo != null && d.lyDo!.isNotEmpty)
                  _InfoRow(
                    label: 'Lý do',
                    value: d.lyDo!,
                    valueColor: AppColors.error,
                    multiLine: true,
                  ),
              ],
            ),
            if (d.yeuCauHinhAnhPhuongTiens.isNotEmpty) ...[
              AppSpacing.sm.verticalSpace,
              _AttachmentSection(files: d.yeuCauHinhAnhPhuongTiens),
            ],
            AppSpacing.xl.verticalSpace,
          ],
        ),
      ),
    );
  }

  Color _trangThaiColor(int id) => switch (id) {
    2 => AppColors.success,
    3 => AppColors.error,
    4 => AppColors.secondary,
    _ => AppColors.warning,
  };

  String _fmtDateTime(DateTime d) =>
      '${d.day.toString().padLeft(2, '0')}/'
      '${d.month.toString().padLeft(2, '0')}/'
      '${d.year}  '
      '${d.hour.toString().padLeft(2, '0')}:'
      '${d.minute.toString().padLeft(2, '0')}';
}

class _StatusBanner extends StatelessWidget {
  final YeuCauPhuongTien data;

  const _StatusBanner({required this.data});

  @override
  Widget build(BuildContext context) {
    final (bg, fg, icon) = _config(data.trangThaiId);

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm2,
      ),
      decoration: BoxDecoration(color: bg, borderRadius: AppRadius.card),
      child: Row(
        children: [
          Container(
            padding: AppSpacing.insetAll8,
            decoration: BoxDecoration(
              color: fg.withValues(alpha: 0.12),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: fg, size: 22),
          ),
          AppSpacing.sm2.horizontalSpace,
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  data.tenTrangThai,
                  style: AppTypography.subhead.copyWith(color: fg),
                ),
                Text(
                  data.tenLoaiYeuCau,
                  style: AppTypography.captionSmall.copyWith(
                    color: fg.withValues(alpha: 0.75),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  (Color bg, Color fg, IconData icon) _config(int id) => switch (id) {
    2 => (
      AppColors.successLight,
      AppColors.success,
      Icons.check_circle_rounded,
    ),
    3 => (AppColors.errorLight, AppColors.error, Icons.cancel_rounded),
    4 => (AppColors.secondaryLight, AppColors.secondary, Icons.undo_rounded),
    _ => (
      AppColors.warningLight,
      AppColors.warning,
      Icons.hourglass_top_rounded,
    ),
  };
}

class _SectionCard extends StatelessWidget {
  final String title;
  final IconData icon;
  final List<Widget> children;

  const _SectionCard({
    required this.title,
    required this.icon,
    required this.children,
  });

  @override
  Widget build(BuildContext context) {
    return AppCard(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.md,
        AppSpacing.sm2,
        AppSpacing.md,
        AppSpacing.md,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, size: 16, color: AppColors.primary),
              AppSpacing.xs.horizontalSpace,
              Text(
                title,
                style: AppTypography.subhead.copyWith(color: AppColors.primary),
              ),
            ],
          ),
          const Padding(
            padding: EdgeInsets.symmetric(vertical: AppSpacing.sm),
            child: Divider(height: 1),
          ),
          ...children,
        ],
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;
  final Color? valueColor;
  final bool multiLine;

  const _InfoRow({
    required this.label,
    required this.value,
    this.valueColor,
    this.multiLine = false,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.xs),
      child: multiLine
          ? Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: AppTypography.captionSmall.secondary),
                const SizedBox(height: 2),
                Text(
                  value,
                  style: AppTypography.body.copyWith(
                    color: valueColor ?? AppColors.textPrimary,
                  ),
                ),
              ],
            )
          : Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SizedBox(
                  width: 110,
                  child: Text(label, style: AppTypography.body.secondary),
                ),
                Expanded(
                  child: Text(
                    value,
                    style: AppTypography.bodyMedium.copyWith(
                      color: valueColor ?? AppColors.textPrimary,
                    ),
                  ),
                ),
              ],
            ),
    );
  }
}

class _AttachmentSection extends StatelessWidget {
  final List<FileAttachment> files;

  const _AttachmentSection({required this.files});

  @override
  Widget build(BuildContext context) {
    final imageList = files.where((e) => e.isImage).toList();
    final fileList = files.where((e) => !e.isImage).toList();

    return AppCard(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.md,
        AppSpacing.sm2,
        AppSpacing.md,
        AppSpacing.md,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(
                Icons.attach_file_rounded,
                size: 16,
                color: AppColors.primary,
              ),
              AppSpacing.xs.horizontalSpace,
              Text(
                'Hình ảnh / Tài liệu đính kèm',
                style: AppTypography.subhead.copyWith(color: AppColors.primary),
              ),
            ],
          ),
          const Padding(
            padding: EdgeInsets.symmetric(vertical: AppSpacing.sm),
            child: Divider(height: 1),
          ),
          if (imageList.isNotEmpty)
            SizedBox(
              height: 104,
              child: ListView.separated(
                scrollDirection: Axis.horizontal,
                itemCount: imageList.length,
                separatorBuilder: (_, _) => AppSpacing.sm.horizontalSpace,
                itemBuilder: (context, i) => _ImageThumb(
                  image: imageList[i],
                  allImages: imageList,
                  index: i,
                ),
              ),
            ),
          if (imageList.isNotEmpty && fileList.isNotEmpty)
            AppSpacing.sm.verticalSpace,
          ...fileList.map((f) => _FileRow(file: f)),
        ],
      ),
    );
  }
}

class _ImageThumb extends StatelessWidget {
  final FileAttachment image;
  final List<FileAttachment> allImages;
  final int index;

  const _ImageThumb({
    required this.image,
    required this.allImages,
    required this.index,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => FullScreenImageViewer.show(
        context,
        files: allImages,
        initialIndex: index,
      ),
      child: ClipRRect(
        borderRadius: AppRadius.buttonSmall,
        child: Image.network(
          image.fileUrl,
          width: 104,
          height: 104,
          fit: BoxFit.cover,
          errorBuilder: (_, _, _) => Container(
            width: 104,
            height: 104,
            color: AppColors.inputFill,
            child: const Icon(
              Icons.broken_image_outlined,
              color: AppColors.textDisabled,
            ),
          ),
          loadingBuilder: (_, child, progress) {
            if (progress == null) return child;
            return Container(
              width: 104,
              height: 104,
              color: AppColors.inputFill,
              child: const Center(
                child: CircularProgressIndicator(
                  strokeWidth: 2,
                  color: AppColors.primary,
                ),
              ),
            );
          },
        ),
      ),
    );
  }
}

class _FileRow extends StatelessWidget {
  final FileAttachment file;

  const _FileRow({required this.file});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.xs),
      child: Row(
        children: [
          const Icon(
            Icons.insert_drive_file_outlined,
            size: 18,
            color: AppColors.secondary,
          ),
          AppSpacing.sm.horizontalSpace,
          Expanded(
            child: Text(
              file.fileName,
              overflow: TextOverflow.ellipsis,
              style: AppTypography.body.copyWith(
                color: AppColors.primary,
                decoration: TextDecoration.underline,
                decorationColor: AppColors.primary,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
