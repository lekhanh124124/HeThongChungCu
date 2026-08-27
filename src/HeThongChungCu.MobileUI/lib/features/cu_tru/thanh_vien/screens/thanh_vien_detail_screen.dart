import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:url_launcher/url_launcher.dart';

import 'package:klks_app/design/design.dart';

import '../models/thanh_vien_model.dart';
import '../services/thanh_vien_service.dart';
import '../widgets/tv_shared_widgets.dart';

class ThanhVienDetailScreen extends StatefulWidget {
  final ThanhVienCuTruModel thanhVien;
  final QuanHeCuTruModel canHoInfo;

  const ThanhVienDetailScreen({
    super.key,
    required this.thanhVien,
    required this.canHoInfo,
  });

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;
    return ThanhVienDetailScreen(
      thanhVien: e['thanhVien'] as ThanhVienCuTruModel,
      canHoInfo: e['canHoInfo'] as QuanHeCuTruModel,
    );
  }

  @override
  State<ThanhVienDetailScreen> createState() => _ThanhVienDetailScreenState();
}

class _ThanhVienDetailScreenState extends State<ThanhVienDetailScreen> {
  final _service = ThanhVienService.instance;

  bool _isLoading = false;
  String? _error;
  ThongTinCuDanModel? _data;

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final result = await _service.getThongTinCuDan(
        widget.thanhVien.quanHeCuTruId,
      );
      if (!mounted) return;
      setState(() => _data = result);
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _goToSua() async {
    if (_data == null) return;
    final reload = await context.push<bool>(
      '/cu-tru/thanh-vien/yc-form',
      extra: {
        'mode': 'edit',
        'thanhVien': widget.thanhVien,
        'canHoInfo': widget.canHoInfo,
        'thongTinCuDan': _data!,
      },
    );
    if (reload == true && mounted) _loadData();
  }

  Future<void> _goToXoa() async {
    final reload = await context.push<bool>(
      '/cu-tru/thanh-vien/xoa-yeu-cau',
      extra: {'thanhVien': widget.thanhVien, 'canHoInfo': widget.canHoInfo},
    );
    if (reload == true && mounted) Navigator.pop(context, true);
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      appBar: AppTopBar(
        title: widget.thanhVien.fullName,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Tải lại',
            onPressed: _isLoading ? null : _loadData,
          ),
        ],
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading && _data == null) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_error != null && _data == null) {
      return ErrorDisplay(error: _error, onRetry: _loadData);
    }

    if (_data == null) return const SizedBox.shrink();

    final d = _data!;

    return RefreshIndicator(
      onRefresh: _loadData,
      color: AppColors.primary,
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: AppSpacing.insetAll16,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (_isLoading)
              const LinearProgressIndicator(color: AppColors.primary),

            _AvatarHeader(d: d),
            const SizedBox(height: AppSpacing.md),

            TvSectionCard(
              title: 'Thông tin cá nhân',
              children: [
                TvInfoRow(label: 'Họ tên', value: d.fullName),
                TvInfoRow(label: 'Giới tính', value: d.gioiTinhName),
                if (d.dob != null)
                  TvInfoRow(label: 'Ngày sinh', value: d.dob!.tvFormatted),
                if (d.idCard != null)
                  TvInfoRow(label: 'CMND/CCCD', value: d.idCard!),
                if (d.phoneNumber != null)
                  TvInfoRow(label: 'SĐT', value: d.phoneNumber!),
                if (d.diaChi != null)
                  TvInfoRow(label: 'Địa chỉ', value: d.diaChi!),
              ],
            ),
            const SizedBox(height: AppSpacing.sm),

            TvSectionCard(
              title: 'Thông tin cư trú',
              children: [
                TvInfoRow(label: 'Quan hệ', value: d.loaiQuanHeTen),
                TvInfoRow(label: 'Trạng thái', value: d.trangThaiCuTruTen),
                if (d.ngayBatDau != null)
                  TvInfoRow(
                    label: 'Ngày bắt đầu',
                    value: d.ngayBatDau!.tvFormatted,
                  ),
                if (d.ngayKetThuc != null)
                  TvInfoRow(
                    label: 'Ngày kết thúc',
                    value: d.ngayKetThuc!.tvFormatted,
                  ),
              ],
            ),

            if (d.taiLieuCuTrus.isNotEmpty) ...[
              const SizedBox(height: AppSpacing.sm),
              TvSectionCard(
                title: 'Tài liệu cư trú',
                children: d.taiLieuCuTrus
                    .map((tl) => _TaiLieuItem(tl: tl))
                    .toList(),
              ),
            ],

            const SizedBox(height: AppSpacing.lg),

            Row(
              children: [
                Expanded(
                  child: AppButton(
                    label: 'Tạo yêu cầu sửa',
                    variant: AppButtonVariant.outline,
                    leadingIcon: Icons.edit_outlined,
                    height: 44,
                    onPressed: _data != null ? _goToSua : null,
                  ),
                ),
                const SizedBox(width: AppSpacing.sm),
                Expanded(
                  child: AppButton(
                    label: 'Tạo yêu cầu xóa',
                    variant: AppButtonVariant.danger,
                    leadingIcon: Icons.person_remove_outlined,
                    height: 44,
                    onPressed: _goToXoa,
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

class _AvatarHeader extends StatelessWidget {
  final ThongTinCuDanModel d;
  const _AvatarHeader({required this.d});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        TvMemberAvatar(
          imageUrl: d.anhDaiDienUrl,
          name: d.fullName,
          radius: 36,
          fontSize: 28,
        ),
        const SizedBox(width: AppSpacing.md),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(d.fullName, style: AppTypography.headline),
              const SizedBox(height: 4),
              AppStatusBadge(
                label: d.trangThaiCuTruTen,
                variant: AppBadgeVariant.info,
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _TaiLieuItem extends StatelessWidget {
  final TaiLieuCuTruModel tl;
  const _TaiLieuItem({required this.tl});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(tl.tenLoaiGiayTo, style: AppTypography.subhead),
        if (tl.soGiayTo.isNotEmpty) ...[
          const SizedBox(height: 2),
          Text(
            'Số: ${tl.soGiayTo}',
            style: AppTypography.captionSmall.secondary,
          ),
        ],
        if (tl.ngayPhatHanh != null) ...[
          const SizedBox(height: 2),
          Text(
            'Ngày phát hành: ${tl.ngayPhatHanh!.tvFormatted}',
            style: AppTypography.captionSmall.secondary,
          ),
        ],
        if (tl.files.isNotEmpty) ...[
          const SizedBox(height: 6),
          Wrap(
            spacing: 8,
            runSpacing: 6,
            children: tl.files.map((f) => _FileChip(file: f)).toList(),
          ),
        ],
        const Divider(height: AppSpacing.lg),
      ],
    );
  }
}

class _FileChip extends StatelessWidget {
  final FileAttachment file;
  const _FileChip({required this.file});

  Future<void> _open(BuildContext context) async {
    final uri = Uri.tryParse(file.fileUrl);
    if (uri == null) return;
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri, mode: LaunchMode.externalApplication);
    } else if (context.mounted) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Không thể mở file')));
    }
  }

  @override
  Widget build(BuildContext context) {
    return ActionChip(
      avatar: Icon(
        file.isPdf ? Icons.picture_as_pdf_outlined : Icons.image_outlined,
        size: 16,
        color: AppColors.primary,
      ),
      label: Text(
        file.fileName,
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
        style: AppTypography.captionSmall,
      ),
      onPressed: () => _open(context),
    );
  }
}
