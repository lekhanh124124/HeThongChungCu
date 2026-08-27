import 'dart:io';
import 'package:flutter/material.dart';

import 'package:klks_app/features/shared/widgets/selector_field.dart';
import 'package:klks_app/features/shared/widgets/file_upload_field.dart';

import '../../quan_he/widgets/shared_widget.dart';
import '../models/thanh_vien_model.dart';
import '../services/thanh_vien_service.dart';

import 'package:klks_app/design/design.dart';

typedef UploadFn =
    Future<List<UploadedFile>> Function({
      required List<File> files,
      required String targetContainer,
    });

class TaiLieuCuTruEditor extends StatefulWidget {
  final void Function(List<TaiLieuCuTruRequest>) onChanged;

  final List<TaiLieuCuTruModel>? initialDocuments;

  const TaiLieuCuTruEditor({
    super.key,
    required this.onChanged,
    this.initialDocuments,
  });

  @override
  State<TaiLieuCuTruEditor> createState() => _TaiLieuCuTruEditorState();
}

class _TaiLieuCuTruEditorState extends State<TaiLieuCuTruEditor> {
  final _yeuCauSvc = ThanhVienService.instance;

  late final Future<List<SelectorItem>> _loaiGiayToFuture = _yeuCauSvc
      .getLoaiGiayToSelector();

  final List<_TaiLieuEntry> _entries = [];

  @override
  void initState() {
    super.initState();
    final docs = widget.initialDocuments;
    if (docs != null && docs.isNotEmpty) {
      for (final doc in docs) {
        _entries.add(_TaiLieuEntry.fromServer(doc));
      }
      _resolveLoaiGiayTo();
    }
  }

  Future<void> _resolveLoaiGiayTo() async {
    try {
      final catalog = await _loaiGiayToFuture;
      if (!mounted) return;
      var changed = false;
      for (final entry in _entries) {
        if (entry._pendingLoaiGiayToId != null && entry.loaiGiayTo == null) {
          final match = catalog
              .where((e) => e.id == entry._pendingLoaiGiayToId)
              .firstOrNull;
          if (match != null) {
            entry.loaiGiayTo = match;
            changed = true;
          }
        }
      }
      if (changed) setState(() {});
    } catch (_) {}
  }

  @override
  void dispose() {
    for (final e in _entries) {
      e.dispose();
    }
    super.dispose();
  }

  void _addEntry() {
    setState(() => _entries.add(_TaiLieuEntry()));
    _notify();
  }

  void _removeEntry(int index) {
    _entries[index].dispose();
    setState(() => _entries.removeAt(index));
    _notify();
  }

  void _notify() {
    final result = _entries
        .where((e) => e.activeFileIds.isNotEmpty)
        .map(
          (e) => TaiLieuCuTruRequest(
            taiLieuCuTruId: e.taiLieuCuTruId,
            loaiGiayToId: e.loaiGiayTo?.id,
            soGiayTo: e.soGiayToCtrl.text.trim(),
            ngayPhatHanh: e.ngayPhatHanh,
            fileIds: e.activeFileIds,
          ),
        )
        .toList();
    widget.onChanged(result);
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        ..._entries.asMap().entries.map(
          (kv) => _TaiLieuCard(
            key: ValueKey('tai_lieu_card_${kv.key}'),
            index: kv.key,
            entry: kv.value,
            loaiGiayToFuture: _loaiGiayToFuture,
            uploadFn: _yeuCauSvc.uploadMedia,
            onChanged: _notify,
            onRemove: () => _removeEntry(kv.key),
          ),
        ),
        const SizedBox(height: AppSpacing.sm),
        AppButton(
          label: _entries.isEmpty ? 'Thêm tài liệu' : 'Thêm tài liệu khác',
          variant: AppButtonVariant.outline,
          leadingIcon: Icons.add,
          height: 44,
          onPressed: _addEntry,
        ),
      ],
    );
  }
}

class _TaiLieuCard extends StatefulWidget {
  final int index;
  final _TaiLieuEntry entry;
  final Future<List<SelectorItem>> loaiGiayToFuture;
  final UploadFn uploadFn;
  final VoidCallback onChanged;
  final VoidCallback onRemove;

  const _TaiLieuCard({
    super.key,
    required this.index,
    required this.entry,
    required this.loaiGiayToFuture,
    required this.uploadFn,
    required this.onChanged,
    required this.onRemove,
  });

  @override
  State<_TaiLieuCard> createState() => _TaiLieuCardState();
}

class _TaiLieuCardState extends State<_TaiLieuCard> {
  @override
  void initState() {
    super.initState();
    widget.entry.soGiayToCtrl.addListener(_onTextChanged);
  }

  @override
  void dispose() {
    widget.entry.soGiayToCtrl.removeListener(_onTextChanged);
    super.dispose();
  }

  void _onTextChanged() => widget.onChanged();

