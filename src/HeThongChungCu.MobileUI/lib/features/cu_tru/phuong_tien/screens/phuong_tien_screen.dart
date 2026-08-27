import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/phuong_tien_model.dart';

import 'tabs/phuong_tien_list_tab.dart';
import 'tabs/pt_lich_su_yeu_cau_tab.dart';

class PhuongTienScreen extends StatefulWidget {
  final QuanHeCuTruModel item;
  const PhuongTienScreen({super.key, required this.item});

  static Widget fromRoute(BuildContext context, GoRouterState state) =>
      PhuongTienScreen(item: state.extra! as QuanHeCuTruModel);

  @override
  State<PhuongTienScreen> createState() => _PhuongTienScreenState();
}

class _PhuongTienScreenState extends State<PhuongTienScreen>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;
  final _lichSuKey = GlobalKey<LichSuYeuCauPhuongTienTabState>();

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
      '/cu-tru/phuong-tien/tao-yeu-cau',
      extra: {'canHoInfo': widget.item, 'loaiYeuCauId': 1},
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
            Text('Phương tiện', style: AppTypography.captionSmall.secondary),
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
            Tab(icon: Icon(Icons.directions_car_outlined), text: 'Phương tiện'),
            Tab(icon: Icon(Icons.history_outlined), text: 'Lịch sử yêu cầu'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          PhuongTienListTab(item: widget.item),
          LichSuYeuCauPhuongTienTab(key: _lichSuKey, item: widget.item),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _onFabPressed,
        backgroundColor: AppColors.primary,
        foregroundColor: AppColors.textOnPrimary,
        tooltip: 'Thêm yêu cầu phương tiện',
        child: const Icon(Icons.add),
      ),
    );
  }
}
