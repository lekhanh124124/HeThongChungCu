import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/quan_he_cu_tru_model.dart';
import '../services/cu_tru_service.dart';

class QuanHeCuTruListScreen extends StatefulWidget {
  const QuanHeCuTruListScreen({super.key});

  @override
  State<QuanHeCuTruListScreen> createState() => _QuanHeCuTruListScreenState();
}

class _QuanHeCuTruListScreenState extends State<QuanHeCuTruListScreen> {
  final _service = CuTruService.instance;

  bool _isLoading = false;
  String? _error;
  List<QuanHeCuTruModel> _list = [];

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
      final result = await _service.getQuanHeCuTruList();
      if (!mounted) return;
      setState(() => _list = result);
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      appBar: AppTopBar(
        title: 'Cư trú',
        actions: [
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
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_error != null) {
      return ErrorDisplay(error: _error, onRetry: _loadData);
    }

    if (_list.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.apartment_outlined,
              size: 56,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              'Không có dữ liệu cư trú',
              style: AppTypography.body.secondary,
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadData,
      color: AppColors.primary,
      child: ListView.separated(
        padding: AppSpacing.insetAll16,
        itemCount: _list.length,
        separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.sm),
        itemBuilder: (_, i) => _CuTruCard(
          item: _list[i],
          onThanhVien: () =>
              context.push('/cu-tru/thanh-vien', extra: _list[i]),
          onPhuongTien: () =>
              context.push('/cu-tru/phuong-tien', extra: _list[i]),
        ),
      ),
    );
  }
}

class _CuTruCard extends StatelessWidget {
  final QuanHeCuTruModel item;
  final VoidCallback onThanhVien;
  final VoidCallback onPhuongTien;

  const _CuTruCard({
    required this.item,
    required this.onThanhVien,
    required this.onPhuongTien,
  });

  @override
  Widget build(BuildContext context) {
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: AppColors.primaryLight,
                  borderRadius: AppRadius.buttonSmall,
                ),
                child: const Icon(
                  Icons.apartment_outlined,
                  color: AppColors.primary,
                  size: 20,
                ),
              ),
              const SizedBox(width: AppSpacing.sm),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(item.diaChiDayDu, style: AppTypography.subhead),
                    const SizedBox(height: 2),
                    Text(
                      'Mã: ${item.maCanHo}',
                      style: AppTypography.captionSmall.secondary,
                    ),
                  ],
                ),
              ),
              AppStatusBadge(
                label: item.loaiQuanHeTen,
                variant: AppBadgeVariant.info,
              ),
            ],
          ),

          const SizedBox(height: AppSpacing.sm),

          Row(
            children: [
              const Icon(
                Icons.people_outline,
                size: 14,
                color: AppColors.textSecondary,
              ),
              const SizedBox(width: 4),
              Text(
                '${item.tongCuDan} cư dân',
                style: AppTypography.caption.secondary,
              ),
              if (item.ngayBatDau != null) ...[
                const SizedBox(width: AppSpacing.md),
                const Icon(
                  Icons.calendar_today_outlined,
                  size: 14,
                  color: AppColors.textSecondary,
                ),
                const SizedBox(width: 4),
                Text(
                  'Từ ${_fmtDate(item.ngayBatDau!)}',
                  style: AppTypography.caption.secondary,
                ),
              ],
            ],
          ),

          const Divider(height: AppSpacing.lg),

          Row(
            children: [
              Expanded(
                child: AppButton(
                  label: 'Thành viên',
                  variant: AppButtonVariant.outline,
                  height: 40,
                  leadingIcon: Icons.people_outline,
                  onPressed: onThanhVien,
                ),
              ),
              const SizedBox(width: AppSpacing.sm),
              Expanded(
                child: AppButton(
                  label: 'Phương tiện',
                  variant: AppButtonVariant.outline,
                  height: 40,
                  leadingIcon: Icons.directions_car_outlined,
                  onPressed: onPhuongTien,
                ),
              ),
            ],
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
