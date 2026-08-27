import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../../models/phuong_tien_model.dart';
import '../../services/phuong_tien_service.dart';

class LichSuYeuCauPhuongTienTab extends StatefulWidget {
  final QuanHeCuTruModel item;

  const LichSuYeuCauPhuongTienTab({super.key, required this.item});

  @override
  State<LichSuYeuCauPhuongTienTab> createState() =>
      LichSuYeuCauPhuongTienTabState();
}

class LichSuYeuCauPhuongTienTabState extends State<LichSuYeuCauPhuongTienTab>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  void reload() => _loadData();

  final _service = PhuongTienService.instance;
  final _scrollController = ScrollController();

  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _error;
  List<YeuCauPhuongTien> _list = [];
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
      final result = await _service.getListYeuCau(
        GetListYeuCauPhuongTienRequest(
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
      final result = await _service.getListYeuCau(
        GetListYeuCauPhuongTienRequest(
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
              Icons.description_outlined,
              size: 56,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              'Chưa có lịch sử yêu cầu phương tiện',
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
          return _YeuCauCard(yeuCau: _list[i]);
        },
      ),
    );
  }
}

class _YeuCauCard extends StatelessWidget {
  final YeuCauPhuongTien yeuCau;
  const _YeuCauCard({required this.yeuCau});

  @override
  Widget build(BuildContext context) {
    final xeInfo = [
      if (yeuCau.tenYeuCauLoaiPhuongTien != null)
        yeuCau.tenYeuCauLoaiPhuongTien!,
      if (yeuCau.yeuCauBienSo != null) yeuCau.yeuCauBienSo!,
      if (yeuCau.yeuCauMauXe != null) yeuCau.yeuCauMauXe!,
    ].join(' • ');

    return AppCard(
      onTap: () => context.push(
        '/cu-tru/phuong-tien/yc-detail',
        extra: {'yeuCauId': yeuCau.id, 'initialData': yeuCau},
      ),
      padding: const EdgeInsets.all(AppSpacing.md),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: AppColors.primaryLight,
              borderRadius: AppRadius.buttonSmall,
            ),
            child: yeuCau.yeuCauHinhAnhPhuongTiens.isNotEmpty
                ? ClipRRect(
                    borderRadius: AppRadius.buttonSmall,
                    child: Image.network(
                      yeuCau.yeuCauHinhAnhPhuongTiens.first.fileUrl,
                      fit: BoxFit.cover,
                    ),
                  )
                : Icon(
                    _loaiIcon(yeuCau.loaiYeuCauId),
                    color: AppColors.primary,
                    size: 20,
                  ),
          ),
          const SizedBox(width: AppSpacing.sm),

          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Text(
                        yeuCau.tenLoaiYeuCau,
                        style: AppTypography.subhead,
                      ),
                    ),
                    const SizedBox(width: AppSpacing.sm),
                    AppStatusBadge(
                      label: yeuCau.tenTrangThai,
                      variant: _trangThaiVariant(yeuCau.trangThaiId),
                    ),
                  ],
                ),
                const SizedBox(height: 4),

                if (xeInfo.isNotEmpty) ...[
                  Text(xeInfo, style: AppTypography.bodyMedium),
                  const SizedBox(height: 2),
                ],

                Text(
                  'Người gửi: ${yeuCau.tenNguoiGui}',
                  style: AppTypography.caption.secondary,
                ),

                if (yeuCau.createdAt != null)
                  Text(
                    'Ngày tạo: ${_fmtDate(yeuCau.createdAt!)}',
                    style: AppTypography.captionSmall.secondary,
                  ),

                if (yeuCau.lyDo != null && yeuCau.lyDo!.isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Row(
                    children: [
                      const Icon(
                        Icons.info_outline,
                        size: 13,
                        color: AppColors.error,
                      ),
                      const SizedBox(width: 4),
                      Expanded(
                        child: Text(
                          yeuCau.lyDo!,
                          style: AppTypography.captionSmall.error,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                    ],
                  ),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }

  AppBadgeVariant _trangThaiVariant(int id) => switch (id) {
    2 => AppBadgeVariant.success,
    3 => AppBadgeVariant.error,
    4 => AppBadgeVariant.info,
    _ => AppBadgeVariant.warning,
  };

  IconData _loaiIcon(int id) => switch (id) {
    1 => Icons.add_road,
    2 => Icons.no_crash,
    3 => Icons.edit_road,
    _ => Icons.description_outlined,
  };

  String _fmtDate(DateTime d) =>
      '${d.day.toString().padLeft(2, '0')}/'
      '${d.month.toString().padLeft(2, '0')}/'
      '${d.year}';
}
