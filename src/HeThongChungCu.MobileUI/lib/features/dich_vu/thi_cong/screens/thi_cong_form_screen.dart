import 'dart:io';

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';

import 'package:klks_app/design/design.dart';

import '../models/thi_cong_model.dart';
import '../services/thi_cong_service.dart';

class ThiCongFormScreen extends StatefulWidget {
  final List<QuanHeCuTruModel> dsCanHo;
  final YeuCauThiCongDetail? existingDetail;

  const ThiCongFormScreen({
    super.key,
    required this.dsCanHo,
    this.existingDetail,
  });

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;
    return ThiCongFormScreen(
      dsCanHo: e['dsCanHo'] as List<QuanHeCuTruModel>,
      existingDetail: e['editData'] as YeuCauThiCongDetail?,
    );
  }

  @override
  State<ThiCongFormScreen> createState() => _ThiCongFormScreenState();
}

class _ThiCongFormScreenState extends State<ThiCongFormScreen> {
  final _service = YeuCauThiCongService.instance;
  final _formKey = GlobalKey<FormState>();
  final _picker = ImagePicker();

  late final TextEditingController _hangMucCtrl;
  late final TextEditingController _donViCtrl;
  late final TextEditingController _nguoiDaiDienCtrl;
  late final TextEditingController _sdtDaiDienCtrl;
  late final TextEditingController _noiDungCtrl;

  QuanHeCuTruModel? _selectedCanHo;
  DateTime? _duKienBatDau;
  DateTime? _duKienKetThuc;

  List<NhanSuThiCong> _danhSachNhanSu = [];
  final List<UploadedFile> _uploadedFiles = [];
  List<int> _existingTepIds = [];

  final List<XFile> _selectedImages = [];
  final List<int> _uploadedImageIds = [];
  bool _isUploading = false;
  bool _isSubmitting = false;

  bool get _isEditing => widget.existingDetail != null;
  bool get _isReturned => widget.existingDetail?.isReturned ?? false;

  @override
  void initState() {
    super.initState();
    final d = widget.existingDetail;
    _hangMucCtrl = TextEditingController(text: d?.hangMucThiCong ?? '');
    _donViCtrl = TextEditingController(text: d?.tenDonViThiCong ?? '');
    _nguoiDaiDienCtrl = TextEditingController(text: d?.nguoiDaiDien ?? '');
    _sdtDaiDienCtrl = TextEditingController(text: d?.soDienThoaiDaiDien ?? '');
    _noiDungCtrl = TextEditingController(text: d?.noiDung ?? '');
    _initFromEditData();
  }

  void _initFromEditData() {
    final d = widget.existingDetail;
    if (d == null) {
      if (widget.dsCanHo.isNotEmpty) _selectedCanHo = widget.dsCanHo.first;
      return;
    }
    _duKienBatDau = d.duKienBatDau;
    _duKienKetThuc = d.duKienKetThuc;
    _danhSachNhanSu = List.from(d.nhanSuThiCongs);
    _existingTepIds = d.danhSachTep.map((e) => e.id).toList();
    try {
      _selectedCanHo = widget.dsCanHo.firstWhere((c) => c.canHoId == d.canHoId);
    } catch (_) {
      if (widget.dsCanHo.isNotEmpty) _selectedCanHo = widget.dsCanHo.first;
    }
  }

  @override
  void dispose() {
    _hangMucCtrl.dispose();
    _donViCtrl.dispose();
    _nguoiDaiDienCtrl.dispose();
    _sdtDaiDienCtrl.dispose();
    _noiDungCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickImages() async {
    final picked = await _picker.pickMultiImage(imageQuality: 80);
    if (picked.isEmpty || !mounted) return;
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
      final uploaded = await _service.uploadMedia(files: files);
      setState(() => _uploadedImageIds.addAll(uploaded.map((u) => u.fileId)));
    } on Exception catch (e) {
      if (!mounted) return;
      ErrorDisplay.showSnackBar(context, error: e);
    } finally {
      if (mounted) setState(() => _isUploading = false);
    }
  }

