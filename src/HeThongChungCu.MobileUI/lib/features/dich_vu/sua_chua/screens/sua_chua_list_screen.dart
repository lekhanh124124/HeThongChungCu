import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/services/cu_tru_service.dart';
import 'package:klks_app/features/cu_tru/quan_he/widgets/can_ho_selector.dart';

import '../models/sua_chua_model.dart';
import '../services/sua_chua_service.dart';

class SuaChuaListScreen extends StatefulWidget {
  const SuaChuaListScreen({super.key});

  @override
  State<SuaChuaListScreen> createState() => _SuaChuaListScreenState();
}

class _SuaChuaListScreenState extends State<SuaChuaListScreen> {
  final _service = YeuCauSuaChuaService.instance;
  final _cuTruService = CuTruService.instance;

  List<YeuCauSuaChua> _items = [];
  List<SelectorItem> _dsTrangThaiYeuCau = [];
  List<SelectorItem> _dsTrangThaiSuaChua = [];
  List<QuanHeCuTruModel> _dsCanHo = [];

  QuanHeCuTruModel? _selectedCanHo;
  int? _filterTrangThaiYeuCauId;
  int? _filterTrangThaiSuaChuaId;
  final _keywordCtrl = TextEditingController();

  bool _isInitLoading = true;
  bool _isListLoading = false;
  Object? _initError;
  Object? _listError;

  @override
  void initState() {
    super.initState();
    _initData();
  }

  @override
  void dispose() {
    _keywordCtrl.dispose();
    super.dispose();
  }

  Future<void> _initData() async {
    setState(() {
      _isInitLoading = true;
      _initError = null;
    });
    try {
      final results = await Future.wait([
        _cuTruService.getQuanHeCuTruList(),
        _service.getTrangThaiYeuCau(),
        _service.getTrangThaiSuaChua(),
      ]);
      final dsCanHo = results[0] as List<QuanHeCuTruModel>;
      setState(() {
        _dsCanHo = dsCanHo;
        _dsTrangThaiYeuCau = results[1] as List<SelectorItem>;
        _dsTrangThaiSuaChua = results[2] as List<SelectorItem>;
        _selectedCanHo = dsCanHo.isNotEmpty ? dsCanHo.first : null;
        _isInitLoading = false;
      });
      if (_selectedCanHo != null) await _loadList();
    } catch (e) {
      setState(() {
        _initError = e;
        _isInitLoading = false;
      });
    }
  }

  Future<void> _loadList() async {
    if (_selectedCanHo == null) return;
    setState(() {
      _isListLoading = true;
      _listError = null;
    });
    try {
      final result = await _service.getList(
        GetListYeuCauRequest(
          canHoId: _selectedCanHo!.canHoId,
          trangThaiYeuCauId: _filterTrangThaiYeuCauId,
          trangThaiSuaChuaId: _filterTrangThaiSuaChuaId,
        ),
      );
      setState(() => _items = result.items);
    } catch (e) {
      setState(() => _listError = e);
    } finally {
      setState(() => _isListLoading = false);
    }
  }

  void _onCanHoChanged(QuanHeCuTruModel canHo) {
    if (_selectedCanHo?.canHoId == canHo.canHoId) return;
    setState(() {
      _selectedCanHo = canHo;
      _filterTrangThaiYeuCauId = null;
      _filterTrangThaiSuaChuaId = null;
      _keywordCtrl.clear();
      _items = [];
    });
    _loadList();
  }

  bool get _hasActiveFilter =>
      _filterTrangThaiYeuCauId != null ||
      _filterTrangThaiSuaChuaId != null ||
      _keywordCtrl.text.trim().isNotEmpty;

  void _clearFilter() {
    setState(() {
      _filterTrangThaiYeuCauId = null;
      _filterTrangThaiSuaChuaId = null;
      _keywordCtrl.clear();
    });
    _loadList();
  }

  Future<void> _navigateToCreate() async {
    final created = await context.push<bool>(
      '/dich-vu/sua-chua/create',
      extra: {'dsCanHo': _dsCanHo},
    );
    if (created == true) _loadList();
  }

  Future<void> _navigateToDetail(YeuCauSuaChua item) async {
    final changed = await context.push<bool>(
      '/dich-vu/sua-chua/detail/${item.id}',
    );
    if (changed == true) _loadList();
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Yêu cầu sửa chữa',
      actions: [
        IconButton(
          icon: const Icon(Icons.refresh),
          onPressed: _isInitLoading ? null : _loadList,
        ),
      ],
      floatingActionButton:
          _initError == null && !_isInitLoading && _dsCanHo.isNotEmpty
          ? FloatingActionButton.extended(
              onPressed: _navigateToCreate,
              backgroundColor: AppColors.primary,
              foregroundColor: AppColors.textOnPrimary,
              icon: const Icon(Icons.add),
              label: Text('Tạo yêu cầu', style: AppTypography.buttonLabel),
            )
          : null,
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isInitLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_initError != null) {
      return ErrorDisplay.fullScreen(error: _initError, onRetry: _initData);
    }

