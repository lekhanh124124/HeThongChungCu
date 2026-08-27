import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/thanh_vien_model.dart';

import 'tabs/thanh_vien_list_tab.dart';
import 'tabs/tv_lich_su_yeu_cau_tab.dart';

class ThanhVienScreen extends StatefulWidget {
  final QuanHeCuTruModel item;
  const ThanhVienScreen({super.key, required this.item});

  static Widget fromRoute(BuildContext context, GoRouterState state) =>
      ThanhVienScreen(item: state.extra! as QuanHeCuTruModel);

  @override
  State<ThanhVienScreen> createState() => _ThanhVienScreenState();
}

class _ThanhVienScreenState extends State<ThanhVienScreen>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;
  final _lichSuKey = GlobalKey<LichSuYeuCauThanhVienTabState>();

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _onFabPressed() async {
    final created = await context.push<bool>(
      '/cu-tru/thanh-vien/yc-form',
      extra: {'mode': 'create', 'canHoInfo': widget.item},
    );
    if (created == true && mounted) {
      _lichSuKey.currentState?.reload();
      _tabController.animateTo(1);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.textPrimary,
        elevation: 0,
        scrolledUnderElevation: 1,
        shadowColor: AppColors.border,
        surfaceTintColor: Colors.transparent,
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(widget.item.maCanHo, style: AppTypography.headline),
            Text('Thành viên', style: AppTypography.captionSmall.secondary),
          ],
        ),
        bottom: TabBar(
          controller: _tabController,
          labelColor: AppColors.primary,
          unselectedLabelColor: AppColors.textSecondary,
          indicatorColor: AppColors.primary,
          labelStyle: AppTypography.caption.copyWith(
            fontWeight: FontWeight.w600,
          ),
          unselectedLabelStyle: AppTypography.captionSmall,
          tabs: const [
            Tab(icon: Icon(Icons.people_outline), text: 'Thành viên'),
            Tab(icon: Icon(Icons.history_outlined), text: 'Lịch sử yêu cầu'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          ThanhVienListTab(item: widget.item),
          LichSuYeuCauThanhVienTab(key: _lichSuKey, item: widget.item),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _onFabPressed,
        backgroundColor: AppColors.primary,
        foregroundColor: AppColors.textOnPrimary,
        tooltip: 'Thêm yêu cầu thành viên',
        child: const Icon(Icons.add),
      ),
    );
  }
}