  void _removeImage(int index) {
    setState(() {
      _selectedImages.removeAt(index);
      if (index < _uploadedImageIds.length) _uploadedImageIds.removeAt(index);
    });
  }

  Future<void> _pickAndUploadFile() async {
    final picked = await _picker.pickMultiImage();
    if (picked.isEmpty) return;
    setState(() => _isUploading = true);
    try {
      final files = picked.map((e) => File(e.path)).toList();
      final uploaded = await _service.uploadMedia(files: files);
      setState(() => _uploadedFiles.addAll(uploaded));
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Đã upload ${uploaded.length} tệp')),
        );
      }
    } on Exception catch (e) {
      if (!mounted) return;
      ErrorDisplay.showSnackBar(context, error: e);
    } finally {
      if (mounted) setState(() => _isUploading = false);
    }
  }

  Future<void> _pickDate({required bool isBatDau}) async {
    if (_isReturned) return;
    final initial = isBatDau
        ? (_duKienBatDau ?? DateTime.now())
        : (_duKienKetThuc ?? DateTime.now());
    final picked = await showDatePicker(
      context: context,
      initialDate: initial,
      firstDate: DateTime(2020),
      lastDate: DateTime(2035),
    );
    if (picked == null) return;
    setState(() {
      if (isBatDau) {
        _duKienBatDau = picked;
      } else {
        _duKienKetThuc = picked;
      }
    });
  }

  Future<void> _showAddNhanSuDialog() async {
    final result = await showDialog<NhanSuThiCong>(
      context: context,
      builder: (_) => const _AddNhanSuDialog(),
    );
    if (result != null) setState(() => _danhSachNhanSu.add(result));
  }

  void _removeNhanSu(int index) =>
      setState(() => _danhSachNhanSu.removeAt(index));

  Future<void> _submit({required bool isSubmit}) async {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedCanHo == null) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Vui lòng chọn căn hộ')));
      return;
    }
    if (_duKienBatDau == null || _duKienKetThuc == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng chọn ngày dự kiến bắt đầu và kết thúc'),
        ),
      );
      return;
    }
    if (_isUploading) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Đang upload, vui lòng chờ...')),
      );
      return;
    }

    setState(() => _isSubmitting = true);
    try {
      final allTepIds = [
        ..._existingTepIds,
        ..._uploadedFiles.map((e) => e.fileId),
        ..._uploadedImageIds,
      ];

      if (_isEditing) {
        await _service.update(
          id: widget.existingDetail!.id,
          hangMucThiCong: _hangMucCtrl.text.trim(),
          duKienBatDau: _duKienBatDau!,
          duKienKetThuc: _duKienKetThuc!,
          noiDung: _noiDungCtrl.text.trim(),
          tenDonViThiCong: _donViCtrl.text.trim(),
          nguoiDaiDien: _nguoiDaiDienCtrl.text.trim(),
          soDienThoaiDaiDien: _sdtDaiDienCtrl.text.trim(),
          danhSachNhanSu: _danhSachNhanSu,
          danhSachTepIds: allTepIds,
          isSubmit: isSubmit,
        );
      } else {
        await _service.create(
          canHoId: _selectedCanHo!.canHoId,
          hangMucThiCong: _hangMucCtrl.text.trim(),
          duKienBatDau: _duKienBatDau!,
          duKienKetThuc: _duKienKetThuc!,
          noiDung: _noiDungCtrl.text.trim(),
          tenDonViThiCong: _donViCtrl.text.trim(),
          nguoiDaiDien: _nguoiDaiDienCtrl.text.trim(),
          soDienThoaiDaiDien: _sdtDaiDienCtrl.text.trim(),
          danhSachNhanSu: _danhSachNhanSu,
          danhSachTepIds: allTepIds,
          isSubmit: isSubmit,
        );
      }

      if (!mounted) return;
      final msg = isSubmit
          ? (_isEditing ? 'Đã gửi lại yêu cầu' : 'Gửi yêu cầu thành công!')
          : 'Đã lưu nháp';
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
      Navigator.pop(context, true);
    } on Exception catch (e) {
      if (!mounted) return;
      ErrorDisplay.showSnackBar(context, error: e);
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: _isEditing ? 'Chỉnh sửa yêu cầu' : 'Tạo yêu cầu thi công',
      body: _buildForm(),
    );
  }

  Widget _buildForm() {
    final df = DateFormat('dd/MM/yyyy');

    return Form(
      key: _formKey,
      child: ListView(
        padding: AppSpacing.insetAll16,
        children: [
          if (_isReturned) ...[
            ErrorDisplay(
              error:
                  'Trạng thái Trả lại: chỉ được bổ sung nhân sự, hồ sơ '
                  'và nội dung. Không thể thay đổi hạng mục và ngày thi công.',
              compact: true,
            ),
            AppSpacing.sm.verticalSpace,
          ],

          if (widget.dsCanHo.length == 1)
            AppCard(
              child: Row(
                children: [
                  const Icon(
                    Icons.apartment_outlined,
                    color: AppColors.primary,
                  ),
                  AppSpacing.sm.horizontalSpace,
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('Căn hộ', style: AppTypography.caption.secondary),
                        Text(
                          widget.dsCanHo.first.diaChiDayDu,
                          style: AppTypography.bodyMedium,
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            )
          else
            DropdownButtonFormField<QuanHeCuTruModel>(
              initialValue: _selectedCanHo,
              decoration: InputDecoration(
                hintText: 'Chọn căn hộ *',
                hintStyle: AppTypography.input.disabled,
              ),
              items: widget.dsCanHo
                  .map(
                    (c) => DropdownMenuItem(
                      value: c,
                      child: Text(c.diaChiDayDu, style: AppTypography.input),
                    ),
                  )
                  .toList(),
              onChanged: (v) => setState(() => _selectedCanHo = v),
              validator: (v) => v == null ? 'Vui lòng chọn căn hộ' : null,
            ),
          AppSpacing.md.verticalSpace,

          AppTextField(
            controller: _hangMucCtrl,
            label: 'Hạng mục thi công',
            hint: 'Nhập hạng mục...',
            enabled: !_isReturned,
            maxLines: 2,
          ),
          AppSpacing.md.verticalSpace,

          Row(
            children: [
              Expanded(
                child: _DateField(
                  label: 'Bắt đầu *',
                  value: _duKienBatDau != null
                      ? df.format(_duKienBatDau!)
                      : null,
                  enabled: !_isReturned,
                  onTap: () => _pickDate(isBatDau: true),
                ),
              ),
              AppSpacing.sm.horizontalSpace,
              Expanded(
                child: _DateField(
                  label: 'Kết thúc *',
                  value: _duKienKetThuc != null
                      ? df.format(_duKienKetThuc!)
                      : null,
                  enabled: !_isReturned,
                  onTap: () => _pickDate(isBatDau: false),
                ),
              ),
            ],
          ),
          AppSpacing.md.verticalSpace,

          AppTextField(
            controller: _donViCtrl,
            label: 'Đơn vị thi công',
            hint: 'Tên đơn vị...',
          ),
          AppSpacing.md.verticalSpace,

          AppTextField(
            controller: _nguoiDaiDienCtrl,
            label: 'Người đại diện',
            hint: 'Họ tên người đại diện...',
          ),
          AppSpacing.md.verticalSpace,

          AppTextField(
            controller: _sdtDaiDienCtrl,
            label: 'Số điện thoại đại diện',
            hint: '0xxx...',
            keyboardType: TextInputType.phone,
          ),
          AppSpacing.md.verticalSpace,

          AppTextField(
            controller: _noiDungCtrl,
            label: 'Nội dung chi tiết',
            hint: 'Mô tả chi tiết công việc cần thi công...',
            maxLines: 4,
          ),
          AppSpacing.xl.verticalSpace,

          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Danh sách nhân sự (${_danhSachNhanSu.length})',
                style: AppTypography.subhead,
              ),
              TextButton.icon(
                onPressed: _showAddNhanSuDialog,
                icon: const Icon(Icons.person_add_outlined, size: 18),
                label: const Text('Thêm'),
                style: TextButton.styleFrom(foregroundColor: AppColors.primary),
              ),
            ],
          ),
          AppSpacing.xs.verticalSpace,
          ..._danhSachNhanSu.asMap().entries.map(
            (entry) => AppCard(
              margin: const EdgeInsets.only(bottom: 6),
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.md,
                vertical: AppSpacing.sm,
              ),
              child: Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          entry.value.hoTen,
                          style: AppTypography.bodyMedium,
                        ),
                        Text(
                          '${entry.value.vaiTro} • CCCD: ${entry.value.soCCCD}',
                          style: AppTypography.captionSmall.secondary,
                        ),
                      ],
                    ),
                  ),
                  IconButton(
                    icon: const Icon(
                      Icons.delete_outline,
                      color: AppColors.error,
                      size: 20,
                    ),
                    onPressed: () => _removeNhanSu(entry.key),
                  ),
                ],
              ),
            ),
          ),
          AppSpacing.xl.verticalSpace,

          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('Hồ sơ đính kèm', style: AppTypography.subhead),
              TextButton.icon(
                onPressed: _isUploading ? null : _pickAndUploadFile,
                icon: _isUploading
                    ? const SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          color: AppColors.primary,
                        ),
                      )
                    : const Icon(Icons.upload_file_outlined, size: 18),
                label: Text(_isUploading ? 'Đang tải...' : 'Tải lên'),
                style: TextButton.styleFrom(foregroundColor: AppColors.primary),
              ),
            ],
          ),
          if (widget.existingDetail != null)
            ...widget.existingDetail!.danhSachTep.map(
              (tep) => ListTile(
                dense: true,
                contentPadding: EdgeInsets.zero,
                leading: const Icon(
                  Icons.insert_drive_file_outlined,
                  color: AppColors.textSecondary,
                ),
                title: Text(
                  tep.fileName.isNotEmpty ? tep.fileName : 'Tệp #${tep.id}',
                  style: AppTypography.captionSmall,
                ),
                subtitle: Text(
                  '(đã lưu)',
                  style: AppTypography.captionSmall.secondary,
                ),
              ),
            ),
          ..._uploadedFiles.map(
            (f) => ListTile(
              dense: true,
              contentPadding: EdgeInsets.zero,
              leading: const Icon(
                Icons.check_circle_outline,
                color: AppColors.success,
              ),
              title: Text(f.fileName, style: AppTypography.captionSmall),
              subtitle: Text(
                '(vừa upload)',
                style: AppTypography.captionSmall.withColor(AppColors.success),
              ),
              trailing: IconButton(
                icon: const Icon(Icons.close, size: 18, color: AppColors.error),
                onPressed: () => setState(() => _uploadedFiles.remove(f)),
              ),
            ),
          ),
          AppSpacing.xl.verticalSpace,

          _buildImageSection(),
          AppSpacing.xl.verticalSpace,

          if (_isSubmitting)
            const Center(
              child: CircularProgressIndicator(color: AppColors.primary),
            )
          else
            Column(
              children: [
                AppButton(
                  label: _isEditing ? 'Gửi lại yêu cầu' : 'Gửi yêu cầu ngay',
                  leadingIcon: Icons.send_outlined,
                  onPressed: () => _submit(isSubmit: true),
                ),
                AppSpacing.sm.verticalSpace,
                AppButton(
                  label: 'Lưu nháp',
                  variant: AppButtonVariant.outline,
                  leadingIcon: Icons.save_outlined,
                  onPressed: () => _submit(isSubmit: false),
                ),
              ],
            ),
          AppSpacing.xxl.verticalSpace,
        ],
      ),
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
              style: AppTypography.subhead,
            ),
            if (_isUploading) ...[
              AppSpacing.sm.horizontalSpace,
              const SizedBox(
                width: 14,
                height: 14,
                child: CircularProgressIndicator(
                  strokeWidth: 2,
                  color: AppColors.primary,
                ),
              ),
              AppSpacing.xs.horizontalSpace,
              Text(
                'Đang upload...',
                style: AppTypography.captionSmall.secondary,
              ),
            ],
          ],
        ),
        AppSpacing.sm.verticalSpace,
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
                        borderRadius: AppRadius.buttonSmall,
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
                            color: AppColors.success,
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
                      border: Border.all(color: AppColors.border),
                      borderRadius: AppRadius.buttonSmall,
                      color: AppColors.inputFill,
                    ),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(
                          Icons.add_photo_alternate_outlined,
                          size: 32,
                          color: AppColors.textDisabled,
                        ),
                        AppSpacing.xs.verticalSpace,
                        Text(
                          'Thêm ảnh',
                          style: AppTypography.captionSmall.secondary,
                        ),
                      ],
                    ),
                  ),
                ),
            ],
          ),
        ),
        AppSpacing.xs.verticalSpace,
        Text(
          'JPG/PNG, tối đa 5MB/ảnh',
          style: AppTypography.captionSmall.secondary,
        ),
      ],
    );
  }
}

