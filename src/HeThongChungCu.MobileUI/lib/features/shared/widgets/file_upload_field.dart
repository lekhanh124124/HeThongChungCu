import 'dart:io';
import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';
import 'package:image_picker/image_picker.dart';
import 'package:photo_view/photo_view.dart';
import 'package:photo_view/photo_view_gallery.dart';
import 'package:url_launcher/url_launcher.dart';

import 'package:klks_app/design/design.dart';

import '../models/file_model.dart';

typedef UploadFn =
    Future<List<UploadedFile>> Function({
      required List<File> files,
      required String targetContainer,
    });

class AppFileUploadField extends StatefulWidget {
  final String label;
  final String targetContainer;
  final UploadFn uploadFn;
  final List<UploadedFile> initialFiles;
  final int? maxFiles;
  final bool allowMultiple;
  final void Function(List<UploadedFile> files) onChanged;
  final bool isRequired;
  final bool enabled;

  const AppFileUploadField({
    super.key,
    required this.label,
    required this.targetContainer,
    required this.uploadFn,
    required this.onChanged,
    this.initialFiles = const [],
    this.maxFiles,
    this.allowMultiple = true,
    this.isRequired = false,
    this.enabled = true,
  });

  @override
  State<AppFileUploadField> createState() => _AppFileUploadFieldState();
}

class _AppFileUploadFieldState extends State<AppFileUploadField> {
  final List<UploadedFile> _uploaded = [];
  final Set<String> _pending = {};

  @override
  void initState() {
    super.initState();
    _uploaded.addAll(widget.initialFiles);
  }

  bool get _canAddMore {
    if (!widget.enabled) return false;
    if (widget.maxFiles == null) return true;
    return (_uploaded.length + _pending.length) < widget.maxFiles!;
  }

  Future<void> _pickFiles() async {
    final source = await _showSourcePicker();
    if (source == null || !mounted) return;

    List<File> files = [];
    switch (source) {
      case _Source.gallery:
        final images = await ImagePicker().pickMultiImage();
        files = images.map((e) => File(e.path)).toList();
      case _Source.camera:
        final image = await ImagePicker().pickImage(source: ImageSource.camera);
        if (image != null) files = [File(image.path)];
      case _Source.file:
        final result = await FilePicker.pickFiles(
          allowMultiple: widget.allowMultiple,
          type: FileType.any,
        );
        if (result != null) {
          files = result.paths.whereType<String>().map(File.new).toList();
        }
    }

    if (files.isEmpty || !mounted) return;

    if (widget.maxFiles != null) {
      final remaining = widget.maxFiles! - _uploaded.length - _pending.length;
      if (remaining <= 0) return;
      files = files.take(remaining.clamp(0, files.length)).toList();
    }

    await _uploadFiles(files);
  }

  Future<void> _uploadFiles(List<File> files) async {
    final fileNames = files.map((f) => f.path.split('/').last).toList();

    setState(() {
      _pending.addAll(fileNames);
    });

    try {
      final results = await widget.uploadFn(
        files: files,
        targetContainer: widget.targetContainer,
      );
      if (!mounted) return;
      setState(() {
        for (final name in fileNames) {
          _pending.remove(name);
        }
        _uploaded.addAll(results);
      });
      widget.onChanged(List.unmodifiable(_uploaded));
    } catch (e) {
      if (!mounted) return;
      setState(() {
        for (final name in fileNames) {
          _pending.remove(name);
        }
      });
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('Upload thất bại: $e')));
    }
  }

  void _viewFile(UploadedFile file) {
    if (file.isImage) {
      final images = _uploaded.where((f) => f.isImage).toList();
      final initialIndex = images.indexOf(file);
      Navigator.push(
        context,
        MaterialPageRoute<void>(
          fullscreenDialog: true,
          builder: (_) => _PhotoGalleryScreen(
            images: images,
            initialIndex: initialIndex < 0 ? 0 : initialIndex,
          ),
        ),
      );
    } else {
      _launchUrl(file.fileUrl);
    }
  }

  Future<void> _launchUrl(String url) async {
    final uri = Uri.tryParse(url);
    if (uri == null) return;
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri, mode: LaunchMode.externalApplication);
    } else if (mounted) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Không thể mở file')));
    }
  }

  void _removeFile(UploadedFile file) {
    setState(() => _uploaded.remove(file));
    widget.onChanged(List.unmodifiable(_uploaded));
  }

  Future<_Source?> _showSourcePicker() => showModalBottomSheet<_Source>(
    context: context,
    backgroundColor: AppColors.surface,
    shape: const RoundedRectangleBorder(borderRadius: AppRadius.modal),
    builder: (_) => const _SourceSheet(),
  );

  @override
  Widget build(BuildContext context) {
    final hasContent = _uploaded.isNotEmpty || _pending.isNotEmpty;

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
              if (widget.maxFiles != null)
                TextSpan(
                  text: '  (tối đa ${widget.maxFiles})',
                  style: const TextStyle(
                    color: AppColors.textDisabled,
                    fontWeight: FontWeight.w400,
                  ),
                ),
            ],
          ),
        ),
        const SizedBox(height: AppSpacing.sm),

        if (hasContent) ...[
          Wrap(
            spacing: AppSpacing.sm,
            runSpacing: AppSpacing.sm,
            children: [
              ..._uploaded.map(
                (f) => _FileChip(
                  file: f,
                  onTap: () => _viewFile(f),
                  onDelete: widget.enabled ? () => _removeFile(f) : null,
                ),
              ),
              ..._pending.map((name) => _PendingChip(fileName: name)),
            ],
          ),
          const SizedBox(height: AppSpacing.sm),
        ],

        if (_canAddMore)
          _AddFileButton(
            label: hasContent ? 'Thêm file' : 'Chọn file',
            onTap: _pickFiles,
          ),

        if (!_canAddMore && widget.maxFiles != null)
          Text(
            'Đã đạt giới hạn ${widget.maxFiles} file',
            style: AppTypography.captionSmall.secondary,
          ),
      ],
    );
  }
}

