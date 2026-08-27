import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../models/dich_vu_model.dart';
import '../services/dich_vu_service.dart';

class TienIchListScreen extends StatefulWidget {
  const TienIchListScreen({super.key});

  @override
  State<TienIchListScreen> createState() => _TienIchListScreenState();
}

class _TienIchListScreenState extends State<TienIchListScreen>
    with AutomaticKeepAliveClientMixin {
  final _service = DichVuService.instance;
  final _searchCtrl = TextEditingController();

  List<DichVuItem> _items = [];
  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _error;
  int _page = 1;
  bool _hasMore = true;

  @override
  bool get wantKeepAlive => true;

  @override
  void initState() {
    super.initState();
    _loadData(reset: true);
  }

  @override
  void dispose() {
    _searchCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadData({bool reset = false}) async {
    if (reset) {
      _page = 1;
      _hasMore = true;
    }

    setState(() {
      if (reset) {
        _isLoading = true;
      } else {
        _isLoadingMore = true;
      }
      _error = null;
    });

    try {
      final result = await _service.getDichVuList(
        keyword: _searchCtrl.text.trim(),
        pageNumber: _page,
      );
      if (!mounted) return;
      setState(() {
        if (reset) {
          _items = result.items;
        } else {
          _items.addAll(result.items);
        }
        _hasMore = result.pagingInfo.hasNextPage;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
          _isLoadingMore = false;
        });
      }
    }
  }

  void _loadMore() {
    if (_hasMore && !_isLoading && !_isLoadingMore) {
      _page++;
      _loadData();
    }
  }

  void _goDetail(DichVuItem item) {
    context.push('/dich-vu/tien-ich/detail/${item.id}');
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);
    return Column(
      children: [
        _SearchBar(
          controller: _searchCtrl,
          onSearch: () => _loadData(reset: true),
          onClear: () {
            _searchCtrl.clear();
            _loadData(reset: true);
          },
        ),
        Expanded(child: _buildBody()),
      ],
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_error != null && _items.isEmpty) {
      return ErrorDisplay(error: _error, onRetry: () => _loadData(reset: true));
    }

    if (_items.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.miscellaneous_services_outlined,
              size: 56,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: AppSpacing.sm),
            Text('Không có dịch vụ nào', style: AppTypography.body.secondary),
          ],
        ),
      );
    }

    return NotificationListener<ScrollNotification>(
      onNotification: (n) {
        if (n is ScrollEndNotification &&
            n.metrics.pixels >= n.metrics.maxScrollExtent - 200) {
          _loadMore();
        }
        return false;
      },
      child: RefreshIndicator(
        onRefresh: () => _loadData(reset: true),
        color: AppColors.primary,
        child: ListView.separated(
          padding: AppSpacing.insetAll16,
          itemCount: _items.length + (_isLoadingMore ? 1 : 0),
          separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.sm),
          itemBuilder: (_, i) {
            if (i == _items.length) {
              return const Center(
                child: Padding(
                  padding: EdgeInsets.all(AppSpacing.md),
                  child: CircularProgressIndicator(color: AppColors.primary),
                ),
              );
            }
            return _DichVuCard(
              item: _items[i],
              onDetail: () => _goDetail(_items[i]),
            );
          },
        ),
      ),
    );
  }
}

class _SearchBar extends StatelessWidget {
  final TextEditingController controller;
  final VoidCallback onSearch;
  final VoidCallback onClear;

  const _SearchBar({
    required this.controller,
    required this.onSearch,
    required this.onClear,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      color: AppColors.background,
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.md,
        AppSpacing.sm,
        AppSpacing.md,
        AppSpacing.sm,
      ),
      child: AppTextField.search(
        controller: controller,
        hint: 'Tìm theo mã hoặc tên dịch vụ...',
        onSubmitted: (_) => onSearch(),
      ),
    );
  }
}

class _DichVuCard extends StatelessWidget {
  final DichVuItem item;
  final VoidCallback onDetail;

  const _DichVuCard({required this.item, required this.onDetail});

  @override
  Widget build(BuildContext context) {
    return AppCard(
      onTap: onDetail,
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: item.isHoatDong
                  ? AppColors.primaryLight
                  : AppColors.secondaryLight,
              borderRadius: AppRadius.buttonSmall,
            ),
            child: Icon(
              Icons.miscellaneous_services_outlined,
              color: item.isHoatDong
                  ? AppColors.primary
                  : AppColors.textSecondary,
              size: 22,
            ),
          ),
          const SizedBox(width: AppSpacing.sm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(item.tenDichVu, style: AppTypography.subhead),
                const SizedBox(height: 2),
                Text(
                  'Mã: ${item.maDichVu}',
                  style: AppTypography.captionSmall.secondary,
                ),
                const SizedBox(height: 4),
                Row(
                  children: [
                    AppStatusBadge(
                      label: item.trangThaiDichVuTen,
                      variant: item.isHoatDong
                          ? AppBadgeVariant.success
                          : AppBadgeVariant.warning,
                    ),
                    if (item.isBatBuoc) ...[
                      const SizedBox(width: 6),
                      const AppStatusBadge(
                        label: 'Bắt buộc',
                        variant: AppBadgeVariant.info,
                      ),
                    ],
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