  Future<void> _pickNgayPhatHanh() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: widget.entry.ngayPhatHanh ?? DateTime.now(),
      firstDate: DateTime(1900),
      lastDate: DateTime.now(),
    );
    if (picked != null && mounted) {
      setState(() => widget.entry.ngayPhatHanh = picked);
      widget.onChanged();
    }
  }

  @override
  Widget build(BuildContext context) {
    final entry = widget.entry;

    return AppCard(
      margin: const EdgeInsets.only(bottom: AppSpacing.sm),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Text(
                'Tài liệu ${widget.index + 1}',
                style: AppTypography.subhead,
              ),
              const Spacer(),
              IconButton(
                icon: const Icon(Icons.close, size: 18, color: AppColors.error),
                onPressed: widget.onRemove,
                tooltip: 'Xóa tài liệu này',
                visualDensity: VisualDensity.compact,
                padding: EdgeInsets.zero,
              ),
            ],
          ),
          const Divider(height: AppSpacing.lg),

          SelectorField.future(
            label: 'Loại giấy tờ',
            future: widget.loaiGiayToFuture,
            selectedItems: entry.loaiGiayTo != null ? [entry.loaiGiayTo!] : [],
            onChangedSingle: (v) {
              setState(() => entry.loaiGiayTo = v);
              widget.onChanged();
            },
          ),
          const SizedBox(height: AppSpacing.sm),

          Field(
            controller: entry.soGiayToCtrl,
            label: 'Số giấy tờ',
            hint: 'VD: 012345678901',
          ),
          const SizedBox(height: AppSpacing.sm),

          DatePickerField(
            label: 'Ngày phát hành',
            value: entry.ngayPhatHanh,
            onTap: _pickNgayPhatHanh,
          ),
          const SizedBox(height: AppSpacing.sm),

          if (entry.existingFiles.any((f) => !f.deleted)) ...[
            Text('File đã lưu', style: AppTypography.captionSmall.secondary),
            const SizedBox(height: 4),
            ...entry.existingFiles.map((ef) {
              if (ef.deleted) return const SizedBox.shrink();
              return _ExistingFileRow(
                file: ef.file,
                onDelete: () {
                  setState(() => ef.deleted = true);
                  widget.onChanged();
                },
              );
            }),
            const SizedBox(height: AppSpacing.sm),
          ],

          AppFileUploadField(
            label: entry.existingFiles.isEmpty
                ? 'File đính kèm'
                : 'Thêm file mới',
            targetContainer: 'tai-lieu-cu-tru',
            uploadFn: widget.uploadFn,
            initialFiles: entry.newUploadedFiles,
            allowMultiple: true,
            onChanged: (files) {
              entry.newUploadedFiles
                ..clear()
                ..addAll(files);
              widget.onChanged();
            },
          ),
        ],
      ),
    );
  }
}

class _ExistingFileRow extends StatelessWidget {
  final FileAttachment file;
  final VoidCallback onDelete;

  const _ExistingFileRow({required this.file, required this.onDelete});

  @override
  Widget build(BuildContext context) {
    final isImage = file.contentType.startsWith('image/');
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        children: [
          Icon(
            isImage ? Icons.image_outlined : Icons.picture_as_pdf_outlined,
            size: 16,
            color: AppColors.primary,
          ),
          const SizedBox(width: 6),
          Expanded(
            child: Text(
              file.fileName,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: AppTypography.captionSmall,
            ),
          ),
          const SizedBox(width: 6),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
            decoration: BoxDecoration(
              color: AppColors.successLight,
              borderRadius: AppRadius.badge,
            ),
            child: Text(
              'Đã lưu',
              style: AppTypography.captionSmall.copyWith(
                color: AppColors.success,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
          const SizedBox(width: 4),
          InkWell(
            onTap: onDelete,
            borderRadius: BorderRadius.circular(12),
            child: const Padding(
              padding: EdgeInsets.all(4),
              child: Icon(Icons.close, size: 16, color: AppColors.error),
            ),
          ),
        ],
      ),
    );
  }
}

class _ExistingFileEntry {
  final FileAttachment file;
  bool deleted;

  _ExistingFileEntry({required this.file}) : deleted = false;
}

class _TaiLieuEntry {
  final int taiLieuCuTruId;

  final int? _pendingLoaiGiayToId;

  SelectorItem? loaiGiayTo;
  final TextEditingController soGiayToCtrl;
  DateTime? ngayPhatHanh;

  final List<_ExistingFileEntry> existingFiles;

  final List<UploadedFile> newUploadedFiles;

  _TaiLieuEntry({
    this.taiLieuCuTruId = 0,
    int? pendingLoaiGiayToId,
    String soGiayTo = '',
    this.ngayPhatHanh,
    List<_ExistingFileEntry>? existingFiles,
    List<UploadedFile>? newUploadedFiles,
  }) : _pendingLoaiGiayToId = pendingLoaiGiayToId,
       loaiGiayTo = null,
       soGiayToCtrl = TextEditingController(text: soGiayTo),
       existingFiles = existingFiles ?? [],
       newUploadedFiles = newUploadedFiles ?? [];

  factory _TaiLieuEntry.fromServer(TaiLieuCuTruModel doc) => _TaiLieuEntry(
    taiLieuCuTruId: doc.id,
    pendingLoaiGiayToId: doc.loaiGiayToId != 0 ? doc.loaiGiayToId : null,
    soGiayTo: doc.soGiayTo,
    ngayPhatHanh: doc.ngayPhatHanh,
    existingFiles: doc.files.map((f) => _ExistingFileEntry(file: f)).toList(),
  );

  List<int> get activeFileIds => [
    ...existingFiles.where((f) => !f.deleted).map((f) => f.file.id),
    ...newUploadedFiles.map((f) => f.fileId),
  ];

  void dispose() => soGiayToCtrl.dispose();
}
