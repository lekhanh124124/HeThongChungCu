import 'dart:async';
import 'package:flutter/material.dart';

import 'package:klks_app/core/navigation/app_navigation.dart';
import 'package:klks_app/design/design.dart';

import '../services/thong_bao_hub_service.dart';

class ThongBaoBellIcon extends StatefulWidget {
  final Color? iconColor;
  final double iconSize;

  const ThongBaoBellIcon({super.key, this.iconColor, this.iconSize = 26});

  @override
  State<ThongBaoBellIcon> createState() => _ThongBaoBellIconState();
}

class _ThongBaoBellIconState extends State<ThongBaoBellIcon>
    with SingleTickerProviderStateMixin {
  late final AnimationController _ringController;
  late final Animation<double> _ringAnim;
  StreamSubscription<int>? _countSub;
  int _unreadCount = 0;

  @override
  void initState() {
    super.initState();

    _ringController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 600),
    );
    _ringAnim = TweenSequence([
      TweenSequenceItem(tween: Tween(begin: 0.0, end: 0.25), weight: 1),
      TweenSequenceItem(tween: Tween(begin: 0.25, end: -0.22), weight: 1),
      TweenSequenceItem(tween: Tween(begin: -0.22, end: 0.18), weight: 1),
      TweenSequenceItem(tween: Tween(begin: 0.18, end: -0.12), weight: 1),
      TweenSequenceItem(tween: Tween(begin: -0.12, end: 0.06), weight: 1),
      TweenSequenceItem(tween: Tween(begin: 0.06, end: 0.0), weight: 1),
    ]).animate(CurvedAnimation(parent: _ringController, curve: Curves.easeOut));

    _countSub = ThongBaoHubService.instance.onUnreadCountChanged.listen((
      count,
    ) {
      if (!mounted) return;
      setState(() => _unreadCount = count);
      if (count > 0) {
        _ringController
          ..reset()
          ..forward();
      }
    });
  }

  @override
  void dispose() {
    _ringController.dispose();
    _countSub?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return IconButton(
      tooltip: 'Thông báo',
      onPressed: AppNavigation.goNotification,
      icon: Stack(
        clipBehavior: Clip.none,
        children: [
          AnimatedBuilder(
            animation: _ringAnim,
            builder: (_, child) => Transform.rotate(
              angle: _ringAnim.value,
              alignment: Alignment.topCenter,
              child: child,
            ),
            child: Icon(
              Icons.notifications_outlined,
              size: widget.iconSize,
              color: widget.iconColor,
            ),
          ),
          if (_unreadCount > 0)
            Positioned(top: -4, right: -6, child: _Badge(count: _unreadCount)),
        ],
      ),
    );
  }
}

class _Badge extends StatelessWidget {
  final int count;
  const _Badge({required this.count});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 5, vertical: 1),
      decoration: BoxDecoration(
        color: AppColors.error,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(
          color: Theme.of(context).scaffoldBackgroundColor,
          width: 1.5,
        ),
      ),
      child: Text(
        count > 99 ? '99+' : '$count',
        style: AppTypography.captionSmall.copyWith(
          color: AppColors.textOnPrimary,
          fontWeight: FontWeight.w700,
          fontSize: 10,
        ),
      ),
    );
  }
}
