import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/hoa_don_model.dart';
import '../services/hoa_don_service.dart';

class HoaDonDetailScreen extends StatefulWidget {
  final int hoaDonId;
  final String maHoaDon;

  const HoaDonDetailScreen({
    super.key,
    required this.hoaDonId,
    required this.maHoaDon,
  });

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;
    return HoaDonDetailScreen(
      hoaDonId: e['hoaDonId'] as int,
      maHoaDon: e['maHoaDon'] as String,
    );
  }

  @override
  State<HoaDonDetailScreen> createState() => _HoaDonDetailScreenState();
}

class _HoaDonDetailScreenState extends State<HoaDonDetailScreen> {
  HoaDonDetail? _detail;
  bool _loading = true;
  Object? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final detail = await HoaDonService.instance.getById(widget.hoaDonId);
      if (!mounted) return;
      setState(() {
        _detail = detail;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e;
        _loading = false;
      });
    }
  }

  void _goToThanhToan() {
    final detail = _detail;
    if (detail == null) return;
    context
        .push(
          '/dich-vu/hoa-don/detail/thanh-toan',
          extra: {
            'hoaDonId': detail.id,
            'maHoaDon': detail.maHoaDon,
            'tongTien': detail.tongTien,
            'chiTietHoaDonIds': detail.chiTietHoaDonIds,
          },
        )
        .then((_) => _load());
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      appBar: AppTopBar(
        title: 'Chi tiết hóa đơn',
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh_rounded),
            onPressed: _loading ? null : _load,
          ),
        ],
      ),
      body: _buildBody(),
      bottomNavigationBar: _buildBottomBar(),
    );
  }

  Widget _buildBody() {
    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_error != null) {
      return ErrorDisplay.fullScreen(error: _error, onRetry: _load);
    }

    final detail = _detail!;
    final cfg = getTrangThaiConfig(detail.trangThaiHoaDonId);

    return ListView(
      padding: const EdgeInsets.all(AppSpacing.md),
      children: [
        _SummaryCard(detail: detail, cfg: cfg),
        const SizedBox(height: AppSpacing.md),

        Padding(
          padding: const EdgeInsets.only(left: 4, bottom: 10),
          child: Text('Chi tiết các khoản phí', style: AppTypography.subhead),
        ),

        ...detail.chiTietHoaDons.map(
          (item) => Padding(
            padding: const EdgeInsets.only(bottom: 10),
            child: _ChiTietCard(chiTiet: item),
          ),
        ),

        const SizedBox(height: 80),
      ],
    );
  }

  Widget? _buildBottomBar() {
    final detail = _detail;
    if (detail == null || !detail.laCoTheThanhToan) return null;

    return Container(
      padding: EdgeInsets.fromLTRB(
        AppSpacing.md,
        12,
        AppSpacing.md,
        12 + MediaQuery.of(context).padding.bottom,
      ),
      decoration: BoxDecoration(
        color: AppColors.surface,
        boxShadow: AppElevation.level2,
      ),
      child: AppButton(
        label: 'Thanh toán ${formatTien(detail.tongTien)}',
        leadingIcon: Icons.qr_code_scanner_rounded,
        onPressed: _goToThanhToan,
      ),
    );
  }
}

class _SummaryCard extends StatelessWidget {
  final HoaDonDetail detail;
  final TrangThaiHoaDonConfig cfg;

  const _SummaryCard({required this.detail, required this.cfg});

