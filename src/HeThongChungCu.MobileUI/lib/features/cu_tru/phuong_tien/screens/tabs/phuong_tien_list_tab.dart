import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../../models/phuong_tien_model.dart';
import '../../services/phuong_tien_service.dart';

class PhuongTienListTab extends StatefulWidget {
  final QuanHeCuTruModel item;

  const PhuongTienListTab({super.key, required this.item});

  @override
  State<PhuongTienListTab> createState() => _PhuongTienListTabState();
}

class _PhuongTienListTabState extends State<PhuongTienListTab>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  final _service = PhuongTienService.instance;
  final _scrollController = ScrollController();

  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _error;
  List<PhuongTien> _list = [];
  int _pageNumber = 1;
  static const int _pageSize = 10;
  bool _hasMore = true;

  @override
  void initState() {
    super.initState();
    _loadData();
    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    final pos = _scrollController.position;
    if (pos.pixels >= pos.maxScrollExtent - 100 &&
        _hasMore &&
        !_isLoadingMore) {
      _loadMore();
    }
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
      _error = null;
      _pageNumber = 1;
      _hasMore = true;
    });

    try {
      final result = await _service.getListPhuongTien(
        GetListPhuongTienRequest(
          pageNumber: 1,
          pageSize: _pageSize,
          canHoId: widget.item.canHoId,
        ),
      );
      if (!mounted) return;
      setState(() {
        _list = result.items;
        _hasMore = result.pagingInfo.hasNextPage;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _loadMore() async {
    setState(() => _isLoadingMore = true);

    try {
      final nextPage = _pageNumber + 1;
      final result = await _service.getListPhuongTien(
        GetListPhuongTienRequest(
          pageNumber: nextPage,
          pageSize: _pageSize,
          canHoId: widget.item.canHoId,
        ),
      );
      if (!mounted) return;
      setState(() {
        _list.addAll(result.items);
        _pageNumber = nextPage;
        _hasMore = result.pagingInfo.hasNextPage;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    } finally {
      if (mounted) setState(() => _isLoadingMore = false);
    }
  }

  void _goToDetail(PhuongTien pt) {
    context.push(
      '/cu-tru/phuong-tien/pt-detail',
      extra: {'phuongTienId': pt.id, 'canHoInfo': widget.item, 'snapshot': pt},
    );
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
              Icons.commute_outlined,
              size: 56,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              'Chưa có phương tiện nào',
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
        controller: _scrollController,
        padding: AppSpacing.insetAll16,
        itemCount: _list.length + (_isLoadingMore ? 1 : 0),
        separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.sm),
        itemBuilder: (_, i) {
          if (i == _list.length) {
            return const Center(
              child: Padding(
                padding: EdgeInsets.all(AppSpacing.md),
                child: CircularProgressIndicator(color: AppColors.primary),
              ),
            );
          }
          return _PhuongTienCard(
            pt: _list[i],
            onTap: () => _goToDetail(_list[i]),
          );
        },
      ),
    );
  }
}

class _PhuongTienCard extends StatelessWidget {
  final PhuongTien pt;
  final VoidCallback onTap;

  const _PhuongTienCard({required this.pt, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final hasAnh = pt.hinhAnhPhuongTiens.isNotEmpty;

    return AppCard(
      onTap: onTap,
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm2,
      ),
      child: Row(
        children: [
          CircleAvatar(
            radius: 22,
            backgroundColor: AppColors.primaryLight,
            backgroundImage: hasAnh
                ? NetworkImage(pt.hinhAnhPhuongTiens.first.fileUrl)
                : null,
            child: !hasAnh
                ? Icon(
                    _loaiIcon(pt.loaiPhuongTienId),
                    color: AppColors.primary,
                    size: 20,
                  )
                : null,
          ),
          const SizedBox(width: AppSpacing.sm),

          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(pt.bienSo, style: AppTypography.subhead),
                const SizedBox(height: 2),
                Text(
                  '${pt.tenLoaiPhuongTien} • ${pt.tenPhuongTien}',
                  style: AppTypography.caption.secondary,
                ),
                Text(
                  'Màu: ${pt.mauXe}',
                  style: AppTypography.captionSmall.secondary,
                ),
              ],
            ),
          ),

          Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              AppStatusBadge(
                label: pt.tenTrangThaiPhuongTien,
                variant: _trangThaiVariant(pt.trangThaiPhuongTienId),
              ),
              const SizedBox(height: 4),
              const Icon(
                Icons.chevron_right,
                size: 16,
                color: AppColors.textSecondary,
              ),
            ],
          ),
        ],
      ),
    );
  }

  AppBadgeVariant _trangThaiVariant(int id) => switch (id) {
    1 => AppBadgeVariant.success,
    2 => AppBadgeVariant.info,
    _ => AppBadgeVariant.warning,
  };

  IconData _loaiIcon(int id) => switch (id) {
    1 => Icons.two_wheeler,
    2 => Icons.directions_car,
    3 => Icons.pedal_bike,
    _ => Icons.commute,
  };
}
