import 'dart:async';
import 'package:flutter/material.dart';

import 'package:klks_app/design/design.dart';

import '../services/thong_bao_hub_service.dart';

class ThongBaoNavIcon extends StatefulWidget {
  final bool isActive;
  final double iconSize;

  const ThongBaoNavIcon({super.key, this.isActive = false, this.iconSize = 24});

  @override
  State<ThongBaoNavIcon> createState() => _ThongBaoNavIconState();
}

class _ThongBaoNavIconState extends State<ThongBaoNavIcon> {
  StreamSubscription<int>? _sub;
  int _count = 0;

  @override
  void initState() {
    super.initState();
    _sub = ThongBaoHubService.instance.onUnreadCountChanged.listen((c) {
      if (mounted) setState(() => _count = c);
    });
  }

  @override
  void dispose() {
    _sub?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      clipBehavior: Clip.none,
      children: [
        Icon(
          widget.isActive ? Icons.notifications : Icons.notifications_outlined,
          size: widget.iconSize,
        ),
        if (_count > 0)
          Positioned(top: -5, right: -8, child: _NavBadge(count: _count)),
      ],
    );
  }
}

class _NavBadge extends StatelessWidget {
  final int count;
  const _NavBadge({required this.count});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 1),
      decoration: BoxDecoration(
        color: AppColors.error,
        borderRadius: BorderRadius.circular(9),
        border: Border.all(
          color:
              Theme.of(context).bottomNavigationBarTheme.backgroundColor ??
              Theme.of(context).scaffoldBackgroundColor,
          width: 1.5,
        ),
      ),
      child: Text(
        count > 99 ? '99+' : '$count',
        style: AppTypography.captionSmall.copyWith(
          color: AppColors.textOnPrimary,
          fontWeight: FontWeight.w700,
          fontSize: 9,
        ),
      ),
    );
  }
}
