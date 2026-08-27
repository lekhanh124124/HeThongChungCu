import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/widgets/can_ho_selector.dart';

import '../models/phan_anh_model.dart';
import '../services/phan_anh_service.dart';

const _kStatuses = <(int?, String)>[
  (null, 'Tất cả'),
  (1, 'Chờ tiếp nhận'),
  (2, 'Đang xử lý'),
  (3, 'BQL đã phản hồi'),
  (4, 'Cư dân đã phản hồi'),
  (5, 'Chờ đánh giá'),
  (6, 'Đã hoàn thành'),
  (7, 'Đã hủy'),
  (8, 'Nháp'),
  (9, 'Đã thu hồi'),
];

AppBadgeVariant _badgeVariant(int id) => switch (id) {
  2 || 3 || 4 => AppBadgeVariant.info,
  5 || 6 => AppBadgeVariant.success,
  7 || 9 => AppBadgeVariant.error,
  _ => AppBadgeVariant.warning,
};

Color _statusDotColor(int id) => switch (id) {
  1 => const Color(0xFFE2E8F0),
  2 => const Color(0xFF3182CE),
  3 => const Color(0xFFED8936),
  4 => const Color(0xFFECC94B),
  5 => const Color(0xFF805AD5),
  6 => const Color(0xFF38A169),
  7 => const Color(0xFFE53E3E),
  8 => const Color(0xFFCBD5E0),
  9 => const Color(0xFFA0AEC0),
  _ => AppColors.textDisabled,
};

class PhanAnhListScreen extends StatefulWidget {
  const PhanAnhListScreen({super.key});

  @override
  State<PhanAnhListScreen> createState() => _PhanAnhListScreenState();
}

class _PhanAnhListScreenState extends State<PhanAnhListScreen> {
  final _service = PhanAnhService.instance;
  final _searchCtrl = TextEditingController();
  final _scrollCtrl = ScrollController();

  List<QuanHeCuTruModel> _dsCanHo = [];
  QuanHeCuTruModel? _selectedCanHo;
  bool _isLoadingCanHo = true;

  List<PhanAnhResponse> _items = [];
  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _errorMsg;
  int _page = 1;
  bool _hasMore = true;
  int? _filterStatus;

  static const _pageSize = 15;

  @override
  void initState() {
    super.initState();
    _searchCtrl.addListener(() => setState(() {}));
    _scrollCtrl.addListener(_onScroll);
    _loadCanHo();
  }

  @override
  void dispose() {
    _searchCtrl.dispose();
    _scrollCtrl.dispose();
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
    _load(reset: true);
  }

  void _onCanHoChanged(QuanHeCuTruModel canHo) {
    setState(() => _selectedCanHo = canHo);
    _load(reset: true);
  }

  void _onScroll() {
    if (_scrollCtrl.position.pixels >=
            _scrollCtrl.position.maxScrollExtent - 200 &&
        !_isLoadingMore &&
        _hasMore) {
      _load();
    }
  }