class _PhotoGalleryScreen extends StatefulWidget {
  final List<UploadedFile> images;
  final int initialIndex;

  const _PhotoGalleryScreen({required this.images, required this.initialIndex});

  @override
  State<_PhotoGalleryScreen> createState() => _PhotoGalleryScreenState();
}

class _PhotoGalleryScreenState extends State<_PhotoGalleryScreen> {
  late int _currentIndex;
  late PageController _pageController;

  @override
  void initState() {
    super.initState();
    _currentIndex = widget.initialIndex;
    _pageController = PageController(initialPage: widget.initialIndex);
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final current = widget.images[_currentIndex];

    return Scaffold(
      backgroundColor: Colors.black,
      extendBodyBehindAppBar: true,
      appBar: AppBar(
        backgroundColor: Colors.black54,
        foregroundColor: Colors.white,
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              current.fileName,
              style: const TextStyle(fontSize: 14, color: Colors.white),
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
            if (widget.images.length > 1)
              Text(
                '${_currentIndex + 1} / ${widget.images.length}',
                style: const TextStyle(fontSize: 11, color: Colors.white70),
              ),
          ],
        ),
      ),
      body: PhotoViewGallery.builder(
        pageController: _pageController,
        itemCount: widget.images.length,
        onPageChanged: (i) => setState(() => _currentIndex = i),
        scrollPhysics: const BouncingScrollPhysics(),
        backgroundDecoration: const BoxDecoration(color: Colors.black),
        builder: (_, i) {
          final img = widget.images[i];
          return PhotoViewGalleryPageOptions(
            imageProvider: NetworkImage(img.fileUrl),
            minScale: PhotoViewComputedScale.contained,
            maxScale: PhotoViewComputedScale.covered * 3,
            errorBuilder: (_, _, _) => const Center(
              child: Icon(Icons.broken_image, color: Colors.white54, size: 64),
            ),
          );
        },
        loadingBuilder: (_, _) => const Center(
          child: CircularProgressIndicator(color: Colors.white54),
        ),
      ),
    );
  }
}

class _FileChip extends StatelessWidget {
  final UploadedFile file;
  final VoidCallback onTap;
  final VoidCallback? onDelete;

