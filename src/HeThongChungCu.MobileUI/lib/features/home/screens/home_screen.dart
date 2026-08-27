import 'dart:async';
import 'package:flutter/material.dart';

import 'package:klks_app/core/navigation/app_navigation.dart';
import 'package:klks_app/core/storage/user_session.dart';
import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/ai/widgets/chat_fab.dart';
import 'package:klks_app/features/thong_bao/widgets/thong_bao_bell_icon.dart';
import 'package:klks_app/features/thong_bao/services/thong_bao_hub_service.dart';

import '../services/home_service.dart';
import '../widgets/home_thong_bao_section.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  StreamSubscription<ThongBaoEvent>? _hubSub;

  @override
  void initState() {
    super.initState();
    HomeService.instance.loadAll();

    _hubSub = ThongBaoHubService.instance.onThongBaoMoi.listen((_) {
      HomeService.instance.refreshThongBao();
    });
  }

  @override
  void dispose() {
    _hubSub?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final fullName = UserSession.instance.fullName ?? 'Người dùng';

    return AppScaffold(
      title: 'Trang chủ',
      actions: [const ThongBaoBellIcon(), AppSpacing.xs.horizontalSpace],
      floatingActionButton: const ChatFab(),
      body: RefreshIndicator(
        onRefresh: HomeService.instance.loadAll,
        color: AppColors.primary,
        child: ListView(
          padding: const EdgeInsets.symmetric(vertical: AppSpacing.md),
          children: [
            Padding(
              padding: AppSpacing.insetH16,
              child: _UserInfo(fullName: fullName),
            ),
            AppSpacing.md.verticalSpace,

            const HomeThongBaoSection(),
            AppSpacing.xl.verticalSpace,

            Padding(
              padding: AppSpacing.insetH16,
              child: const ChatBannerButton(),
            ),
            AppSpacing.xl.verticalSpace,

            Padding(
              padding: AppSpacing.insetH16,
              child: Text('Dịch vụ', style: AppTypography.subhead),
            ),
            AppSpacing.sm.verticalSpace,

            const _DichVuScroll(),
            AppSpacing.xl.verticalSpace,
          ],
        ),
      ),
    );
  }
}

class _UserInfo extends StatelessWidget {
  final String fullName;
  const _UserInfo({required this.fullName});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        ValueListenableBuilder<String?>(
          valueListenable: UserSession.instance.anhDaiDienUrlNotifier,
          builder: (_, url, _) => CircleAvatar(
            radius: 28,
            backgroundColor: AppColors.primaryLight,
            backgroundImage: url != null && url.isNotEmpty
                ? NetworkImage(url)
                : null,
            child: url == null || url.isEmpty
                ? const Icon(Icons.person, size: 28, color: AppColors.primary)
                : null,
          ),
        ),
        AppSpacing.md.horizontalSpace,
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Xin chào,', style: AppTypography.captionSmall.secondary),
              Text(fullName, style: AppTypography.headline),
            ],
          ),
        ),
      ],
    );
  }
}

class _ShortcutItem {
  final IconData icon;
  final String label;
  final Color color;
  final VoidCallback onTap;

  const _ShortcutItem({
    required this.icon,
    required this.label,
    required this.color,
    required this.onTap,
  });
}

class _DichVuScroll extends StatelessWidget {
  const _DichVuScroll();

  static final _items = <_ShortcutItem>[
    _ShortcutItem(
      icon: Icons.miscellaneous_services_outlined,
      label: 'Tiện ích',
      color: AppColors.primary,
      onTap: () => AppNavigation.goTabThenPush(2, '/dich-vu/tien-ich'),
    ),
    _ShortcutItem(
      icon: Icons.build_outlined,
      label: 'Sửa chữa',
      color: AppColors.accentOrange,
      onTap: () => AppNavigation.goTabThenPush(2, '/dich-vu/sua-chua'),
    ),
    _ShortcutItem(
      icon: Icons.construction_outlined,
      label: 'Thi công',
      color: AppColors.accentBrown,
      onTap: () => AppNavigation.goTabThenPush(2, '/dich-vu/thi-cong'),
    ),
    _ShortcutItem(
      icon: Icons.receipt_long_outlined,
      label: 'Hóa đơn',
      color: AppColors.success,
      onTap: () => AppNavigation.goTabThenPush(2, '/dich-vu/hoa-don'),
    ),
    _ShortcutItem(
      icon: Icons.campaign_outlined,
      label: 'Phản ánh',
      color: AppColors.error,
      onTap: () => AppNavigation.goTabThenPush(2, '/dich-vu/phan-anh'),
    ),
    _ShortcutItem(
      icon: Icons.poll_outlined,
      label: 'Khảo sát',
      color: AppColors.accentPurple,
      onTap: () => AppNavigation.goTabThenPush(2, '/dich-vu/khao-sat'),
    ),
  ];

  @override
  Widget build(BuildContext context) => _HorizontalShortcutList(items: _items);
}

class _HorizontalShortcutList extends StatelessWidget {
  final List<_ShortcutItem> items;
  const _HorizontalShortcutList({required this.items});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 96,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: AppSpacing.insetH16,
        itemCount: items.length,
        separatorBuilder: (_, _) => AppSpacing.sm.horizontalSpace,
        itemBuilder: (_, i) => _ShortcutCell(item: items[i]),
      ),
    );
  }
}

class _ShortcutCell extends StatelessWidget {
  final _ShortcutItem item;
  const _ShortcutCell({required this.item});

  @override
  Widget build(BuildContext context) {
    final bg = item.color.withAlpha(20);

    return SizedBox(
      width: 80,
      child: AppCard(
        onTap: item.onTap,
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.xs,
          vertical: AppSpacing.sm,
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              width: 42,
              height: 42,
              decoration: BoxDecoration(color: bg, shape: BoxShape.circle),
              child: Icon(item.icon, size: 22, color: item.color),
            ),
            AppSpacing.xs.verticalSpace,
            Text(
              item.label,
              style: AppTypography.captionSmall.withColor(
                AppColors.textPrimary,
              ),
              textAlign: TextAlign.center,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ),
      ),
    );
  }
}
