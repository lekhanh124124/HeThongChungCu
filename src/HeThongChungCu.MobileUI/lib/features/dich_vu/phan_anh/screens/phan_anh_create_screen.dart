
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';

import '../models/phan_anh_model.dart';
import '../services/phan_anh_service.dart';

const _kLoaiPhanAnh = <(int, String)>[
  (1, 'Vệ sinh & Môi trường'),
  (2, 'An ninh & Bảo vệ'),
  (3, 'Hạ tầng & Kỹ thuật'),
  (4, 'Thái độ phục vụ'),
  (5, 'Tài chính & Phí dịch vụ'),
  (6, 'Khác'),
];

class PhanAnhCreateScreen extends StatefulWidget {
  final PhanAnhDetailResponse? existing;

  const PhanAnhCreateScreen({super.key, this.existing});

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra as Map<String, dynamic>?;
    return PhanAnhCreateScreen(existing: e?['existing']);
  }

  @override
  State<PhanAnhCreateScreen> createState() => _PhanAnhCreateScreenState();
}

class _PhanAnhCreateScreenState extends State<PhanAnhCreateScreen> {
  final _service = PhanAnhService.instance;
  final _formKey = GlobalKey<FormState>();
  final _picker = ImagePicker();

  late final TextEditingController _tieuDeCtrl;
  late final TextEditingController _noiDungCtrl;

  List<QuanHeCuTruModel> _canHoList = [];
  bool _isLoadingCanHo = true;
  String? _loadCanHoError;
  QuanHeCuTruModel? _selectedCanHo;

  late int _selectedLoaiId;

  final List<UploadedFile> _uploadedFiles = [];
  List<int> existingTepIds = [];

  final List<XFile> _selectedImages = [];
  final List<int> _uploadedImageIds = [];
  bool _isUploading = false;

  bool _isSubmitting = false;

  bool get _isEdit => widget.existing != null;

  @override
  void initState() {
    super.initState();
    _loadCanHoList();
    final e = widget.existing;

    _tieuDeCtrl = TextEditingController(text: e?.tieuDe ?? '');
    _noiDungCtrl = TextEditingController(text: e?.noiDung ?? '');

    final existingLoai = e?.loaiPhanAnhId;
    _selectedLoaiId = _kLoaiPhanAnh.any((l) => l.$1 == existingLoai)
        ? existingLoai!
        : _kLoaiPhanAnh.first.$1;
  }

  @override
  void dispose() {
    _tieuDeCtrl.dispose();
    _noiDungCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickImages() async {
    final picked = await _picker.pickMultiImage(imageQuality: 80);
    if (picked.isEmpty) return;
    if (!mounted) return;

    final remaining = 5 - _selectedImages.length;
    if (remaining <= 0) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Tối đa 5 ảnh')));
      return;
    }

    final toAdd = picked.take(remaining).toList();
    setState(() => _selectedImages.addAll(toAdd));
    await _uploadImages(toAdd);
  }

