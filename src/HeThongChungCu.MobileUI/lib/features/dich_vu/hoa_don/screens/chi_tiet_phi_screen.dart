import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/hoa_don_model.dart';
import '../services/hoa_don_service.dart';

class ChiTietPhiScreen extends StatefulWidget {
  final ChiTietHoaDon chiTiet;

  const ChiTietPhiScreen({super.key, required this.chiTiet});

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;
    return ChiTietPhiScreen(chiTiet: e['chiTiet'] as ChiTietHoaDon);
  }

  @override
  State<ChiTietPhiScreen> createState() => _ChiTietPhiScreenState();
}

class _ChiTietPhiScreenState extends State<ChiTietPhiScreen> {
  bool _loading = true;
  Object? _error;

  ChiTietLuyTien? _luyTien;
  ChiTietCoDinh? _coDinh;
  ChiTietDienTich? _dienTich;
  ChiTietKhungGio? _khungGio;

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
      final id = widget.chiTiet.id;
      if (widget.chiTiet.laLuyTien) {
        _luyTien = await HoaDonService.instance.getChiTietLuyTien(id);
      } else if (widget.chiTiet.laCoDinh) {
        _coDinh = await HoaDonService.instance.getChiTietCoDinh(id);
      } else if (widget.chiTiet.laDienTich) {
        _dienTich = await HoaDonService.instance.getChiTietDienTich(id);
      } else if (widget.chiTiet.laKhungGio) {
        _khungGio = await HoaDonService.instance.getChiTietKhungGio(id);
      }
      if (!mounted) return;
      setState(() => _loading = false);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e;
        _loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(title: widget.chiTiet.tenMucPhi, body: _buildBody());
  }

  Widget _buildBody() {
    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_error != null) {
      return ErrorDisplay.fullScreen(error: _error, onRetry: _load);
    }

    if (_luyTien != null) return _LuyTienView(data: _luyTien!);
    if (_coDinh != null) return _CoDinhView(data: _coDinh!);
    if (_dienTich != null) return _DienTichView(data: _dienTich!);
    if (_khungGio != null) return _KhungGioView(data: _khungGio!);

    return Center(
      child: Text('Không có dữ liệu', style: AppTypography.body.secondary),
    );
  }
}

class _SectionCard extends StatelessWidget {
  final String title;
  final List<Widget> children;

  const _SectionCard({required this.title, required this.children});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm,
      ),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: AppRadius.card,
        boxShadow: AppElevation.level1,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.md,
              AppSpacing.sm2,
              AppSpacing.md,
              AppSpacing.sm,
            ),
            child: Text(
              title,
              style: AppTypography.subhead.copyWith(color: AppColors.primary),
            ),
          ),
          const Divider(height: 1),
          ...children,
        ],
      ),
    );
  }
}

class _DataRow extends StatelessWidget {
  final String label;
  final String value;
  final bool isBold;
  final Color? valueColor;

  const _DataRow({
    required this.label,
    required this.value,
    this.isBold = false,
    this.valueColor,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: 10,
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: AppTypography.body.secondary),
          Text(
            value,
            style: AppTypography.bodyMedium.copyWith(
              fontWeight: isBold ? FontWeight.w700 : FontWeight.w500,
              color: valueColor ?? AppColors.textPrimary,
            ),
          ),
        ],
      ),
    );
  }
}

class _LuyTienView extends StatelessWidget {
  final ChiTietLuyTien data;

  const _LuyTienView({required this.data});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.only(top: AppSpacing.md, bottom: AppSpacing.xl),
      children: [
        if (data.anhDongHoUrl.isNotEmpty)
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.md,
              0,
              AppSpacing.md,
              AppSpacing.sm,
            ),
            child: ClipRRect(
              borderRadius: AppRadius.card,
              child: Image.network(
                data.anhDongHoUrl,
                height: 180,
                width: double.infinity,
                fit: BoxFit.cover,
                errorBuilder: (_, _, _) => Container(
                  height: 120,
                  decoration: BoxDecoration(
                    color: AppColors.primaryLight,
                    borderRadius: AppRadius.card,
                  ),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(
                        Icons.image_not_supported_rounded,
                        color: AppColors.textDisabled,
                        size: 32,
                      ),
                      const SizedBox(height: 6),
                      Text(
                        'Không tải được ảnh đồng hồ',
                        style: AppTypography.captionSmall.secondary,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),

        _SectionCard(
          title: 'Thông tin chỉ số',
          children: [
            _DataRow(label: 'Chỉ số cũ', value: formatSoThap(data.chiSoCu)),
            const Divider(height: 1),
            _DataRow(label: 'Chỉ số mới', value: formatSoThap(data.chiSoMoi)),
            const Divider(height: 1),
            _DataRow(
              label: 'Tiêu thụ',
              value: '${formatSoThap(data.soLuongTieuThu)} đơn vị',
              isBold: true,
            ),
          ],
        ),

        if (data.bacThang.isNotEmpty)
          _SectionCard(
            title: 'Phân bổ theo bậc thang',
            children: [
              ...data.bacThang.asMap().entries.map((entry) {
                final i = entry.key;
                final bac = entry.value;
                return Column(
                  children: [
                    if (i > 0) const Divider(height: 1),
                    Padding(
                      padding: const EdgeInsets.all(AppSpacing.sm2),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Container(
                                width: 6,
                                height: 6,
                                decoration: const BoxDecoration(
                                  color: AppColors.primary,
                                  shape: BoxShape.circle,
                                ),
                              ),
                              const SizedBox(width: 8),
                              Text(bac.tenBac, style: AppTypography.bodyMedium),
                            ],
                          ),
                          const SizedBox(height: 8),
                          Row(
                            children: [
                              const SizedBox(width: 14),
                              Expanded(
                                child: Wrap(
                                  spacing: 16,
                                  runSpacing: 4,
                                  children: [
                                    _MiniInfo(
                                      label: 'Số lượng',
                                      value: formatSoThap(bac.soLuong),
                                    ),
                                    _MiniInfo(
                                      label: 'Đơn giá',
                                      value: formatTien(bac.donGia),
                                    ),
                                  ],
                                ),
                              ),
                              Text(
                                formatTien(bac.thanhTien),
                                style: AppTypography.bodyMedium.primary,
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ],
                );
              }),
              const Divider(height: 1),
              _DataRow(
                label: 'Tổng cộng',
                value: formatTien(data.thanhTien),
                isBold: true,
                valueColor: AppColors.primary,
              ),
            ],
          ),
      ],
    );
  }
}

class _MiniInfo extends StatelessWidget {
  final String label;
  final String value;

  const _MiniInfo({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: AppTypography.captionSmall.disabled),
        Text(value, style: AppTypography.captionSmall.secondary),
      ],
    );
  }
}