class _DateField extends StatelessWidget {
  final String label;
  final String? value;
  final bool enabled;
  final VoidCallback onTap;

  const _DateField({
    required this.label,
    required this.value,
    required this.enabled,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: enabled ? onTap : null,
      borderRadius: AppRadius.inputField,
      child: InputDecorator(
        decoration: InputDecoration(
          hintText: label,
          hintStyle: AppTypography.input.disabled,
          suffixIcon: const Icon(
            Icons.calendar_today_outlined,
            size: 18,
            color: AppColors.textSecondary,
          ),
          enabled: enabled,
        ),
        isEmpty: value == null,
        child: value != null
            ? Text(value!, style: AppTypography.input)
            : const SizedBox.shrink(),
      ),
    );
  }
}

class _AddNhanSuDialog extends StatefulWidget {
  const _AddNhanSuDialog();

  @override
  State<_AddNhanSuDialog> createState() => _AddNhanSuDialogState();
}

class _AddNhanSuDialogState extends State<_AddNhanSuDialog> {
  final _formKey = GlobalKey<FormState>();
  final _hoTenCtrl = TextEditingController();
  final _cccdCtrl = TextEditingController();
  final _sdtCtrl = TextEditingController();
  final _vaiTroCtrl = TextEditingController();
  final _ghiChuCtrl = TextEditingController();

