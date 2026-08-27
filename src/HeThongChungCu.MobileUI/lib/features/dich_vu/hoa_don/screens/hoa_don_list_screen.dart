import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/widgets/can_ho_selector.dart';

import '../models/hoa_don_model.dart';
import '../services/hoa_don_service.dart';

const _tabs = [
  (label: 'Tất cả', trangThaiId: null as int?),
  (label: 'Chờ duyệt', trangThaiId: 1),
  (label: 'Chưa thanh toán', trangThaiId: 2),
  (label: 'Đã thanh toán', trangThaiId: 3),
  (label: 'Quá hạn', trangThaiId: 4),
  (label: 'Một phần', trangThaiId: 5),
  (label: 'Đã hủy', trangThaiId: 6),
];

class HoaDonListScreen extends StatefulWidget {
  const HoaDonListScreen({super.key});

  @override
  State<HoaDonListScreen> createState() => _HoaDonListScreenState();
}

class _HoaDonListScreenState extends State<HoaDonListScreen>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;

  List<QuanHeCuTruModel> _dsCanHo = [];
  QuanHeCuTruModel? _selectedCanHo;
  bool _isLoadingCanHo = true;

  int? _filterThang;
  int? _filterNam;

  bool get _hasFilter => _filterThang != null || _filterNam != null;

  String get _filterLabel {
    if (_filterThang != null && _filterNam != null) {
      return 'T$_filterThang/$_filterNam';
    }
    if (_filterThang != null) return 'Tháng $_filterThang';
    if (_filterNam != null) return 'Năm $_filterNam';
    return 'Lọc';
  }

  String get _tabViewKey =>
      '${_selectedCanHo?.canHoId}-$_filterThang-$_filterNam';

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: _tabs.length, vsync: this);
    _loadCanHo();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _loadCanHo() async {
    setState(() => _isLoadingCanHo = true);
    try {
      final list = await HoaDonService.instance.getCanHoList();
      if (!mounted) return;
      setState(() {
        _dsCanHo = list;
        _selectedCanHo = list.isNotEmpty ? list.first : null;
        _isLoadingCanHo = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _isLoadingCanHo = false);
      ErrorDisplay.showSnackBar(context, error: e);
    }
  }

  void _onCanHoChanged(QuanHeCuTruModel canHo) =>
      setState(() => _selectedCanHo = canHo);

  Future<void> _openFilterSheet() async {
    int? tempThang = _filterThang;
    int? tempNam = _filterNam;

    final now = DateTime.now();
    final years = List.generate(now.year - 2019, (i) => now.year - i);

    await showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setSheet) {
          return Container(
            decoration: const BoxDecoration(
              color: AppColors.surface,
              borderRadius: AppRadius.modal,
            ),
            padding: EdgeInsets.fromLTRB(
              AppSpacing.md,
              AppSpacing.md,
              AppSpacing.md,
              AppSpacing.md + MediaQuery.of(ctx).padding.bottom,
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Center(
                  child: Container(
                    width: 40,
                    height: 4,
                    decoration: BoxDecoration(
                      color: AppColors.border,
                      borderRadius: AppRadius.badge,
                    ),
                  ),
                ),
                const SizedBox(height: AppSpacing.md),

                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text('Lọc theo tháng/năm', style: AppTypography.headline),
                    if (tempThang != null || tempNam != null)
                      TextButton(
                        onPressed: () => setSheet(() {
                          tempThang = null;
                          tempNam = null;
                        }),
                        child: Text(
                          'Xoá lọc',
                          style: AppTypography.buttonLabel.error,
                        ),
                      ),
                  ],
                ),
                const SizedBox(height: AppSpacing.md),

                Text('Năm', style: AppTypography.subhead.secondary),
                const SizedBox(height: AppSpacing.sm),
                Wrap(
                  spacing: AppSpacing.sm,
                  runSpacing: AppSpacing.sm,
                  children: years.map((y) {
                    final sel = tempNam == y;
                    return _FilterChip(
                      label: '$y',
                      selected: sel,
                      onTap: () => setSheet(() => tempNam = sel ? null : y),
                    );
                  }).toList(),
                ),
                const SizedBox(height: AppSpacing.md),

                Text('Tháng', style: AppTypography.subhead.secondary),
                const SizedBox(height: AppSpacing.sm),
                Wrap(
                  spacing: AppSpacing.sm,
                  runSpacing: AppSpacing.sm,
                  children: List.generate(12, (i) => i + 1).map((m) {
                    final sel = tempThang == m;
                    return _FilterChip(
                      label: 'T$m',
                      selected: sel,
                      width: 52,
                      onTap: () => setSheet(() => tempThang = sel ? null : m),
                    );
                  }).toList(),
                ),
                const SizedBox(height: AppSpacing.lg),

                AppButton(
                  label: 'Áp dụng',
                  onPressed: () {
                    setState(() {
                      _filterThang = tempThang;
                      _filterNam = tempNam;
                    });
                    Navigator.pop(ctx);
                  },
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoadingCanHo) {
      return const AppScaffold(
        title: 'Hóa đơn',
        body: Center(child: CircularProgressIndicator()),
      );
    }

    if (_dsCanHo.isEmpty) {
      return AppScaffold(
        title: 'Hóa đơn',
        body: Center(
          child: Padding(
            padding: AppSpacing.insetAll24,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Icon(
                  Icons.home_work_outlined,
                  size: 64,
                  color: AppColors.textDisabled,
                ),
                const SizedBox(height: AppSpacing.sm2),
                Text(
                  'Không tìm thấy thông tin căn hộ',
                  style: AppTypography.subhead,
                ),
                const SizedBox(height: AppSpacing.sm),
                Text(
                  'Vui lòng liên hệ Ban Quản Lý để được hỗ trợ.',
                  style: AppTypography.body.secondary,
                  textAlign: TextAlign.center,
                ),
              ],
            ),
          ),
        ),
      );
    }

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppTopBar(
        title: 'Hóa đơn',
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: AppSpacing.sm),
            child: TextButton.icon(
              onPressed: _openFilterSheet,
              icon: Icon(
                _hasFilter
                    ? Icons.filter_alt_rounded
                    : Icons.filter_alt_outlined,
                size: 18,
                color: _hasFilter ? AppColors.primary : AppColors.textDisabled,
              ),
              label: Text(
                _filterLabel,
                style: AppTypography.caption.copyWith(
                  fontWeight: FontWeight.w600,
                  color: _hasFilter
                      ? AppColors.primary
                      : AppColors.textDisabled,
                ),
              ),
              style: TextButton.styleFrom(
                padding: const EdgeInsets.symmetric(horizontal: AppSpacing.sm),
              ),
            ),
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          isScrollable: true,
          tabAlignment: TabAlignment.start,
          labelColor: AppColors.primary,
          unselectedLabelColor: AppColors.textSecondary,
          indicatorColor: AppColors.primary,
          indicatorSize: TabBarIndicatorSize.label,
          labelStyle: AppTypography.subhead,
          unselectedLabelStyle: AppTypography.body,
          tabs: _tabs.map((t) => Tab(text: t.label)).toList(),
        ),
      ),
      body: Column(
        children: [
          CanHoSelector(
            dsCanHo: _dsCanHo,
            selected: _selectedCanHo,
            onChanged: _onCanHoChanged,
          ),

          if (_hasFilter)
            Align(
              alignment: Alignment.centerLeft,
              child: Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.md,
                  AppSpacing.sm,
                  AppSpacing.md,
                  0,
                ),
                child: Chip(
                  avatar: const Icon(
                    Icons.calendar_month_outlined,
                    size: 14,
                    color: AppColors.primary,
                  ),
                  label: Text(
                    _filterLabel,
                    style: AppTypography.captionSmall.primary,
                  ),
                  onDeleted: () => setState(() {
                    _filterThang = null;
                    _filterNam = null;
                  }),
                  deleteIconColor: AppColors.textSecondary,
                  backgroundColor: AppColors.primaryLight,
                  side: BorderSide.none,
                  visualDensity: VisualDensity.compact,
                ),
              ),
            ),

          Expanded(
            child: TabBarView(
              controller: _tabController,
              key: ValueKey(_tabViewKey),
              children: _tabs.map((t) {
                final canHoId = _selectedCanHo?.canHoId ?? 0;
                if (canHoId <= 0) {
                  return Center(
                    child: Text(
                      'Chưa chọn căn hộ',
                      style: AppTypography.body.secondary,
                    ),
                  );
                }
                return _HoaDonTabContent(
                  canHoId: canHoId,
                  trangThaiId: t.trangThaiId,
                  filterThang: _filterThang,
                  filterNam: _filterNam,
                );
              }).toList(),
            ),
          ),
        ],
      ),
    );
  }
}