class _CoDinhView extends StatelessWidget {
  final ChiTietCoDinh data;

  const _CoDinhView({required this.data});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.only(top: AppSpacing.md, bottom: AppSpacing.xl),
      children: [
        _SectionCard(
          title: 'Thông tin phí cố định',
          children: [
            _DataRow(label: 'Tên mục phí', value: data.tenMucPhi),
            const Divider(height: 1),
            _DataRow(label: 'Số lượng', value: formatSoThap(data.soLuong)),
            const Divider(height: 1),
            _DataRow(label: 'Đơn giá', value: formatTien(data.donGia)),
            const Divider(height: 1),
            _DataRow(
              label: 'Thành tiền',
              value: formatTien(data.thanhTien),
              isBold: true,
              valueColor: AppColors.primary,
            ),
          ],
        ),
        if (data.ghiChu.isNotEmpty)
          _SectionCard(
            title: 'Ghi chú',
            children: [
              Padding(
                padding: AppSpacing.insetAll16,
                child: Text(data.ghiChu, style: AppTypography.body.secondary),
              ),
            ],
          ),
      ],
    );
  }
}

class _DienTichView extends StatelessWidget {
  final ChiTietDienTich data;

  const _DienTichView({required this.data});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.only(top: AppSpacing.md, bottom: AppSpacing.xl),
      children: [
        _SectionCard(
          title: 'Phí theo diện tích',
          children: [
            _DataRow(label: 'Loại căn hộ', value: data.tenLoaiCanHo),
            const Divider(height: 1),
            _DataRow(
              label: 'Diện tích',
              value: '${formatSoThap(data.dienTich)} m²',
            ),
            const Divider(height: 1),
            _DataRow(label: 'Đơn giá/m²', value: formatTien(data.donGia)),
            const Divider(height: 1),
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.md,
                vertical: AppSpacing.sm,
              ),
              child: Text(
                '${formatSoThap(data.dienTich)} m² × ${formatTien(data.donGia)}/m²',
                textAlign: TextAlign.center,
                style: AppTypography.captionSmall.secondary,
              ),
            ),
            const Divider(height: 1),
            _DataRow(
              label: 'Thành tiền',
              value: formatTien(data.thanhTien),
              isBold: true,
              valueColor: AppColors.primary,
            ),
          ],
        ),
      ],
    );
  }
}

class _KhungGioView extends StatelessWidget {
  final ChiTietKhungGio data;

  const _KhungGioView({required this.data});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.only(top: AppSpacing.md, bottom: AppSpacing.xl),
      children: [
        _SectionCard(
          title: 'Phân bổ theo khung giờ',
          children: [
            ...data.khungGios.asMap().entries.map((entry) {
              final i = entry.key;
              final kg = entry.value;
              return Column(
                children: [
                  if (i > 0) const Divider(height: 1),
                  Padding(
                    padding: const EdgeInsets.all(AppSpacing.sm2),
                    child: Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.all(8),
                          decoration: BoxDecoration(
                            color: AppColors.primaryLight,
                            borderRadius: AppRadius.buttonSmall,
                          ),
                          child: const Icon(
                            Icons.access_time_rounded,
                            color: AppColors.primary,
                            size: 16,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                kg.tenKhungGio,
                                style: AppTypography.bodyMedium,
                              ),
                              Text(
                                '${kg.gioBatDau} – ${kg.gioKetThuc}',
                                style: AppTypography.captionSmall.secondary,
                              ),
                            ],
                          ),
                        ),
                        Text(
                          formatTien(kg.donGia),
                          style: AppTypography.bodyMedium,
                        ),
                      ],
                    ),
                  ),
                ],
              );
            }),
            const Divider(height: 1),
            _DataRow(
              label: 'Tổng cộng',
              value: formatTien(data.thanhTien),
              isBold: true,
              valueColor: AppColors.primary,
            ),
          ],
        ),
      ],
    );
  }
}