  @override
  void dispose() {
    _hoTenCtrl.dispose();
    _cccdCtrl.dispose();
    _sdtCtrl.dispose();
    _vaiTroCtrl.dispose();
    _ghiChuCtrl.dispose();
    super.dispose();
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    Navigator.pop(
      context,
      NhanSuThiCong(
        hoTen: _hoTenCtrl.text.trim(),
        soCCCD: _cccdCtrl.text.trim(),
        soDienThoai: _sdtCtrl.text.trim(),
        vaiTro: _vaiTroCtrl.text.trim(),
        ghiChu: _ghiChuCtrl.text.trim(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text('Thêm nhân sự', style: AppTypography.headline),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              AppTextField(
                controller: _hoTenCtrl,
                label: 'Họ tên',
                hint: 'Nhập họ tên...',
                errorText: null,
              ),
              AppSpacing.sm.verticalSpace,
              AppTextField(
                controller: _cccdCtrl,
                label: 'Số CCCD',
                hint: 'Nhập số CCCD...',
                keyboardType: TextInputType.number,
              ),
              AppSpacing.sm.verticalSpace,
              AppTextField(
                controller: _sdtCtrl,
                label: 'Số điện thoại',
                hint: '0xxx...',
                keyboardType: TextInputType.phone,
              ),
              AppSpacing.sm.verticalSpace,
              AppTextField(
                controller: _vaiTroCtrl,
                label: 'Vai trò',
                hint: 'VD: Thợ chính...',
              ),
              AppSpacing.sm.verticalSpace,
              AppTextField(
                controller: _ghiChuCtrl,
                label: 'Ghi chú',
                hint: 'Ghi chú thêm...',
              ),
            ],
          ),
        ),
      ),
      actions: [
        AppButton(
          label: 'Hủy',
          variant: AppButtonVariant.outline,
          expanded: false,
          height: 40,
          onPressed: () => Navigator.pop(context),
        ),
        AppButton(
          label: 'Thêm',
          expanded: false,
          height: 40,
          onPressed: _submit,
        ),
      ],
    );
  }
}