  const _FileChip({required this.file, required this.onTap, this.onDelete});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        constraints: const BoxConstraints(maxWidth: 120),
        decoration: BoxDecoration(
          border: Border.all(color: AppColors.border),
          borderRadius: AppRadius.buttonSmall,
          color: AppColors.inputFill,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ClipRRect(
              borderRadius: const BorderRadius.vertical(
                top: Radius.circular(AppRadius.sm - 1),
              ),
              child: file.isImage
                  ? Image.network(
                      file.fileUrl,
                      height: 80,
                      width: double.infinity,
                      fit: BoxFit.cover,
                      errorBuilder: (_, _, _) => _FileIconBox(file: file),
                    )
                  : _FileIconBox(file: file),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 5),
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      file.fileName,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: AppTypography.captionSmall,
                    ),
                  ),
                  if (onDelete != null)
                    GestureDetector(
                      onTap: onDelete,
                      child: const Icon(
                        Icons.close,
                        size: 14,
                        color: AppColors.error,
                      ),
                    ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _FileIconBox extends StatelessWidget {
  final UploadedFile file;
  const _FileIconBox({required this.file});

  @override
  Widget build(BuildContext context) {
    final icon = file.isPdf
        ? Icons.picture_as_pdf_outlined
        : file.contentType.startsWith('video/')
        ? Icons.videocam_outlined
        : Icons.insert_drive_file_outlined;

    return Container(
      height: 80,
      width: double.infinity,
      color: AppColors.secondaryLight,
      child: Icon(icon, size: 32, color: AppColors.textSecondary),
    );
  }
}

class _PendingChip extends StatelessWidget {
  final String fileName;

  const _PendingChip({required this.fileName});

  @override
  Widget build(BuildContext context) {
    return Container(
      constraints: const BoxConstraints(maxWidth: 120),
      decoration: BoxDecoration(
        border: Border.all(color: AppColors.border),
        borderRadius: AppRadius.buttonSmall,
        color: AppColors.inputFill,
      ),
      padding: AppSpacing.insetAll8,
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const SizedBox(
            height: 24,
            width: 24,
            child: CircularProgressIndicator(
              strokeWidth: 2,
              color: AppColors.primary,
            ),
          ),
          const SizedBox(height: 6),
          Text(
            fileName,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: AppTypography.captionSmall.secondary,
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}

class _AddFileButton extends StatelessWidget {
  final String label;
  final VoidCallback onTap;
  const _AddFileButton({required this.label, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: AppRadius.inputField,
      child: CustomPaint(
        painter: _DottedBorderPainter(color: AppColors.border),
        child: SizedBox(
          width: double.infinity,
          height: 48,
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.add, size: 18, color: AppColors.primary),
              const SizedBox(width: 6),
              Text(label, style: AppTypography.buttonLabel.primary),
            ],
          ),
        ),
      ),
    );
  }
}

class _SourceSheet extends StatelessWidget {
  const _SourceSheet();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.fromLTRB(
        AppSpacing.md,
        AppSpacing.xs,
        AppSpacing.md,
        AppSpacing.lg + MediaQuery.paddingOf(context).bottom,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 40,
            height: 4,
            margin: const EdgeInsets.only(bottom: AppSpacing.md),
            decoration: BoxDecoration(
              color: AppColors.border,
              borderRadius: AppRadius.badge,
            ),
          ),
          Text('Chọn file từ', style: AppTypography.headline),
          const SizedBox(height: AppSpacing.sm),
          ListTile(
            leading: const Icon(
              Icons.photo_library_outlined,
              color: AppColors.primary,
            ),
            title: Text('Thư viện ảnh', style: AppTypography.bodyMedium),
            onTap: () => Navigator.pop(context, _Source.gallery),
            shape: RoundedRectangleBorder(borderRadius: AppRadius.buttonSmall),
          ),
          ListTile(
            leading: const Icon(
              Icons.camera_alt_outlined,
              color: AppColors.primary,
            ),
            title: Text('Chụp ảnh', style: AppTypography.bodyMedium),
            onTap: () => Navigator.pop(context, _Source.camera),
            shape: RoundedRectangleBorder(borderRadius: AppRadius.buttonSmall),
          ),
          ListTile(
            leading: const Icon(
              Icons.folder_outlined,
              color: AppColors.primary,
            ),
            title: Text('File manager', style: AppTypography.bodyMedium),
            onTap: () => Navigator.pop(context, _Source.file),
            shape: RoundedRectangleBorder(borderRadius: AppRadius.buttonSmall),
          ),
        ],
      ),
    );
  }
}

class _DottedBorderPainter extends CustomPainter {
  final Color color;
  static const _radius = Radius.circular(AppRadius.input);
  const _DottedBorderPainter({required this.color});

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = color
      ..strokeWidth = 1.5
      ..style = PaintingStyle.stroke;
    final path = Path()
      ..addRRect(
        RRect.fromRectAndRadius(
          Rect.fromLTWH(0, 0, size.width, size.height),
          _radius,
        ),
      );
    const dashLen = 6.0;
    const gapLen = 4.0;
    for (final metric in path.computeMetrics()) {
      var dist = 0.0;
      while (dist < metric.length) {
        final end = (dist + dashLen).clamp(0.0, metric.length);
        canvas.drawPath(metric.extractPath(dist, end), paint);
        dist += dashLen + gapLen;
      }
    }
  }

  @override
  bool shouldRepaint(_DottedBorderPainter old) => old.color != color;
}

enum _Source { gallery, camera, file }