  @override
  Widget build(BuildContext context) {
    final gradientColors = detail.laCoTheThanhToan
        ? [AppColors.primary, AppColors.primaryDark]
        : detail.laDaThanhToan
        ? [const Color(0xFF16A34A), const Color(0xFF059669)]
        : [AppColors.secondary, const Color(0xFF334155)];

    return Container(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: gradientColors,
        ),
        borderRadius: AppRadius.card,
        boxShadow: [
          BoxShadow(
            color: gradientColors.first.withAlpha(60),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      padding: AppSpacing.insetAll16,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(cfg.icon, color: Colors.white.withAlpha(230), size: 18),
              const SizedBox(width: 6),
              Text(
                cfg.ten,
                style: AppTypography.caption.copyWith(
                  color: Colors.white.withAlpha(230),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Text(
            formatTien(detail.tongTien),
            style: AppTypography.display.copyWith(
              color: Colors.white,
              letterSpacing: -0.5,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            detail.kyThanhToan,
            style: AppTypography.caption.copyWith(
              color: Colors.white.withAlpha(190),
            ),
          ),
          const SizedBox(height: 16),
          const Divider(color: Colors.white24, height: 1),
          const SizedBox(height: 14),
          _InfoRow(
            icon: Icons.tag_rounded,
            label: 'Mã hóa đơn',
            value: detail.maHoaDon,
          ),
          const SizedBox(height: 8),
          _InfoRow(
            icon: Icons.calendar_today_rounded,
            label: 'Ngày lập',
            value: formatNgay(detail.ngayLap),
          ),
          const SizedBox(height: 8),
          _InfoRow(
            icon: Icons.event_busy_rounded,
            label: 'Hạn thanh toán',
            value: formatNgay(detail.ngayHanThanhToan),
          ),
          if (detail.ghiChu.isNotEmpty) ...[
            const SizedBox(height: 8),
            _InfoRow(
              icon: Icons.notes_rounded,
              label: 'Ghi chú',
              value: detail.ghiChu,
            ),
          ],
        ],
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _InfoRow({
    required this.icon,
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Icon(icon, color: Colors.white54, size: 14),
        const SizedBox(width: 6),
        Text(
          '$label: ',
          style: AppTypography.captionSmall.copyWith(color: Colors.white60),
        ),
        Expanded(
          child: Text(
            value,
            style: AppTypography.caption.copyWith(color: Colors.white),
            overflow: TextOverflow.ellipsis,
          ),
        ),
      ],
    );
  }
}

class _ChiTietCard extends StatelessWidget {
  final ChiTietHoaDon chiTiet;

  const _ChiTietCard({required this.chiTiet});

  @override
  Widget build(BuildContext context) {
    final icon = getLoaiDinhGiaIcon(chiTiet.loaiDinhGiaId);
    final loaiLabel = getLoaiDinhGiaLabel(
      chiTiet.loaiDinhGiaId,
      chiTiet.loaiDinhGiaTen,
    );

    return AppCard(
      onTap: chiTiet.coChiTietDrillDown
          ? () => context.push(
              '/dich-vu/hoa-don/detail/chi-tiet-phi',
              extra: {'chiTiet': chiTiet},
            )
          : null,
      padding: const EdgeInsets.all(AppSpacing.sm2),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(9),
            decoration: BoxDecoration(
              color: AppColors.primaryLight,
              borderRadius: AppRadius.buttonSmall,
            ),
            child: Icon(icon, color: AppColors.primary, size: 18),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(chiTiet.tenMucPhi, style: AppTypography.bodyMedium),
                const SizedBox(height: 2),
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 6,
                        vertical: 2,
                      ),
                      decoration: BoxDecoration(
                        color: AppColors.primaryLight,
                        borderRadius: BorderRadius.circular(AppRadius.xs),
                      ),
                      child: Text(
                        loaiLabel,
                        style: AppTypography.captionSmall.copyWith(
                          color: AppColors.primary,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ),
                    if (chiTiet.ghiChu.isNotEmpty) ...[
                      const SizedBox(width: 6),
                      Expanded(
                        child: Text(
                          chiTiet.ghiChu,
                          style: AppTypography.captionSmall.secondary,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                    ],
                  ],
                ),
                if (!chiTiet.laLuyTien) ...[
                  const SizedBox(height: 4),
                  Text(
                    '${formatSoThap(chiTiet.soLuong)} × ${formatTien(chiTiet.donGia)}',
                    style: AppTypography.captionSmall.secondary,
                  ),
                ],
              ],
            ),
          ),
          const SizedBox(width: 8),
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Text(
                formatTien(chiTiet.thanhTien),
                style: AppTypography.headline.copyWith(
                  color: AppColors.textPrimary,
                ),
              ),
              if (chiTiet.coChiTietDrillDown) ...[
                const SizedBox(height: 2),
                Row(
                  children: [
                    Text('Chi tiết', style: AppTypography.captionSmall.primary),
                    const Icon(
                      Icons.chevron_right_rounded,
                      size: 14,
                      color: AppColors.primary,
                    ),
                  ],
                ),
              ],
            ],
          ),
        ],
      ),
    );
  }
}