  Future<void> _load({bool reset = false}) async {
    if (reset) {
      setState(() {
        _page = 1;
        _hasMore = true;
        _items = [];
        _isLoading = true;
        _errorMsg = null;
      });
    } else {
      setState(() => _isLoadingMore = true);
    }

    try {
      final keyword = _searchCtrl.text.trim();
      final result = await _service.getList(
        keyword: keyword.isEmpty ? null : keyword,
        trangThaiPhanAnhId: _filterStatus,
        canHoId: _selectedCanHo?.canHoId,
        pageNumber: _page,
        pageSize: _pageSize,
      );
      if (!mounted) return;
      setState(() {
        _items.addAll(result.items);
        _hasMore = _items.length < result.pagingInfo.totalItems;
        _page++;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _errorMsg = e.toString());
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
          _isLoadingMore = false;
        });
      }
    }
  }

  void _showFilterSheet() {
    int? localSelected = _filterStatus;

    showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (sheetCtx) {
        return StatefulBuilder(
          builder: (ctx, setSheetState) {
            void apply(int? value) {
              setSheetState(() => localSelected = value);
              setState(() => _filterStatus = value);
              Navigator.pop(sheetCtx);
              _load(reset: true);
            }

            return Container(
              decoration: const BoxDecoration(
                color: AppColors.surface,
                borderRadius: AppRadius.modal,
              ),
              padding: EdgeInsets.only(
                bottom: MediaQuery.of(ctx).viewInsets.bottom,
              ),
              child: SafeArea(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Container(
                      margin: const EdgeInsets.symmetric(vertical: 10),
                      width: 40,
                      height: 4,
                      decoration: BoxDecoration(
                        color: AppColors.border,
                        borderRadius: AppRadius.badge,
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.fromLTRB(16, 0, 16, 12),
                      child: Align(
                        alignment: Alignment.centerLeft,
                        child: Text(
                          'Lọc theo trạng thái',
                          style: AppTypography.headline,
                        ),
                      ),
                    ),
                    const Divider(height: 1),
                    ListView.builder(
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemCount: _kStatuses.length,
                      itemBuilder: (_, i) {
                        final (value, label) = _kStatuses[i];
                        final isSelected = localSelected == value;
                        return InkWell(
                          onTap: () => apply(value),
                          child: Padding(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 16,
                              vertical: 12,
                            ),
                            child: Row(
                              children: [
                                if (value != null)
                                  Container(
                                    width: 10,
                                    height: 10,
                                    margin: const EdgeInsets.only(right: 10),
                                    decoration: BoxDecoration(
                                      color: _statusDotColor(value),
                                      shape: BoxShape.circle,
                                      border: Border.all(
                                        color: AppColors.border,
                                        width: 0.5,
                                      ),
                                    ),
                                  )
                                else
                                  const SizedBox(width: 20),
                                Expanded(
                                  child: Text(
                                    label,
                                    style: AppTypography.body.copyWith(
                                      color: isSelected
                                          ? AppColors.primary
                                          : AppColors.textPrimary,
                                      fontWeight: isSelected
                                          ? FontWeight.w600
                                          : FontWeight.w400,
                                    ),
                                  ),
                                ),
                                if (isSelected)
                                  const Icon(
                                    Icons.check,
                                    color: AppColors.primary,
                                    size: 18,
                                  ),
                              ],
                            ),
                          ),
                        );
                      },
                    ),
                    const SizedBox(height: 8),
                  ],
                ),
              ),
            );
          },
        );
      },
    );
  }

  String? get _activeStatusLabel => _filterStatus != null
      ? _kStatuses
            .firstWhere(
              (s) => s.$1 == _filterStatus,
              orElse: () => (_filterStatus, 'ID $_filterStatus'),
            )
            .$2
      : null;

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      showAppBar: true,
      appBar: AppTopBar(
        title: 'Phản ánh khiếu nại',
        actions: [
          IconButton(
            icon: Badge(
              isLabelVisible: _filterStatus != null,
              backgroundColor: AppColors.primary,
              child: const Icon(Icons.filter_list),
            ),
            tooltip: 'Lọc trạng thái',
            onPressed: _showFilterSheet,
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () async {
          final created = await context.push<bool>('/dich-vu/phan-anh/create');
          if (created == true && mounted) _load(reset: true);
        },
        backgroundColor: AppColors.primary,
        icon: const Icon(Icons.add, color: AppColors.textOnPrimary),
        label: Text(
          'Tạo mới',
          style: AppTypography.buttonLabel.copyWith(
            color: AppColors.textOnPrimary,
          ),
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
              hint: 'Tìm kiếm tiêu đề phản ánh...',
              controller: _searchCtrl,
              onSubmitted: (_) => _load(reset: true),
            ),
          ),

          if (_activeStatusLabel != null)
            Align(
              alignment: Alignment.centerLeft,
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 4, 16, 0),
                child: Chip(
                  avatar: _filterStatus != null
                      ? Container(
                          width: 10,
                          height: 10,
                          decoration: BoxDecoration(
                            color: _statusDotColor(_filterStatus!),
                            shape: BoxShape.circle,
                          ),
                        )
                      : null,
                  label: Text(
                    _activeStatusLabel!,
                    style: AppTypography.captionSmall,
                  ),
                  onDeleted: () {
                    setState(() => _filterStatus = null);
                    _load(reset: true);
                  },
                  deleteIconColor: AppColors.textSecondary,
                  backgroundColor: AppColors.secondaryLight,
                  side: BorderSide.none,
                  visualDensity: VisualDensity.compact,
                ),
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
    if (_errorMsg != null) {
      return ErrorDisplay.fullScreen(
        error: _errorMsg!,
        onRetry: () => _load(reset: true),
      );
    }
    if (_items.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.inbox_outlined,
              size: 64,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: 12),
            Text(
              _selectedCanHo != null
                  ? 'Không có phản ánh nào cho ${_selectedCanHo!.tenCanHo}'
                  : 'Không có phản ánh nào.',
              style: AppTypography.body.copyWith(
                color: AppColors.textSecondary,
              ),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: () => _load(reset: true),
      child: ListView.separated(
        controller: _scrollCtrl,
        padding: const EdgeInsets.fromLTRB(16, 12, 16, 100),
        itemCount: _items.length + (_isLoadingMore ? 1 : 0),
        separatorBuilder: (_, _) => const SizedBox(height: 10),
        itemBuilder: (_, i) {
          if (i == _items.length) {
            return const Center(
              child: Padding(
                padding: EdgeInsets.all(16),
                child: CircularProgressIndicator(),
              ),
            );
          }
          final item = _items[i];
          return _PhanAnhCard(
            item: item,
            onTap: () async {
              await context.push<void>('/dich-vu/phan-anh/detail/${item.id}');
              if (mounted) _load(reset: true);
            },
          );
        },
      ),
    );
  }
}

