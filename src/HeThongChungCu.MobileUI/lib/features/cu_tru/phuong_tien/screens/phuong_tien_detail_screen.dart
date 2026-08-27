import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/phuong_tien_model.dart';
import '../services/phuong_tien_service.dart';

class PhuongTienDetailScreen extends StatefulWidget {
  final int phuongTienId;
  final PhuongTien? snapshot;

  final QuanHeCuTruModel canHoInfo;

  const PhuongTienDetailScreen({
    super.key,
    required this.phuongTienId,
    required this.canHoInfo,
    this.snapshot,
  });

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;
    return PhuongTienDetailScreen(
      phuongTienId: e['phuongTienId'] as int,
      canHoInfo: e['canHoInfo'] as QuanHeCuTruModel,
      snapshot: e['snapshot'] as PhuongTien?,
    );
  }

  @override
  State<PhuongTienDetailScreen> createState() => _PhuongTienDetailScreenState();
}

class _PhuongTienDetailScreenState extends State<PhuongTienDetailScreen> {
  final _service = PhuongTienService.instance;

  bool _isLoading = false;
  String? _error;
  PhuongTien? _data;

  @override
  void initState() {
    super.initState();
    _data = widget.snapshot;
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final result = await _service.getPhuongTienById(widget.phuongTienId);
      if (!mounted) return;
      setState(() => _data = result);
    } on Exception catch (e) {
      if (!mounted) return;
      if (_data == null) {
        setState(() => _error = e.toString());
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Không thể làm mới: ${e.toString()}')),
        );
      }
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _goTaoYeuCau(int loaiYeuCauId) async {
    if (_data == null) return;
    final result = await context.push<bool>(
      '/cu-tru/phuong-tien/tao-yeu-cau',
      extra: {
        'canHoInfo': widget.canHoInfo,
        'loaiYeuCauId': loaiYeuCauId,
        'phuongTien': _data,
      },
    );
    if (result == true && mounted) _loadData();
  }

