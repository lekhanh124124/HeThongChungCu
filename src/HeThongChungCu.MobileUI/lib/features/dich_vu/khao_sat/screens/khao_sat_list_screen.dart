import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/widgets/can_ho_selector.dart';

import '../models/khao_sat_model.dart';
import '../services/khao_sat_service.dart';

class KhaoSatListScreen extends StatefulWidget {
  const KhaoSatListScreen({super.key});

  @override
  State<KhaoSatListScreen> createState() => _KhaoSatListScreenState();
}

class _KhaoSatListScreenState extends State<KhaoSatListScreen>
    with SingleTickerProviderStateMixin {
  final _service = KhaoSatService.instance;
  final _searchController = TextEditingController();
  late final TabController _tabController;

  List<QuanHeCuTruModel> _dsCanHo = [];
  QuanHeCuTruModel? _selectedCanHo;
  bool _isLoadingCanHo = true;

  List<KhaoSatResponse> _items = [];
  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _error;
  int _page = 1;
  bool _hasMore = true;

  static const _tabs = [
    (label: 'Tất cả', trangThaiId: null),
    (label: 'Đang diễn ra', trangThaiId: 2),
    (label: 'Đã kết thúc', trangThaiId: 4),
  ];

  final _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: _tabs.length, vsync: this)
      ..addListener(() {
        if (!_tabController.indexIsChanging) _reset();
      });
    _scrollController.addListener(_onScroll);
    _loadCanHo();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _searchController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _loadCanHo() async {
    setState(() => _isLoadingCanHo = true);
    try {
      final list = await _service.getCanHoList();
      if (!mounted) return;
      setState(() {
        _dsCanHo = list;
        _selectedCanHo = list.isNotEmpty ? list.first : null;
        _isLoadingCanHo = false;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _isLoadingCanHo = false);
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    }
    _load();
  }

  void _onCanHoChanged(QuanHeCuTruModel canHo) {
    setState(() => _selectedCanHo = canHo);
    _reset();
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
            _scrollController.position.maxScrollExtent - 200 &&
        !_isLoadingMore &&
        _hasMore) {
      _loadMore();
    }
  }

  Future<void> _reset() async {
    setState(() {
      _items = [];
      _page = 1;
      _hasMore = true;
      _error = null;
    });
    await _load();
  }

  Future<void> _load() async {
    if (_isLoading) return;
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final trangThaiId = _tabs[_tabController.index].trangThaiId;
      final result = await _service.getList(
        trangThaiId: trangThaiId,
        keyword: _searchController.text.trim(),
        pageNumber: 1,
        pageSize: 10,
      );
      if (!mounted) return;
      setState(() {
        _items = result.items;
        _page = 1;
        _hasMore = result.hasMore;
        _isLoading = false;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
        _isLoading = false;
      });
    }
  }

  Future<void> _loadMore() async {
    setState(() => _isLoadingMore = true);
    try {
      final trangThaiId = _tabs[_tabController.index].trangThaiId;
      final result = await _service.getList(
        trangThaiId: trangThaiId,
        keyword: _searchController.text.trim(),
        pageNumber: _page + 1,
        pageSize: 10,
      );
      if (!mounted) return;
      setState(() {
        _items.addAll(result.items);
        _page++;
        _hasMore = result.hasMore;
        _isLoadingMore = false;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _isLoadingMore = false);
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    }
  }

  void _goDetail(KhaoSatResponse item) {
    if (_selectedCanHo == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng chọn căn hộ trước khi xem chi tiết'),
        ),
      );
      return;
    }
    context
        .push(
          '/dich-vu/khao-sat/detail',
          extra: {
            'khaoSatId': item.id,
            'selectedCanHo': _selectedCanHo!,
            'dsCanHo': _dsCanHo,
          },
        )
        .then((_) => _reset());
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      showAppBar: true,
      appBar: AppTopBar(
        title: 'Khảo sát & Bầu cử',
        bottom: TabBar(
          controller: _tabController,
          tabs: _tabs.map((t) => Tab(text: t.label)).toList(),
          labelColor: AppColors.primary,
          unselectedLabelColor: AppColors.textSecondary,
          indicatorColor: AppColors.primary,
          labelStyle: AppTypography.subhead,
        ),
      ),
      body: Column(
        children: [
          if (_isLoadingCanHo)
            const LinearProgressIndicator()
          else if (_dsCanHo.isNotEmpty)
            CanHoSelector(
              dsCanHo: _dsCanHo,
              selected: _selectedCanHo,
              onChanged: _onCanHoChanged,
            ),

          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
            child: AppTextField.search(
              hint: 'Tìm kiếm đợt khảo sát...',
              controller: _searchController,
              onSubmitted: (_) => _reset(),
            ),
          ),

          Expanded(child: _buildBody()),
        ],
      ),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_error != null) {
      return ErrorDisplay.fullScreen(error: _error!, onRetry: _load);
    }
    if (_items.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.ballot_outlined,
              size: 64,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: 12),
            Text(
              'Không có đợt khảo sát nào',
              style: AppTypography.body.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _reset,
      child: ListView.separated(
        controller: _scrollController,
        padding: const EdgeInsets.all(16),
        itemCount: _items.length + (_isLoadingMore ? 1 : 0),
        separatorBuilder: (_, _) => const SizedBox(height: 12),
        itemBuilder: (_, i) {
          if (i == _items.length) {
            return const Center(
              child: Padding(
                padding: EdgeInsets.all(16),
                child: CircularProgressIndicator(),
              ),
            );
          }
          return _KhaoSatCard(
            item: _items[i],
            onTap: () => _goDetail(_items[i]),
          );
        },
      ),
    );
  }
}