class _PhanAnhCard extends StatelessWidget {
  final PhanAnhResponse item;
  final VoidCallback onTap;

  const _PhanAnhCard({required this.item, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final fmt = DateFormat('dd/MM/yyyy HH:mm');

    return AppCard(
      onTap: onTap,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
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
              AppStatusBadge(
                label: item.trangThaiPhanAnhTen,
                variant: _badgeVariant(item.trangThaiPhanAnhId),
              ),
            ],
          ),
          const SizedBox(height: 8),

          _MetaRow(icon: Icons.home_outlined, text: item.tenCanHo),
          const SizedBox(height: 3),
          _MetaRow(icon: Icons.category_outlined, text: item.loaiPhanAnhTen),
          const SizedBox(height: 3),
          _MetaRow(icon: Icons.person_outline, text: item.tenNguoiGui),
          if (item.tenNguoiXuLy != null) ...[
            const SizedBox(height: 3),
            _MetaRow(
              icon: Icons.engineering_outlined,
              text: item.tenNguoiXuLy!,
            ),
          ],

          const SizedBox(height: 8),
          const Divider(height: 1),
          const SizedBox(height: 8),

          Row(
            children: [
              const Icon(
                Icons.access_time,
                size: 13,
                color: AppColors.textDisabled,
              ),
              const SizedBox(width: 4),
              Text(
                fmt.format(item.createdAt.toLocal()),
                style: AppTypography.captionSmall.copyWith(
                  color: AppColors.textDisabled,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _MetaRow extends StatelessWidget {
  final IconData icon;
  final String text;

  const _MetaRow({required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Icon(icon, size: 14, color: AppColors.textSecondary),
        const SizedBox(width: 6),
        Expanded(
          child: Text(
            text,
            style: AppTypography.captionSmall.copyWith(
              color: AppColors.textSecondary,
            ),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        ),
      ],
    );
  }
}
