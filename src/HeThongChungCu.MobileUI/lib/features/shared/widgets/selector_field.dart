import 'package:flutter/material.dart';

import 'package:klks_app/design/design.dart';

import '../models/selector_item_model.dart';

class SelectorField extends StatefulWidget {
  final String label;
  final String? hint;
  final List<SelectorItem>? items;
  final Future<List<SelectorItem>>? itemsFuture;
  final List<SelectorItem> selectedItems;
  final bool isMultiple;
  final void Function(List<SelectorItem> selected)? onChanged;
  final void Function(SelectorItem? item)? onChangedSingle;
  final bool isRequired;
  final bool enabled;

  const SelectorField({
    super.key,
    required this.label,
    this.hint,
    this.items,
    this.itemsFuture,
    this.selectedItems = const [],
    this.isMultiple = false,
    this.onChanged,
    this.onChangedSingle,
    this.isRequired = false,
    this.enabled = true,
  }) : assert(
         items != null || itemsFuture != null,
         'Phải truyền items hoặc itemsFuture',
       );

  const SelectorField.future({
    super.key,
    required this.label,
    this.hint,
    required Future<List<SelectorItem>> future,
    this.selectedItems = const [],
    this.isMultiple = false,
    this.onChanged,
    this.onChangedSingle,
    this.isRequired = false,
    this.enabled = true,
  }) : items = null,
       itemsFuture = future;

  @override
  State<SelectorField> createState() => _SelectorFieldState();
}

class _SelectorFieldState extends State<SelectorField> {
  List<SelectorItem> _allItems = [];
  List<SelectorItem> _selected = [];
  bool _loading = false;

  @override
  void initState() {
    super.initState();
    _selected = List.of(widget.selectedItems);
    if (widget.items != null) {
      _allItems = widget.items!;
    } else {
      _loadFromFuture();
    }
  }

  @override
  void didUpdateWidget(SelectorField old) {
    super.didUpdateWidget(old);
    if (widget.items != null && widget.items != old.items) {
      setState(() => _allItems = widget.items!);
    }
    if (widget.selectedItems != old.selectedItems) {
      setState(() => _selected = List.of(widget.selectedItems));
    }
  }

  Future<void> _loadFromFuture() async {
    setState(() => _loading = true);
    try {
      final result = await widget.itemsFuture!;
      if (mounted) setState(() => _allItems = result);
    } catch (_) {
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  String get _displayText {
    if (_selected.isEmpty) return '';
    if (widget.isMultiple) return _selected.map((e) => e.name).join(', ');
    return _selected.first.name;
  }

  Future<void> _openPicker() async {
    if (!widget.enabled || _loading) return;

    final result = await showModalBottomSheet<List<SelectorItem>>(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(borderRadius: AppRadius.modal),
      builder: (_) => _SelectorSheet(
        label: widget.label,
        allItems: _allItems,
        selected: _selected,
        isMultiple: widget.isMultiple,
      ),
    );

    if (result == null || !mounted) return;

    setState(() => _selected = result);
    widget.onChanged?.call(_selected);
    if (!widget.isMultiple) {
      widget.onChangedSingle?.call(_selected.isEmpty ? null : _selected.first);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        RichText(
          text: TextSpan(
            style: AppTypography.captionSmall.copyWith(
              color: AppColors.textSecondary,
              fontWeight: FontWeight.w500,
            ),
            children: [
              TextSpan(text: widget.label),
              if (widget.isRequired)
                const TextSpan(
                  text: ' *',
                  style: TextStyle(color: AppColors.error),
                ),
            ],
          ),
        ),
        const SizedBox(height: 6),

        InkWell(
          onTap: (_loading || !widget.enabled) ? null : _openPicker,
          borderRadius: AppRadius.inputField,
          child: InputDecorator(
            decoration: InputDecoration(
              hintText: _loading
                  ? 'Đang tải...'
                  : (widget.hint ?? 'Chọn ${widget.label.toLowerCase()}'),
              hintStyle: AppTypography.input.disabled,
              border: OutlineInputBorder(
                borderRadius: AppRadius.inputField,
                borderSide: BorderSide.none,
              ),
              enabledBorder: OutlineInputBorder(
                borderRadius: AppRadius.inputField,
                borderSide: BorderSide.none,
              ),
              filled: true,
              fillColor: widget.enabled
                  ? AppColors.inputFill
                  : AppColors.secondaryLight,
              contentPadding: AppSpacing.inputPadding,
              suffixIcon: _loading
                  ? const Padding(
                      padding: EdgeInsets.all(12),
                      child: SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          color: AppColors.primary,
                        ),
                      ),
                    )
                  : const Icon(
                      Icons.arrow_drop_down,
                      color: AppColors.textSecondary,
                    ),
              enabled: widget.enabled && !_loading,
            ),
            isEmpty: _selected.isEmpty,
            child: _selected.isEmpty
                ? const SizedBox.shrink()
                : Text(
                    _displayText,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: AppTypography.input.copyWith(
                      color: AppColors.textPrimary,
                    ),
                  ),
          ),
        ),

        if (widget.isMultiple && _selected.isNotEmpty) ...[
          const SizedBox(height: AppSpacing.sm),
          Wrap(
            spacing: 6,
            runSpacing: 4,
            children: _selected.map((item) {
              return Chip(
                label: Text(item.name, style: AppTypography.captionSmall),
                deleteIcon: const Icon(Icons.close, size: 14),
                onDeleted: widget.enabled
                    ? () {
                        setState(() => _selected.remove(item));
                        widget.onChanged?.call(_selected);
                      }
                    : null,
                visualDensity: VisualDensity.compact,
                materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
              );
            }).toList(),
          ),
        ],
      ],
    );
  }
}