  Future<void> _uploadImages(List<XFile> images) async {
    setState(() => _isUploading = true);
    try {
      final files = images.map((x) => File(x.path)).toList();
      final uploaded = await _service.uploadMedia(files: files, targetContainer: 'yeu-cau-phan-anh');
      setState(() {
        _uploadedImageIds.addAll(uploaded.map((u) => u.fileId));
      });
    } on Exception catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('Upload ảnh lỗi: $e')));
    } finally {
      if (mounted) setState(() => _isUploading = false);
    }
  }

  void _removeImage(int index) {
    setState(() {
      _selectedImages.removeAt(index);
      if (index < _uploadedImageIds.length) {
        _uploadedImageIds.removeAt(index);
      }
    });
  }

  Future<void> _loadCanHoList() async {
    setState(() {
      _isLoadingCanHo = true;
      _loadCanHoError = null;
    });
    try {
      final list = await _service.getCanHoList();
      if (!mounted) return;
      setState(() {
        _canHoList = list;
        if (list.length == 1) _selectedCanHo = list.first;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _loadCanHoError = e.toString());
    } finally {
      if (mounted) setState(() => _isLoadingCanHo = false);
    }
  }

  Future<void> _submit({required bool asDraft}) async {
    if (!(_formKey.currentState?.validate() ?? false)) return;

    setState(() => _isSubmitting = true);
    try {
      PhanAnhResponse result;
      final allTepIds = [
        ...existingTepIds,
        ..._uploadedFiles.map((e) => e.fileId),
        ..._uploadedImageIds,
      ];
      if (_isEdit) {
        result = await _service.update(
          id: widget.existing!.id,
          tieuDe: _tieuDeCtrl.text.trim(),
          noiDung: _noiDungCtrl.text.trim(),
          loaiPhanAnhId: _selectedLoaiId,
          danhSachTepIds: allTepIds,
          isSubmit: !asDraft,
          isWithdraw: false,
        );
      } else {
        result = await _service.create(
          canHoId: _selectedCanHo!.canHoId,
          tieuDe: _tieuDeCtrl.text.trim(),
          noiDung: _noiDungCtrl.text.trim(),
          loaiPhanAnhId: _selectedLoaiId,
          danhSachTepIds: allTepIds,
          isSubmit: !asDraft,
        );
      }

      if (!mounted) return;
      final action = asDraft ? 'Đã lưu nháp' : 'Đã gửi phản ánh';
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('$action (ID: ${result.id})'),
          backgroundColor: Colors.green,
        ),
      );
      Navigator.pop(context, true);
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_isEdit ? 'Chỉnh sửa phản ánh' : 'Tạo phản ánh mới'),
      ),
      resizeToAvoidBottomInset: true,
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            _buildCanHoSelector(),
            const SizedBox(height: 16),

            _SectionLabel('Loại phản ánh *'),
            DropdownButtonFormField<int>(
              initialValue: _selectedLoaiId,
              decoration: _inputDeco(
                hint: 'Chọn loại phản ánh',
                prefixIcon: Icons.category_outlined,
              ),
              items: _kLoaiPhanAnh
                  .map((l) => DropdownMenuItem(value: l.$1, child: Text(l.$2)))
                  .toList(),
              onChanged: (v) {
                if (v != null) setState(() => _selectedLoaiId = v);
              },
              validator: (v) =>
                  v == null ? 'Vui lòng chọn loại phản ánh' : null,
            ),
            const SizedBox(height: 16),

            _SectionLabel('Tiêu đề *'),
            TextFormField(
              controller: _tieuDeCtrl,
              decoration: _inputDeco(
                hint: 'Mô tả ngắn gọn sự cố...',
                prefixIcon: Icons.title,
              ),
              maxLength: 200,
              textCapitalization: TextCapitalization.sentences,
              validator: (v) {
                if (v == null || v.trim().isEmpty) {
                  return 'Tiêu đề không được để trống';
                }
                if (v.trim().length < 5) {
                  return 'Tiêu đề phải có ít nhất 5 ký tự';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),

            _SectionLabel('Nội dung chi tiết *'),
            TextFormField(
              controller: _noiDungCtrl,
              decoration: _inputDeco(
                hint: 'Mô tả chi tiết vị trí, tình trạng hư hỏng...',
                prefixIcon: Icons.notes,
              ),
              minLines: 4,
              maxLines: 10,
              maxLength: 2000,
              textCapitalization: TextCapitalization.sentences,
              validator: (v) {
                if (v == null || v.trim().isEmpty) {
                  return 'Nội dung không được để trống';
                }
                if (v.trim().length < 10) {
                  return 'Nội dung phải có ít nhất 10 ký tự';
                }
                return null;
              },
            ),
            const SizedBox(height: 4),

            _buildImageSection(),

            const SizedBox(height: 32),

            if (_isSubmitting)
              const Center(child: CircularProgressIndicator())
            else
              Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  ElevatedButton.icon(
                    onPressed: () => _submit(asDraft: false),
                    icon: const Icon(Icons.send),
                    label: Text(_isEdit ? 'Lưu và gửi' : 'Gửi phản ánh'),
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 14),
                    ),
                  ),
                  const SizedBox(height: 10),
                  OutlinedButton.icon(
                    onPressed: () => _submit(asDraft: true),
                    icon: const Icon(Icons.save_outlined),
                    label: const Text('Lưu nháp'),
                    style: OutlinedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 14),
                    ),
                  ),
                ],
              ),

            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }

  InputDecoration _inputDeco({required String hint, IconData? prefixIcon}) =>
      InputDecoration(
        hintText: hint,
        prefixIcon: prefixIcon != null ? Icon(prefixIcon, size: 20) : null,
        border: const OutlineInputBorder(),
        isDense: true,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 12,
          vertical: 12,
        ),
      );

  Widget _buildCanHoSelector() {
    if (_isLoadingCanHo) {
      return const InputDecorator(
        decoration: InputDecoration(
          labelText: 'Căn hộ *',
          prefixIcon: Icon(Icons.apartment),
          border: OutlineInputBorder(),
        ),
        child: SizedBox(
          height: 20,
          child: Row(
            children: [
              SizedBox(
                width: 16,
                height: 16,
                child: CircularProgressIndicator(strokeWidth: 2),
              ),
              SizedBox(width: 8),
              Text('Đang tải danh sách căn hộ...'),
            ],
          ),
        ),
      );
    }

    if (_loadCanHoError != null) {
      return InputDecorator(
        decoration: InputDecoration(
          labelText: 'Căn hộ *',
          prefixIcon: const Icon(Icons.apartment),
          border: const OutlineInputBorder(),
          errorText: 'Không tải được danh sách',
          suffixIcon: IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Thử lại',
            onPressed: _loadCanHoList,
          ),
        ),
        child: const SizedBox.shrink(),
      );
    }

    if (_canHoList.isEmpty) {
      return const InputDecorator(
        decoration: InputDecoration(
          labelText: 'Căn hộ *',
          prefixIcon: Icon(Icons.apartment),
          border: OutlineInputBorder(),
          errorText: 'Bạn chưa có căn hộ nào',
        ),
        child: SizedBox.shrink(),
      );
    }

    return DropdownButtonFormField<QuanHeCuTruModel>(
      isExpanded: true,
      initialValue: _selectedCanHo,
      decoration: const InputDecoration(
        labelText: 'Căn hộ *',
        prefixIcon: Icon(Icons.apartment),
        border: OutlineInputBorder(),
      ),
      items: _canHoList
          .map(
            (canHo) => DropdownMenuItem<QuanHeCuTruModel>(
              value: canHo,
              child: Text(
                canHo.diaChiDayDu,
                overflow: TextOverflow.ellipsis,
                maxLines: 1,
              ),
            ),
          )
          .toList(),
      onChanged: (val) => setState(() => _selectedCanHo = val),
      validator: (_) => _selectedCanHo == null ? 'Vui lòng chọn căn hộ' : null,
    );
  }

  Widget _buildImageSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Text(
              'Ảnh hiện trường (${_selectedImages.length}/5)',
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
            if (_isUploading) ...[
              const SizedBox(width: 8),
              const SizedBox(
                width: 14,
                height: 14,
                child: CircularProgressIndicator(strokeWidth: 2),
              ),
              const SizedBox(width: 4),
              Text(
                'Đang upload...',
                style: TextStyle(fontSize: 12, color: Colors.grey.shade500),
              ),
            ],
          ],
        ),
        const SizedBox(height: 8),
        SizedBox(
          height: 100,
          child: ListView(
            scrollDirection: Axis.horizontal,
            children: [
              ..._selectedImages.asMap().entries.map((entry) {
                final i = entry.key;
                final img = entry.value;
                return Stack(
                  children: [
                    Container(
                      margin: const EdgeInsets.only(right: 8),
                      width: 100,
                      height: 100,
                      child: ClipRRect(
                        borderRadius: BorderRadius.circular(8),
                        child: Image.file(File(img.path), fit: BoxFit.cover),
                      ),
                    ),
                    Positioned(
                      top: 2,
                      right: 10,
                      child: GestureDetector(
                        onTap: () => _removeImage(i),
                        child: Container(
                          decoration: const BoxDecoration(
                            color: Colors.black54,
                            shape: BoxShape.circle,
                          ),
                          padding: const EdgeInsets.all(2),
                          child: const Icon(
                            Icons.close,
                            size: 14,
                            color: Colors.white,
                          ),
                        ),
                      ),
                    ),
                    if (i < _uploadedImageIds.length)
                      Positioned(
                        bottom: 4,
                        right: 12,
                        child: Container(
                          decoration: const BoxDecoration(
                            color: Colors.green,
                            shape: BoxShape.circle,
                          ),
                          padding: const EdgeInsets.all(2),
                          child: const Icon(
                            Icons.check,
                            size: 12,
                            color: Colors.white,
                          ),
                        ),
                      ),
                  ],
                );
              }),

              if (_selectedImages.length < 5)
                GestureDetector(
                  onTap: _isUploading ? null : _pickImages,
                  child: Container(
                    width: 100,
                    height: 100,
                    decoration: BoxDecoration(
                      border: Border.all(color: Colors.grey.shade300),
                      borderRadius: BorderRadius.circular(8),
                      color: Colors.grey.shade50,
                    ),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(
                          Icons.add_photo_alternate_outlined,
                          size: 32,
                          color: Colors.grey.shade400,
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Thêm ảnh',
                          style: TextStyle(
                            fontSize: 11,
                            color: Colors.grey.shade500,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
            ],
          ),
        ),
        const SizedBox(height: 4),
        Text(
          'JPG/PNG, tối đa 5MB/ảnh',
          style: TextStyle(fontSize: 11, color: Colors.grey.shade400),
        ),
      ],
    );
  }
}

class _SectionLabel extends StatelessWidget {
  final String text;
  const _SectionLabel(this.text);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Text(
        text,
        style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
      ),
    );
  }
}