class _HoaDonTabContent extends StatefulWidget {
  final int canHoId;
  final int? trangThaiId;
  final int? filterThang;
  final int? filterNam;

  const _HoaDonTabContent({
    required this.canHoId,
    this.trangThaiId,
    this.filterThang,
    this.filterNam,
  });

  @override
  State<_HoaDonTabContent> createState() => _HoaDonTabContentState();
}

class _HoaDonTabContentState extends State<_HoaDonTabContent>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => false;

  List<HoaDon> _items = [];
  PagingInfo? _paging;
  bool _loading = false;
  bool _loadingMore = false;
  Object? _error;
  int _currentPage = 1;

  final _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _load(reset: true);
    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
            _scrollController.position.maxScrollExtent - 200 &&
        !_loadingMore &&
        (_paging?.hasNextPage ?? false)) {
      _loadMore();
    }
  }

  Future<void> _load({bool reset = false}) async {
    if (_loading) return;
    if (reset) {
      setState(() {
        _loading = true;
        _error = null;
        _currentPage = 1;
        _items = [];
      });
    }
    try {
      final result = await HoaDonService.instance.getList(
        canHoId: widget.canHoId,
        trangThaiHoaDonId: widget.trangThaiId,
        thang: widget.filterThang,
        nam: widget.filterNam,
        pageNumber: _currentPage,
      );
      if (!mounted) return;
      setState(() {
        _items = reset ? result.items : [..._items, ...result.items];
        _paging = result.pagingInfo;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e;
        _loading = false;
      });
    }
  }

  Future<void> _loadMore() async {
    final paging = _paging;
    if (paging == null || !paging.hasNextPage || _loadingMore || _loading) {
      return;
    }
    setState(() {
      _loadingMore = true;
      _currentPage++;
    });
    try {
      final result = await HoaDonService.instance.getList(
        canHoId: widget.canHoId,
        trangThaiHoaDonId: widget.trangThaiId,
        thang: widget.filterThang,
        nam: widget.filterNam,
        pageNumber: _currentPage,
      );
      if (!mounted) return;
      setState(() {
        _items = [..._items, ...result.items];
        _paging = result.pagingInfo;
        _loadingMore = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _loadingMore = false;
        _currentPage--;
      });
      ErrorDisplay.showSnackBar(context, error: e);
    }
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);

    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_error != null) {
      return ErrorDisplay.fullScreen(
        error: _error,
        onRetry: () => _load(reset: true),
      );
    }
    if (_items.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              widget.filterThang != null || widget.filterNam != null
                  ? Icons.search_off_rounded
                  : Icons.receipt_long_outlined,
              size: 64,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: AppSpacing.sm2),
            Text(
              widget.filterThang != null || widget.filterNam != null
                  ? 'Không có hóa đơn nào\ntrong khoảng thời gian này'
                  : 'Không có hóa đơn nào',
              textAlign: TextAlign.center,
              style: AppTypography.body.secondary,
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: () => _load(reset: true),
      child: ListView.separated(
        controller: _scrollController,
        padding: AppSpacing.insetAll16,
        itemCount: _items.length + (_loadingMore ? 1 : 0),
        separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.sm2),
        itemBuilder: (context, i) {
          if (i == _items.length) {
            return const Center(
              child: Padding(
                padding: AppSpacing.insetAll16,
                child: CircularProgressIndicator(),
              ),
            );
          }
          return _HoaDonCard(
            hoaDon: _items[i],
            onTap: () => context
                .push<void>(
                  '/dich-vu/hoa-don/detail',
                  extra: {
                    'hoaDonId': _items[i].id,
                    'maHoaDon': _items[i].maHoaDon,
                  },
                )
                .then((_) => _load(reset: true)),
          );
        },
      ),
    );
  }
}