  void _showSnack(String msg) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
  }

  Future<void> _baoMatThe() async {
    final pt = _data;
    if (pt == null || pt.thePhuongTiens.isEmpty) {
      _showSnack('Xe này chưa có thẻ nào');
      return;
    }

    final selectedIds = await showModalBottomSheet<List<int>>(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(borderRadius: AppRadius.modal),
      builder: (_) => _BaoMatTheSheet(theList: pt.thePhuongTiens),
    );

    if (selectedIds == null || selectedIds.isEmpty || !mounted) return;

    final ok = await AppConfirmDialog.show(
      context,
      title: 'Xác nhận báo mất thẻ',
      message:
          'Báo mất ${selectedIds.length} thẻ đã chọn? '
          'Thẻ sẽ bị khoá và không thể hoàn tác.',
      confirmLabel: 'Báo mất',
      isDangerous: true,
    );
    if (ok != true || !mounted) return;

    try {
      await _service.baoMatThe(selectedIds);
      if (!mounted) return;
      _showSnack('Đã báo mất ${selectedIds.length} thẻ thành công');
    } on Exception catch (e) {
      if (!mounted) return;
      _showSnack(e.toString());
    }
  }

  @override
  Widget build(BuildContext context) {
    final title = _data?.bienSo ?? 'Chi tiết phương tiện';

    return AppScaffold(
      appBar: AppTopBar(
        title: title,
        actions: [
          if (_data != null) ...[
            PopupMenuButton<int>(
              tooltip: 'Tạo yêu cầu',
              icon: const Icon(Icons.more_vert),
              onSelected: _goTaoYeuCau,
              itemBuilder: (_) => const [
                PopupMenuItem(
                  value: 2,
                  child: Row(
                    children: [
                      Icon(Icons.edit_outlined, size: 18),
                      SizedBox(width: 8),
                      Text('Yêu cầu sửa thông tin'),
                    ],
                  ),
                ),
                PopupMenuItem(
                  value: 3,
                  child: Row(
                    children: [
                      Icon(Icons.delete_outline, size: 18, color: Colors.red),
                      SizedBox(width: 8),
                      Text(
                        'Yêu cầu huỷ đăng ký',
                        style: TextStyle(color: Colors.red),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ],
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Làm mới',
            onPressed: _isLoading ? null : _loadData,
          ),
        ],
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_data == null && _error != null) {
      return ErrorDisplay(error: _error, onRetry: _loadData);
    }

    if (_data == null) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    final d = _data!;

    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (d.hinhAnhPhuongTiens.isNotEmpty)
            _ImageGallery(images: d.hinhAnhPhuongTiens),

          if (_isLoading)
            const LinearProgressIndicator(color: AppColors.primary),

          Padding(
            padding: AppSpacing.insetAll16,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _VehicleHeader(d: d),
                const SizedBox(height: AppSpacing.md),

                _SectionCard(
                  title: 'Thông tin phương tiện',
                  children: [
                    _InfoRow('Biển số', d.bienSo),
                    _InfoRow('Tên xe', d.tenPhuongTien),
                    _InfoRow('Loại', d.tenLoaiPhuongTien),
                    _InfoRow('Màu xe', d.mauXe),
                    _InfoRow('Vị trí', d.viTriNgan),
                    _InfoRow('Trạng thái', d.tenTrangThaiPhuongTien),
                  ],
                ),

                if (d.thePhuongTiens.isNotEmpty) ...[
                  const SizedBox(height: AppSpacing.sm),
                  _TheSection(theList: d.thePhuongTiens),
                ],

                const SizedBox(height: AppSpacing.lg),

                if (d.thePhuongTiens.any(
                  (t) => t.trangThaiThePhuongTienId == 1,
                ))
                  AppButton(
                    label: 'Báo mất thẻ',
                    variant: AppButtonVariant.outline,
                    foregroundColor: AppColors.error,
                    leadingIcon: Icons.credit_card_off_outlined,
                    onPressed: () => _baoMatThe(),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _ImageGallery extends StatelessWidget {
  final List<FileAttachment> images;
  const _ImageGallery({required this.images});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 200,
      child: PageView.builder(
        itemCount: images.length,
        itemBuilder: (_, i) => Image.network(
          images[i].fileUrl,
          fit: BoxFit.cover,
          errorBuilder: (_, _, _) => const Center(
            child: Icon(
              Icons.broken_image_outlined,
              size: 48,
              color: AppColors.textDisabled,
            ),
          ),
          loadingBuilder: (_, child, progress) => progress == null
              ? child
              : const Center(
                  child: CircularProgressIndicator(color: AppColors.primary),
                ),
        ),
      ),
    );
  }
}

class _VehicleHeader extends StatelessWidget {
  final PhuongTien d;
  const _VehicleHeader({required this.d});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 56,
          height: 56,
          decoration: BoxDecoration(
            color: AppColors.primaryLight,
            borderRadius: AppRadius.card,
          ),
          child: Icon(
            _loaiIcon(d.loaiPhuongTienId),
            size: 28,
            color: AppColors.primary,
          ),
        ),
        const SizedBox(width: AppSpacing.md),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(d.bienSo, style: AppTypography.headline),
              Text(
                '${d.tenLoaiPhuongTien} • ${d.mauXe}',
                style: AppTypography.body.secondary,
              ),
            ],
          ),
        ),
        AppStatusBadge(
          label: d.tenTrangThaiPhuongTien,
          variant: _trangThaiVariant(d.trangThaiPhuongTienId),
        ),
      ],
    );
  }

  AppBadgeVariant _trangThaiVariant(int id) => switch (id) {
    1 => AppBadgeVariant.success,
    2 => AppBadgeVariant.info,
    _ => AppBadgeVariant.warning,
  };

  IconData _loaiIcon(int id) => switch (id) {
    1 => Icons.two_wheeler,
    2 => Icons.directions_car,
    3 => Icons.pedal_bike,
    _ => Icons.commute,
  };
}

class _TheSection extends StatelessWidget {
  final List<ThePhuongTien> theList;
  const _TheSection({required this.theList});

  @override
  Widget build(BuildContext context) {
    return _SectionCard(
      title: 'Thẻ phương tiện (${theList.length})',
      children: theList.map((the) => _TheTile(the: the)).toList(),
    );
  }
}

class _TheTile extends StatelessWidget {
  final ThePhuongTien the;
  const _TheTile({required this.the});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          const Icon(
            Icons.credit_card_outlined,
            size: 18,
            color: AppColors.textSecondary,
          ),
          const SizedBox(width: AppSpacing.sm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(the.maThe, style: AppTypography.subhead),
                if (the.ngayBatDau != null || the.ngayKetThuc != null)
                  Text(
                    [
                      if (the.ngayBatDau != null)
                        'Từ: ${_fmtDate(the.ngayBatDau!)}',
                      if (the.ngayKetThuc != null)
                        'Đến: ${_fmtDate(the.ngayKetThuc!)}',
                    ].join('  •  '),
                    style: AppTypography.captionSmall.secondary,
                  ),
              ],
            ),
          ),
          AppStatusBadge(
            label: the.tenTrangThaiThePhuongTien,
            variant: the.trangThaiThePhuongTienId == 1
                ? AppBadgeVariant.success
                : AppBadgeVariant.info,
          ),
        ],
      ),
    );
  }

  String _fmtDate(DateTime d) =>
      '${d.day.toString().padLeft(2, '0')}/'
      '${d.month.toString().padLeft(2, '0')}/'
      '${d.year}';
}