class _KhaoSatCard extends StatelessWidget {
  final KhaoSatResponse item;
  final VoidCallback onTap;

  const _KhaoSatCard({required this.item, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final fmt = DateFormat('dd/MM/yyyy');
    final statusColor = _statusColor(item.trangThaiId);
    final badgeVariant = _badgeVariant(item.trangThaiId);

    return AppCard(
      onTap: onTap,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  item.tieuDe,
                  style: AppTypography.subhead.copyWith(
                    color: AppColors.textPrimary,
                  ),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
              const SizedBox(width: 8),
              AppStatusBadge(label: item.trangThaiTen, variant: badgeVariant),
            ],
          ),
          const SizedBox(height: 6),
          Text(
            item.loaiKhaoSatTen,
            style: AppTypography.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: 10),

          Row(
            children: [
              const Icon(
                Icons.calendar_today_outlined,
                size: 14,
                color: AppColors.textSecondary,
              ),
              const SizedBox(width: 4),
              Text(
                '${fmt.format(item.ngayBatDau)} → ${fmt.format(item.ngayKetThuc)}',
                style: AppTypography.captionSmall.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),

          Row(
            children: [
              _InfoChip(
                icon: Icons.people_outline,
                label: 'Tối thiểu ${item.tyleThamGiaToiThieu.toInt()}%',
              ),
              const SizedBox(width: 8),
              if (item.isAnDanh)
                _InfoChip(icon: Icons.shield_outlined, label: 'Ẩn danh'),
              const Spacer(),
              if (item.isVoted)
                Row(
                  children: [
                    const Icon(
                      Icons.check_circle,
                      size: 14,
                      color: AppColors.success,
                    ),
                    const SizedBox(width: 4),
                    Text(
                      'Đã bỏ phiếu',
                      style: AppTypography.captionSmall.copyWith(
                        color: AppColors.success,
                      ),
                    ),
                  ],
                )
              else if (item.canVote)
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 4,
                  ),
                  decoration: BoxDecoration(
                    color: statusColor.withAlpha(20),
                    borderRadius: AppRadius.badge,
                    border: Border.all(color: statusColor.withAlpha(80)),
                  ),
                  child: Text(
                    'Bỏ phiếu ngay',
                    style: AppTypography.captionSmall.copyWith(
                      color: statusColor,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ),
            ],
          ),
        ],
      ),
    );
  }

  Color _statusColor(int id) => switch (id) {
    2 => AppColors.success,
    3 => AppColors.warning,
    4 => AppColors.secondary,
    _ => AppColors.primary,
  };

  AppBadgeVariant _badgeVariant(int id) => switch (id) {
    2 => AppBadgeVariant.success,
    3 => AppBadgeVariant.warning,
    _ => AppBadgeVariant.info,
  };
}

class _InfoChip extends StatelessWidget {
  final IconData icon;
  final String label;
  const _InfoChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 13, color: AppColors.textSecondary),
        const SizedBox(width: 3),
        Text(
          label,
          style: AppTypography.captionSmall.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
      ],
    );
  }
}