class _HoaDonCard extends StatelessWidget {
  final HoaDon hoaDon;
  final VoidCallback onTap;

  const _HoaDonCard({required this.hoaDon, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final cfg = getTrangThaiConfig(hoaDon.trangThaiHoaDonId);

    return AppCard(
      onTap: onTap,
      padding: EdgeInsets.zero,
      child: Column(
        children: [
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppSpacing.md,
              vertical: 14,
            ),
            decoration: BoxDecoration(
              color: hoaDon.laQuaHan
                  ? AppColors.errorLight
                  : AppColors.primaryLight.withAlpha(80),
              borderRadius: const BorderRadius.vertical(
                top: Radius.circular(AppRadius.standard),
              ),
            ),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(AppSpacing.sm),
                  decoration: BoxDecoration(
                    color: cfg.mauNen,
                    borderRadius: AppRadius.buttonSmall,
                  ),
                  child: Icon(cfg.icon, color: cfg.mau, size: 18),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(hoaDon.maHoaDon, style: AppTypography.subhead),
                      Text(
                        hoaDon.kyThanhToan,
                        style: AppTypography.captionSmall.secondary,
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 4,
                  ),
                  decoration: BoxDecoration(
                    color: cfg.mauNen,
                    borderRadius: AppRadius.badge,
                  ),
                  child: Text(
                    cfg.ten,
                    style: AppTypography.captionSmall.copyWith(
                      color: cfg.mau,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),
          ),

          Padding(
            padding: AppSpacing.insetAll16,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Hạn thanh toán',
                      style: AppTypography.captionSmall.disabled,
                    ),
                    const SizedBox(height: 2),
                    Row(
                      children: [
                        if (hoaDon.sapHetHan && !hoaDon.laDaThanhToan)
                          const Padding(
                            padding: EdgeInsets.only(right: 4),
                            child: Icon(
                              Icons.warning_amber_rounded,
                              color: AppColors.warning,
                              size: 14,
                            ),
                          ),
                        Text(
                          formatNgay(hoaDon.ngayHanThanhToan),
                          style: AppTypography.subhead.copyWith(
                            color: hoaDon.laQuaHan
                                ? AppColors.error
                                : AppColors.textPrimary,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Text(
                      'Tổng tiền',
                      style: AppTypography.captionSmall.disabled,
                    ),
                    const SizedBox(height: 2),
                    Text(
                      formatTien(hoaDon.tongTien),
                      style: AppTypography.headline.primary,
                    ),
                  ],
                ),
              ],
            ),
          ),

          if (hoaDon.laCoTheThanhToan)
            Container(
              width: double.infinity,
              padding: const EdgeInsets.symmetric(vertical: 10),
              decoration: const BoxDecoration(
                color: AppColors.primary,
                borderRadius: BorderRadius.vertical(
                  bottom: Radius.circular(AppRadius.standard),
                ),
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(
                    Icons.qr_code_rounded,
                    color: AppColors.textOnPrimary,
                    size: 16,
                  ),
                  const SizedBox(width: 6),
                  Text(
                    'Xem & Thanh toán',
                    style: AppTypography.subhead.onPrimary,
                  ),
                ],
              ),
            ),
        ],
      ),
    );
  }
}

class _FilterChip extends StatelessWidget {
  final String label;
  final bool selected;
  final VoidCallback onTap;
  final double? width;

  const _FilterChip({
    required this.label,
    required this.selected,
    required this.onTap,
    this.width,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: width,
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
        decoration: BoxDecoration(
          color: selected ? AppColors.primary : AppColors.inputFill,
          borderRadius: AppRadius.buttonSmall,
        ),
        child: Text(
          label,
          textAlign: TextAlign.center,
          style: AppTypography.caption.copyWith(
            color: selected ? AppColors.textOnPrimary : AppColors.textSecondary,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
    );
  }
}
