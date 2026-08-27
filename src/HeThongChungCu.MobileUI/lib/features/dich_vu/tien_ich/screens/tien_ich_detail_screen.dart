import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import 'package:klks_app/design/design.dart';

import '../models/dich_vu_model.dart';
import '../services/dich_vu_service.dart';

class TienIchDetailScreen extends StatefulWidget {
  final int dichVuId;

  const TienIchDetailScreen({super.key, required this.dichVuId});

  @override
  State<TienIchDetailScreen> createState() => _TienIchDetailScreenState();
}

class _TienIchDetailScreenState extends State<TienIchDetailScreen> {
  final _service = DichVuService.instance;
  DichVuDetail? _detail;
  bool _isLoading = true;
  String? _error;

  final _currencyFmt = NumberFormat.currency(locale: 'vi_VN', symbol: '₫');

  @override
  void initState() {
    super.initState();
    _loadDetail();
  }

  Future<void> _loadDetail() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final detail = await _service.getDichVuById(widget.dichVuId);
      if (!mounted) return;
      setState(() => _detail = detail);
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  void _goDangKy() {
    if (_detail == null) return;
    context.push(
      '/dich-vu/tien-ich/dang-ky',
      extra: {
        'dichVuId': _detail!.id,
        'tenDichVu': _detail!.tenDichVu,
        'khungGioList': _detail!.khungGioDichVu,
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      appBar: AppTopBar(
        title: _detail?.tenDichVu ?? 'Chi tiết tiện ích',
        actions: [
          if (_detail != null)
            IconButton(
              icon: const Icon(Icons.app_registration),
              tooltip: 'Đăng ký tiện ích',
              onPressed: _goDangKy,
            ),
        ],
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }
    if (_error != null) {
      return ErrorDisplay(error: _error, onRetry: _loadDetail);
    }

    final d = _detail!;

    return SingleChildScrollView(
      padding: AppSpacing.insetAll16,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          AppCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Text(d.tenDichVu, style: AppTypography.headline),
                    ),
                    const SizedBox(width: AppSpacing.sm),
                    AppStatusBadge(
                      label: d.trangThaiDichVuTen,
                      variant: d.isHoatDong
                          ? AppBadgeVariant.success
                          : AppBadgeVariant.warning,
                    ),
                  ],
                ),
                const Divider(height: AppSpacing.lg),
                _InfoRow('Mã tiện ích', d.maDichVu),
                _InfoRow('Loại', d.loaiDichVuTen),
                _InfoRow('Đơn vị tính', d.donViTinh),
                _InfoRow('Bắt buộc', d.isBatBuoc ? 'Có' : 'Không'),
                if (d.soLuongToiDa != null)
                  _InfoRow('Sức chứa tối đa', '${d.soLuongToiDa}'),
                if (d.moTa != null && d.moTa!.isNotEmpty)
                  _InfoRow('Mô tả', d.moTa!),
              ],
            ),
          ),

          if (d.bangGia != null) ...[
            const SizedBox(height: AppSpacing.sm),
            _SectionLabel('Bảng giá: ${d.bangGia!.tenBangGia}'),
            AppCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _InfoRow(
                    'Loại định giá',
                    DichVuCatalog.loaiDinhGiaName(d.bangGia!.loaiDinhGiaId),
                  ),
                  if (d.bangGia!.donGia > 0)
                    _InfoRow('Đơn giá', _currencyFmt.format(d.bangGia!.donGia)),
                  if (d.bangGia!.ngayApDung != null)
                    _InfoRow('Ngày áp dụng', _fmtDate(d.bangGia!.ngayApDung!)),
                  if (d.bangGia!.ngayKetThuc != null)
                    _InfoRow(
                      'Ngày kết thúc',
                      _fmtDate(d.bangGia!.ngayKetThuc!),
                    ),
                  if (d.bangGia!.giaLuyTiens.isNotEmpty) ...[
                    const Divider(height: AppSpacing.lg),
                    Text('Giá lũy tiến', style: AppTypography.subhead),
                    const SizedBox(height: AppSpacing.xs),
                    ...d.bangGia!.giaLuyTiens.map(
                      (g) => Padding(
                        padding: const EdgeInsets.symmetric(vertical: 2),
                        child: Text(
                          '• Từ ${g.tuMuc} → ${g.denMuc ?? '∞'}: '
                          '${_currencyFmt.format(g.donGia)}',
                          style: AppTypography.body,
                        ),
                      ),
                    ),
                  ],
                  if (d.bangGia!.giaKhungGios.isNotEmpty) ...[
                    const Divider(height: AppSpacing.lg),
                    Text('Giá theo khung giờ', style: AppTypography.subhead),
                    const SizedBox(height: AppSpacing.xs),
                    ...d.bangGia!.giaKhungGios.map(
                      (g) => Padding(
                        padding: const EdgeInsets.symmetric(vertical: 2),
                        child: Text(
                          '• ${g.tenKhungGio}: ${_currencyFmt.format(g.donGia)}',
                          style: AppTypography.body,
                        ),
                      ),
                    ),
                  ],
                ],
              ),
            ),
          ],

          if (d.khungGioDichVu.isNotEmpty) ...[
            const SizedBox(height: AppSpacing.sm),
            _SectionLabel('Khung giờ'),
            ...d.khungGioDichVu.map(
              (k) => AppCard(
                margin: const EdgeInsets.only(bottom: AppSpacing.xs),
                child: Row(
                  children: [
                    Icon(
                      Icons.access_time_outlined,
                      size: 18,
                      color: k.isActive
                          ? AppColors.primary
                          : AppColors.textDisabled,
                    ),
                    const SizedBox(width: AppSpacing.sm),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(k.tenKhungGio, style: AppTypography.subhead),
                          Text(
                            k.thoiGian,
                            style: AppTypography.caption.secondary,
                          ),
                        ],
                      ),
                    ),
                    AppStatusBadge(
                      label: k.isActive ? 'Hoạt động' : 'Tắt',
                      variant: k.isActive
                          ? AppBadgeVariant.success
                          : AppBadgeVariant.info,
                    ),
                  ],
                ),
              ),
            ),
          ],

          const SizedBox(height: AppSpacing.lg),

          AppButton(
            label: 'Đăng ký tiện ích',
            leadingIcon: Icons.app_registration,
            onPressed: _goDangKy,
          ),

          const SizedBox(height: AppSpacing.md),
        ],
      ),
    );
  }

  String _fmtDate(DateTime d) =>
      '${d.day.toString().padLeft(2, '0')}/'
      '${d.month.toString().padLeft(2, '0')}/'
      '${d.year}';
}

class _SectionLabel extends StatelessWidget {
  final String text;
  const _SectionLabel(this.text);

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(bottom: AppSpacing.sm),
    child: Text(
      text,
      style: AppTypography.subhead.copyWith(color: AppColors.primary),
    ),
  );
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
            width: 130,
            child: Text(label, style: AppTypography.caption.secondary),
          ),
          Expanded(child: Text(value, style: AppTypography.bodyMedium)),
        ],
      ),
    );
  }
}