    if (_dsCanHo.isEmpty) {
      return Center(
        child: Padding(
          padding: AppSpacing.insetAll24,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(
                Icons.home_outlined,
                size: 64,
                color: AppColors.textDisabled,
              ),
              AppSpacing.lg.verticalSpace,
              Text(
                'Bạn chưa được liên kết với căn hộ nào.\nVui lòng liên hệ Ban quản lý.',
                textAlign: TextAlign.center,
                style: AppTypography.body.secondary,
              ),
            ],
          ),
        ),
      );
    }

    return Column(
      children: [
        CanHoSelector(
          dsCanHo: _dsCanHo,
          selected: _selectedCanHo,
          onChanged: _onCanHoChanged,
        ),
        _buildFilterBar(),
        Expanded(child: _buildList()),
      ],
    );
  }

  Widget _buildFilterBar() {
    return Container(
      padding: AppSpacing.insetH16.copyWith(
        top: AppSpacing.sm,
        bottom: AppSpacing.sm,
      ),
      color: AppColors.background,
      child: Column(
        children: [
          AppTextField.search(
            controller: _keywordCtrl,
            hint: 'Tìm loại sự cố, nội dung...',
            onSubmitted: (_) => _loadList(),
          ),
          AppSpacing.sm.verticalSpace,
          Row(
            children: [
              Expanded(
                child: _FilterChipDropdown(
                  hint: 'Trạng thái YC',
                  items: _dsTrangThaiYeuCau,
                  selectedId: _filterTrangThaiYeuCauId,
                  onChanged: (id) {
                    setState(() => _filterTrangThaiYeuCauId = id);
                    _loadList();
                  },
                ),
              ),
              AppSpacing.sm.horizontalSpace,
              Expanded(
                child: _FilterChipDropdown(
                  hint: 'Trạng thái SC',
                  items: _dsTrangThaiSuaChua,
                  selectedId: _filterTrangThaiSuaChuaId,
                  onChanged: (id) {
                    setState(() => _filterTrangThaiSuaChuaId = id);
                    _loadList();
                  },
                ),
              ),
              if (_hasActiveFilter) ...[
                AppSpacing.sm.horizontalSpace,
                GestureDetector(
                  onTap: _clearFilter,
                  child: Container(
                    padding: const EdgeInsets.all(8),
                    decoration: BoxDecoration(
                      color: AppColors.errorLight,
                      borderRadius: AppRadius.buttonSmall,
                    ),
                    child: const Icon(
                      Icons.filter_alt_off,
                      size: 18,
                      color: AppColors.error,
                    ),
                  ),
                ),
              ],
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildList() {
    if (_isListLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_listError != null) {
      return ErrorDisplay.fullScreen(error: _listError, onRetry: _loadList);
    }

    if (_items.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.handyman_outlined,
              size: 64,
              color: AppColors.textDisabled,
            ),
            AppSpacing.sm.verticalSpace,
            Text(
              _hasActiveFilter
                  ? 'Không có yêu cầu nào với bộ lọc này.'
                  : 'Căn hộ này chưa có yêu cầu sửa chữa nào.',
              style: AppTypography.body.secondary,
              textAlign: TextAlign.center,
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadList,
      color: AppColors.primary,
      child: ListView.separated(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.md,
          AppSpacing.sm,
          AppSpacing.md,
          100,
        ),
        itemCount: _items.length,
        separatorBuilder: (_, _) => AppSpacing.sm.verticalSpace,
        itemBuilder: (_, i) => _YeuCauCard(
          item: _items[i],
          onTap: () => _navigateToDetail(_items[i]),
        ),
      ),
    );
  }
}

class _FilterChipDropdown extends StatelessWidget {
  final String hint;
  final List<SelectorItem> items;
  final int? selectedId;
  final ValueChanged<int?> onChanged;

  const _FilterChipDropdown({
    required this.hint,
    required this.items,
    required this.selectedId,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    final selected = items.where((e) => e.id == selectedId).firstOrNull;
    return GestureDetector(
      onTap: () async {
        final result = await showModalBottomSheet<int?>(
          context: context,
          backgroundColor: AppColors.surface,
          shape: const RoundedRectangleBorder(borderRadius: AppRadius.modal),
          builder: (_) =>
              _FilterSheet(title: hint, items: items, selectedId: selectedId),
        );
        if (result != null) onChanged(result == -1 ? null : result);
      },
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
        decoration: BoxDecoration(
          color: selected != null
              ? AppColors.primaryLight
              : AppColors.inputFill,
          borderRadius: AppRadius.inputField,
          border: selected != null
              ? Border.all(color: AppColors.primary.withAlpha(80))
              : null,
        ),
        child: Row(
          children: [
            Expanded(
              child: Text(
                selected?.name ?? hint,
                style: AppTypography.captionSmall.copyWith(
                  color: selected != null
                      ? AppColors.primary
                      : AppColors.textDisabled,
                  fontWeight: selected != null ? FontWeight.w600 : null,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
            Icon(
              Icons.arrow_drop_down,
              size: 18,
              color: selected != null
                  ? AppColors.primary
                  : AppColors.textSecondary,
            ),
          ],
        ),
      ),
    );
  }
}

class _FilterSheet extends StatelessWidget {
  final String title;
  final List<SelectorItem> items;
  final int? selectedId;

  const _FilterSheet({
    required this.title,
    required this.items,
    required this.selectedId,
  });

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          AppSpacing.sm.verticalSpace,
          Container(
            width: 40,
            height: 4,
            decoration: BoxDecoration(
              color: AppColors.border,
              borderRadius: AppRadius.badge,
            ),
          ),
          AppSpacing.sm.verticalSpace,
          Padding(
            padding: AppSpacing.insetH16,
            child: Text(title, style: AppTypography.headline),
          ),
          const Divider(height: 16),
          ListTile(
            title: Text('Tất cả', style: AppTypography.bodyMedium),
            trailing: selectedId == null
                ? const Icon(Icons.check_circle, color: AppColors.primary)
                : null,
            onTap: () => Navigator.pop(context, -1),
          ),
          ...items.map(
            (item) => ListTile(
              title: Text(item.name, style: AppTypography.bodyMedium),
              trailing: selectedId == item.id
                  ? const Icon(Icons.check_circle, color: AppColors.primary)
                  : null,
              selected: selectedId == item.id,
              selectedTileColor: AppColors.primaryLight.withAlpha(80),
              onTap: () => Navigator.pop(context, item.id),
            ),
          ),
          AppSpacing.sm.verticalSpace,
        ],
      ),
    );
  }
}

class _YeuCauCard extends StatelessWidget {
  final YeuCauSuaChua item;
  final VoidCallback onTap;

  const _YeuCauCard({required this.item, required this.onTap});

  AppBadgeVariant get _badgeVariant {
    switch (item.trangThaiYeuCauId) {
      case TrangThaiYeuCau.completed:
        return AppBadgeVariant.success;
      case TrangThaiYeuCau.pending:
      case TrangThaiYeuCau.returned:
        return AppBadgeVariant.warning;
      case TrangThaiYeuCau.saved:
        return AppBadgeVariant.info;
      default:
        return AppBadgeVariant.error;
    }
  }

  String _fmt(DateTime dt) {
    final l = dt.toLocal();
    return '${l.day}/${l.month}/${l.year} '
        '${l.hour.toString().padLeft(2, '0')}:${l.minute.toString().padLeft(2, '0')}';
  }

  @override
  Widget build(BuildContext context) {
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
                  item.loaiSuCoTen ?? 'Sự cố #${item.loaiSuCoId}',
                  style: AppTypography.subhead,
                ),
              ),
              if (item.trangThaiYeuCauTen != null) ...[
                AppSpacing.sm.horizontalSpace,
                AppStatusBadge(
                  label: item.trangThaiYeuCauTen!,
                  variant: _badgeVariant,
                ),
              ],
            ],
          ),
          AppSpacing.xs.verticalSpace,
          Text(
            item.noiDung,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: AppTypography.body.secondary,
          ),
          AppSpacing.sm.verticalSpace,
          _MetaRow(Icons.apartment_outlined, item.diaChiDayDu),
          if (item.trangThaiSuaChuaTen != null) ...[
            AppSpacing.xs.verticalSpace,
            _MetaRow(
              Icons.engineering_outlined,
              item.trangThaiSuaChuaTen!,
              color: AppColors.primary,
            ),
          ],
          if (item.henTu != null) ...[
            AppSpacing.xs.verticalSpace,
            _MetaRow(
              Icons.calendar_today_outlined,
              'Hẹn: ${_fmt(item.henTu!)}',
              color: AppColors.success,
            ),
          ],
          if (item.createdAt != null) ...[
            AppSpacing.xs.verticalSpace,
            _MetaRow(
              Icons.access_time_outlined,
              'Gửi: ${_fmt(item.createdAt!)}',
            ),
          ],
        ],
      ),
    );
  }
}

class _MetaRow extends StatelessWidget {
  final IconData icon;
  final String text;
  final Color? color;
  const _MetaRow(this.icon, this.text, {this.color});

  @override
  Widget build(BuildContext context) {
    final c = color ?? AppColors.textSecondary;
    return Row(
      children: [
        Icon(icon, size: 13, color: c),
        AppSpacing.xs.horizontalSpace,
        Expanded(
          child: Text(text, style: AppTypography.captionSmall.withColor(c)),
        ),
      ],
    );
  }
}