class _SectionCard extends StatelessWidget {
  final String title;
  final List<Widget> children;
  const _SectionCard({required this.title, required this.children});

  @override
  Widget build(BuildContext context) {
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(title, style: AppTypography.subhead),
          const Divider(height: AppSpacing.lg),
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
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 100,
            child: Text(label, style: AppTypography.caption.secondary),
          ),
          Expanded(child: Text(value, style: AppTypography.bodyMedium)),
        ],
      ),
    );
  }
}

class _BaoMatTheSheet extends StatefulWidget {
  final List<ThePhuongTien> theList;
  const _BaoMatTheSheet({required this.theList});

  @override
  State<_BaoMatTheSheet> createState() => _BaoMatTheSheetState();
}

class _BaoMatTheSheetState extends State<_BaoMatTheSheet> {
  final Set<int> _selected = {};

  String _fmtDate(DateTime d) =>
      '${d.day.toString().padLeft(2, '0')}/'
      '${d.month.toString().padLeft(2, '0')}/'
      '${d.year}';

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: Padding(
        padding: EdgeInsets.only(
          left: AppSpacing.md,
          right: AppSpacing.md,
          top: AppSpacing.md,
          bottom: MediaQuery.viewInsetsOf(context).bottom + AppSpacing.md,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 4,
                margin: const EdgeInsets.only(bottom: AppSpacing.md),
                decoration: BoxDecoration(
                  color: AppColors.border,
                  borderRadius: AppRadius.badge,
                ),
              ),
            ),

            Text('Chọn thẻ báo mất', style: AppTypography.headline),
            const SizedBox(height: 4),
            Text(
              'Chọn các thẻ cần báo mất. Thao tác này không thể hoàn tác.',
              style: AppTypography.caption.secondary,
            ),
            const SizedBox(height: AppSpacing.md),

            ...widget.theList.map((the) {
              final isActive = the.trangThaiThePhuongTienId == 1;
              final isSelected = _selected.contains(the.id);

              return AppCard(
                margin: const EdgeInsets.only(bottom: AppSpacing.sm),
                onTap: isActive
                    ? () => setState(() {
                        if (isSelected) {
                          _selected.remove(the.id);
                        } else {
                          _selected.add(the.id);
                        }
                      })
                    : null,
                color: isSelected ? AppColors.errorLight : null,
                child: Row(
                  children: [
                    Checkbox(
                      value: isSelected,
                      onChanged: isActive
                          ? (v) => setState(() {
                              v == true
                                  ? _selected.add(the.id)
                                  : _selected.remove(the.id);
                            })
                          : null,
                      activeColor: AppColors.error,
                    ),
                    const SizedBox(width: AppSpacing.xs),

                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              const Icon(
                                Icons.credit_card_outlined,
                                size: 16,
                                color: AppColors.textSecondary,
                              ),
                              const SizedBox(width: 6),
                              Text(the.maThe, style: AppTypography.subhead),
                            ],
                          ),
                          if (the.ngayBatDau != null ||
                              the.ngayKetThuc != null) ...[
                            const SizedBox(height: 2),
                            Text(
                              [
                                if (the.ngayBatDau != null)
                                  'Từ: ${_fmtDate(the.ngayBatDau!)}',
                                if (the.ngayKetThuc != null)
                                  'Đến: ${_fmtDate(the.ngayKetThuc!)}',
                              ].join('  •  '),
                              style: AppTypography.captionSmall.secondary,
                            ),
                          ],
                        ],
                      ),
                    ),

                    AppStatusBadge(
                      label: the.tenTrangThaiThePhuongTien,
                      variant: isActive
                          ? AppBadgeVariant.success
                          : AppBadgeVariant.info,
                    ),
                  ],
                ),
              );
            }),

            const SizedBox(height: AppSpacing.md),

            AppButton(
              label: _selected.isEmpty
                  ? 'Chọn ít nhất 1 thẻ'
                  : 'Báo mất ${_selected.length} thẻ',
              variant: AppButtonVariant.danger,
              onPressed: _selected.isEmpty
                  ? null
                  : () => Navigator.pop(context, _selected.toList()),
            ),
          ],
        ),
      ),
    );
  }
}
