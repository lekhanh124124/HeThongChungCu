import 'dart:async';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/thong_bao_model.dart';
import '../services/thong_bao_service.dart';
import '../services/thong_bao_hub_service.dart';

class ThongBaoListScreen extends StatefulWidget {
  const ThongBaoListScreen({super.key});

  @override
  State<ThongBaoListScreen> createState() => _ThongBaoListScreenState();
}

class _ThongBaoListScreenState extends State<ThongBaoListScreen> {
  final _service = ThongBaoService.instance;

  final List<ThongBaoItem> _items = [];
  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _error;
  int _pageNumber = 0;
  bool _hasMore = true;
  bool _onlyUnread = true;

  final _scrollController = ScrollController();
  StreamSubscription<ThongBaoEvent>? _hubSub;

  @override
  void initState() {
    super.initState();
    _loadData(reset: true);
    _scrollController.addListener(_onScroll);

    ThongBaoHubService.instance.resetUnreadCount();

    _hubSub = ThongBaoHubService.instance.onThongBaoMoi.listen((event) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('🔔 ${event.tieuDe}'),
          duration: const Duration(seconds: 3),
          action: SnackBarAction(
            label: 'Làm mới',
            onPressed: () => _loadData(reset: true),
          ),
        ),
      );
      _loadData(reset: true);
    });
  }

  @override
  void dispose() {
    _scrollController.dispose();
    _hubSub?.cancel();
    super.dispose();
  }

  void _onScroll() {
    final pos = _scrollController.position;
    if (pos.pixels >= pos.maxScrollExtent - 200 &&
        !_isLoadingMore &&
        _hasMore) {
      _loadMore();
    }
  }

  Future<void> _loadData({bool reset = false}) async {
    if (reset) {
      setState(() {
        _isLoading = true;
        _error = null;
        _pageNumber = 0;
        _hasMore = true;
      });
    }

    final result = await _service.getList(
      pageNumber: 0,
      onlyUnread: _onlyUnread,
    );

    if (!mounted) return;

    if (result.isOk) {
      setState(() {
        _items
          ..clear()
          ..addAll(result.data!.items);
        _hasMore = result.data!.pagingInfo.hasNextPage;
        _pageNumber = 1;
        _isLoading = false;
      });
    } else {
      setState(() {
        _error = result.errorMessage;
        _isLoading = false;
      });
    }
  }

  Future<void> _loadMore() async {
    setState(() => _isLoadingMore = true);

    final result = await _service.getList(
      pageNumber: _pageNumber,
      onlyUnread: _onlyUnread,
    );

    if (!mounted) return;

    setState(() => _isLoadingMore = false);

    if (result.isOk) {
      setState(() {
        _items.addAll(result.data!.items);
        _hasMore = result.data!.pagingInfo.hasNextPage;
        _pageNumber++;
      });
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(result.errorMessage!)));
    }
  }

  Future<void> _markAsRead(int index) async {
    final item = _items[index];
    if (item.isRead) return;

    final result = await _service.daDoc(phanBoThongBaoId: item.id);

    if (!mounted) return;

    if (result.isOk) {
      setState(() {
        _items[index] = item.copyWith(isRead: true, readAt: DateTime.now());
      });
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(result.errorMessage!)));
    }
  }

  int get _unreadCount => _items.where((e) => !e.isRead).length;

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      appBar: AppTopBar(
        title: 'Thông báo',
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: 4),
            child: FilterChip(
              label: Text('Chưa đọc', style: AppTypography.captionSmall),
              selected: _onlyUnread,
              onSelected: (val) {
                setState(() => _onlyUnread = val);
                _loadData(reset: true);
              },
            ),
          ),
          StreamBuilder(
            stream: ThongBaoHubService.instance.onConnectionStateChanged,
            builder: (context, _) {
              final connected = ThongBaoHubService.instance.isConnected;
              return Padding(
                padding: const EdgeInsets.only(right: 12, left: 4),
                child: Tooltip(
                  message: connected
                      ? 'Real-time: Đang kết nối'
                      : 'Real-time: Mất kết nối',
                  child: Icon(
                    Icons.circle,
                    size: 10,
                    color: connected
                        ? AppColors.success
                        : AppColors.textDisabled,
                  ),
                ),
              );
            },
          ),
          if (_unreadCount > 0)
            Padding(
              padding: const EdgeInsets.only(right: 8),
              child: Center(child: _UnreadBadge(count: _unreadCount)),
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
      return ErrorDisplay(error: _error, onRetry: () => _loadData(reset: true));
    }

    if (_items.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              Icons.notifications_none,
              size: 64,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: 8),
            Text(
              _onlyUnread
                  ? 'Không có thông báo chưa đọc'
                  : 'Không có thông báo nào',
              style: AppTypography.body.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: () => _loadData(reset: true),
      color: AppColors.primary,
      child: ListView.separated(
        controller: _scrollController,
        itemCount: _items.length + (_isLoadingMore ? 1 : 0),
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          if (index == _items.length) {
            return const Padding(
              padding: EdgeInsets.symmetric(vertical: 20),
              child: Center(
                child: CircularProgressIndicator(color: AppColors.primary),
              ),
            );
          }
          final item = _items[index];
          return _ThongBaoTile(
            item: item,
            onTap: () async {
              final captured = item;
              await _markAsRead(index);
              if (!context.mounted) return;
              context.push('/thong-bao/detail', extra: captured);
            },
          );
        },
      ),
    );
  }
}

class _UnreadBadge extends StatelessWidget {
  final int count;
  const _UnreadBadge({required this.count});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 7, vertical: 2),
      decoration: BoxDecoration(
        color: AppColors.error,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(
        count > 99 ? '99+' : '$count',
        style: AppTypography.captionSmall.copyWith(
          color: AppColors.textOnPrimary,
        ),
      ),
    );
  }
}

class _ThongBaoTile extends StatelessWidget {
  final ThongBaoItem item;
  final VoidCallback onTap;

  const _ThongBaoTile({required this.item, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      child: ColoredBox(
        color: item.isRead
            ? Colors.transparent
            : AppColors.primaryLight.withAlpha(80),
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Padding(
                padding: const EdgeInsets.only(top: 5, right: 10),
                child: Container(
                  width: 8,
                  height: 8,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: item.isRead ? Colors.transparent : AppColors.primary,
                  ),
                ),
              ),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Text(
                            item.tieuDe,
                            style: item.isRead
                                ? AppTypography.bodyMedium
                                : AppTypography.subhead,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                        const SizedBox(width: 8),
                        Text(
                          item.thoiGianHienThi,
                          style: AppTypography.captionSmall.copyWith(
                            color: AppColors.textSecondary,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    Text(
                      item.noiDung,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: AppTypography.body.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                    if (item.tenLoaiThongBao.isNotEmpty) ...[
                      const SizedBox(height: 6),
                      AppStatusBadge(
                        label: item.tenLoaiThongBao,
                        variant: AppBadgeVariant.info,
                      ),
                    ],
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