class _SelectorSheet extends StatefulWidget {
  final String label;
  final List<SelectorItem> allItems;
  final List<SelectorItem> selected;
  final bool isMultiple;

  const _SelectorSheet({
    required this.label,
    required this.allItems,
    required this.selected,
    required this.isMultiple,
  });

  @override
  State<_SelectorSheet> createState() => _SelectorSheetState();
}

class _SelectorSheetState extends State<_SelectorSheet> {
  late List<SelectorItem> _selected;
  late List<SelectorItem> _filtered;
  final _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _selected = List.of(widget.selected);
    _filtered = List.of(widget.allItems);
    _searchController.addListener(_onSearch);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _onSearch() {
    final q = _searchController.text.toLowerCase();
    setState(() {
      _filtered = widget.allItems
          .where((e) => e.name.toLowerCase().contains(q))
          .toList();
    });
  }

  void _toggle(SelectorItem item) {
    if (widget.isMultiple) {
      setState(() {
        if (_selected.contains(item)) {
          _selected.remove(item);
          Navigator.pop(context, <SelectorItem>[]);
        } else {
          _selected.add(item);
        }
      });
    } else {
      Navigator.pop(context, [item]);
    }
  }

  @override
  Widget build(BuildContext context) {
    final mq = MediaQuery.of(context);

    return SizedBox(
      height: mq.size.height * 0.75,
      child: Column(
        children: [
          const SizedBox(height: AppSpacing.sm),
          Container(
            width: 40,
            height: 4,
            decoration: BoxDecoration(
              color: AppColors.border,
              borderRadius: AppRadius.badge,
            ),
          ),
          const SizedBox(height: AppSpacing.sm),

          Padding(
            padding: AppSpacing.insetH16,
            child: Row(
              children: [
                Expanded(
                  child: Text(
                    'Chọn ${widget.label}',
                    style: AppTypography.headline,
                  ),
                ),
                if (widget.isMultiple)
                  AppButton(
                    label: 'Xác nhận (${_selected.length})',
                    expanded: false,
                    height: 36,
                    onPressed: () => Navigator.pop(context, _selected),
                  ),
              ],
            ),
          ),
          const SizedBox(height: AppSpacing.sm),

          Padding(
            padding: AppSpacing.insetH16,
            child: AppTextField.search(
              controller: _searchController,
              hint: 'Tìm kiếm...',
            ),
          ),
          const SizedBox(height: AppSpacing.xs),

          Expanded(
            child: _filtered.isEmpty
                ? Center(
                    child: Text(
                      'Không có kết quả',
                      style: AppTypography.body.secondary,
                    ),
                  )
                : ListView.builder(
                    itemCount: _filtered.length,
                    itemBuilder: (_, i) {
                      final item = _filtered[i];
                      final isSelected = _selected.contains(item);
                      return ListTile(
                        title: Text(item.name, style: AppTypography.bodyMedium),
                        trailing: isSelected
                            ? Icon(
                                widget.isMultiple
                                    ? Icons.check_box
                                    : Icons.check_circle,
                                color: AppColors.primary,
                              )
                            : widget.isMultiple
                            ? const Icon(
                                Icons.check_box_outline_blank,
                                color: AppColors.textSecondary,
                              )
                            : null,
                        onTap: () => _toggle(item),
                        selected: isSelected,
                        selectedTileColor: AppColors.primaryLight.withAlpha(80),
                      );
                    },
                  ),
          ),

          if (widget.isMultiple && _selected.isNotEmpty)
            SafeArea(
              child: Padding(
                padding: AppSpacing.insetAll8,
                child: TextButton(
                  onPressed: () => setState(() => _selected.clear()),
                  child: Text(
                    'Bỏ chọn tất cả',
                    style: AppTypography.buttonLabel.copyWith(
                      color: AppColors.textSecondary,
                    ),
                  ),
                ),
              ),
            ),

          SizedBox(height: mq.padding.bottom),
        ],
      ),
    );
  }
}
