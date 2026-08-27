import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../models/thanh_vien_model.dart';
import '../../services/thanh_vien_service.dart';
import '../../widgets/tv_shared_widgets.dart';

import 'package:klks_app/design/design.dart';

class ThanhVienListTab extends StatefulWidget {
  final QuanHeCuTruModel item;

  const ThanhVienListTab({super.key, required this.item});

  @override
  State<ThanhVienListTab> createState() => _ThanhVienListTabState();
}

class _ThanhVienListTabState extends State<ThanhVienListTab>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  final _service = ThanhVienService.instance;

  bool _isLoading = false;
  String? _error;
  List<ThanhVienCuTruModel> _list = [];

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
      final result = await _service.getThanhVienCuTru(widget.item.canHoId);
      if (!mounted) return;
      setState(() => _list = result);
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _goToDetail(ThanhVienCuTruModel member) async {
    final reload = await context.push<bool>(
      '/cu-tru/thanh-vien/tv-detail',
      extra: {'thanhVien': member, 'canHoInfo': widget.item},
    );
    if (reload == true && mounted) _loadData();
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);

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
              Icons.people_outline,
              size: 56,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: AppSpacing.sm),
            Text('Chưa có thành viên nào', style: AppTypography.body.secondary),
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
        itemBuilder: (_, i) => _ThanhVienCard(
          member: _list[i],
          onTap: () => _goToDetail(_list[i]),
        ),
      ),
    );
  }
}

class _ThanhVienCard extends StatelessWidget {
  final ThanhVienCuTruModel member;
  final VoidCallback onTap;
  const _ThanhVienCard({required this.member, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return AppCard(
      onTap: onTap,
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm2,
      ),
      child: Row(
        children: [
          TvMemberAvatar(imageUrl: member.anhDaiDienUrl, name: member.fullName),
          const SizedBox(width: AppSpacing.sm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(member.fullName, style: AppTypography.subhead),
                Text(
                  member.loaiQuanHeTen,
                  style: AppTypography.caption.secondary,
                ),
                if (member.ngayBatDau != null)
                  Text(
                    'Từ ${member.ngayBatDau!.tvFormatted}',
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
