import 'dart:io';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/widgets/can_ho_selector.dart';
import 'package:klks_app/features/shared/services/upload_service.dart';
import 'package:klks_app/features/shared/widgets/selector_field.dart';
import 'package:klks_app/features/shared/widgets/file_upload_field.dart';

import '../models/sua_chua_model.dart';
import '../services/sua_chua_service.dart';

class SuaChuaCreateScreen extends StatefulWidget {
  final List<QuanHeCuTruModel> dsCanHo;
  final YeuCauSuaChua? editData;

  const SuaChuaCreateScreen({super.key, required this.dsCanHo, this.editData});

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;

    final List<QuanHeCuTruModel> dsCanHo = e['dsCanHo'];
    final YeuCauSuaChua? editData = e['editData'];

    return SuaChuaCreateScreen(dsCanHo: dsCanHo, editData: editData);
  }

  @override
  State<SuaChuaCreateScreen> createState() => _SuaChuaCreateScreenState();
}

class _SuaChuaCreateScreenState extends State<SuaChuaCreateScreen> {
  final _service = YeuCauSuaChuaService.instance;
  final _formKey = GlobalKey<FormState>();

  final _noiDungCtrl = TextEditingController();
  final _moTaViTriCtrl = TextEditingController();

  QuanHeCuTruModel? _selectedCanHo;
  SelectorItem? _selectedPhamVi;
  SelectorItem? _selectedLoaiSuCo;

  List<UploadedFile> _uploadedFiles = [];

  bool _isSubmitting = false;
  bool _isCatalogLoading = true;
  Object? _catalogError;

  List<SelectorItem> _dsPhamVi = [];
  List<SelectorItem> _dsLoaiSuCo = [];

  bool get _isEditMode => widget.editData != null;

  @override
  void initState() {
    super.initState();
    _loadCatalogs();
  }

  @override
  void dispose() {
    _noiDungCtrl.dispose();
    _moTaViTriCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadCatalogs() async {
    setState(() {
      _isCatalogLoading = true;
      _catalogError = null;
    });
    try {
      final results = await Future.wait([
        _service.getPhamViSuaChua(),
        _service.getLoaiSuCo(),
      ]);
      setState(() {
        _dsPhamVi = results[0];
        _dsLoaiSuCo = results[1];
        _isCatalogLoading = false;
      });
      _initFromEditData();
    } catch (e) {
      setState(() {
        _catalogError = e;
        _isCatalogLoading = false;
      });
    }
  }

  void _initFromEditData() {
    final d = widget.editData;
    if (d == null) {
      setState(() {
        _selectedCanHo = widget.dsCanHo.isNotEmpty
            ? widget.dsCanHo.first
            : null;
      });
      return;
    }

    _noiDungCtrl.text = d.noiDung;
    _moTaViTriCtrl.text = d.moTaViTri ?? '';

    setState(() {
      _selectedCanHo =
          widget.dsCanHo.where((c) => c.canHoId == d.canHoId).firstOrNull ??
          (widget.dsCanHo.isNotEmpty ? widget.dsCanHo.first : null);

      if (d.phamViId != null) {
        _selectedPhamVi = _dsPhamVi
            .where((p) => p.id == d.phamViId)
            .firstOrNull;
      }
      if (d.loaiSuCoId != null) {
        _selectedLoaiSuCo = _dsLoaiSuCo
            .where((l) => l.id == d.loaiSuCoId)
            .firstOrNull;
      }

      _uploadedFiles = d.danhSachTep
          .map(
            (f) => UploadedFile(
              fileId: f.id,
              fileName: f.fileName,
              fileUrl: f.fileUrl,
              contentType: f.contentType,
            ),
          )
          .toList();
    });
  }

  Future<void> _submit({required bool isSubmit}) async {
    if (!_formKey.currentState!.validate()) return;

    if (_selectedPhamVi == null || _selectedLoaiSuCo == null) {
      ErrorDisplay.showSnackBar(
        context,
        error: Exception('Vui lòng chọn đầy đủ phạm vi và loại sự cố'),
      );
      return;
    }

    setState(() => _isSubmitting = true);
    try {
      if (_isEditMode) {
        await _service.capNhatYeuCau(
          CapNhatYeuCauRequest(
            id: widget.editData!.id,
            phamViId: _selectedPhamVi!.id,
            loaiSuCoId: _selectedLoaiSuCo!.id,
            noiDung: _noiDungCtrl.text.trim(),
            moTaViTri: _moTaViTriCtrl.text.trim().isEmpty
                ? null
                : _moTaViTriCtrl.text.trim(),
            danhSachTepIds: _uploadedFiles.map((f) => f.fileId).toList(),
            isSubmit: isSubmit,
          ),
        );
      } else {
        await _service.taoYeuCau(
          TaoYeuCauRequest(
            canHoId: _selectedCanHo!.canHoId,
            phamViId: _selectedPhamVi!.id,
            loaiSuCoId: _selectedLoaiSuCo!.id,
            noiDung: _noiDungCtrl.text.trim(),
            moTaViTri: _moTaViTriCtrl.text.trim().isEmpty
                ? null
                : _moTaViTriCtrl.text.trim(),
            danhSachTepIds: _uploadedFiles.map((f) => f.fileId).toList(),
            isSubmit: isSubmit,
          ),
        );
      }

      if (!mounted) return;
      final msg = isSubmit
          ? (_isEditMode ? 'Đã gửi lại yêu cầu' : 'Gửi yêu cầu thành công')
          : 'Đã lưu nháp';
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(msg), backgroundColor: AppColors.success),
      );
      Navigator.pop(context, true);
    } catch (e) {
      if (!mounted) return;
      ErrorDisplay.showSnackBar(context, error: e);
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: _isEditMode ? 'Chỉnh sửa yêu cầu' : 'Tạo yêu cầu mới',
      body: _isCatalogLoading
          ? const Center(child: CircularProgressIndicator())
          : _catalogError != null
          ? ErrorDisplay.fullScreen(
              error: _catalogError,
              onRetry: _loadCatalogs,
            )
          : _buildForm(),
    );
  }

  Widget _buildForm() {
    return Form(
      key: _formKey,
      child: ListView(
        padding: AppSpacing.insetAll16,
        children: [
          CanHoSelector(
            dsCanHo: widget.dsCanHo,
            selected: _selectedCanHo,
            onChanged: (c) => setState(() => _selectedCanHo = c),
          ),
          AppSpacing.md.verticalSpace,

          SelectorField(
            label: 'Phạm vi sửa chữa',
            isRequired: true,
            items: _dsPhamVi,
            selectedItems: _selectedPhamVi != null
                ? [_selectedPhamVi!]
                : const [],
            onChangedSingle: (v) => setState(() => _selectedPhamVi = v),
          ),
          AppSpacing.md.verticalSpace,

          SelectorField(
            label: 'Loại sự cố',
            isRequired: true,
            items: _dsLoaiSuCo,
            selectedItems: _selectedLoaiSuCo != null
                ? [_selectedLoaiSuCo!]
                : const [],
            onChangedSingle: (v) => setState(() => _selectedLoaiSuCo = v),
          ),
          AppSpacing.md.verticalSpace,

          AppTextField(
            label: 'Mô tả sự cố',
            hint: 'Vd: Bình nóng lạnh không vào điện từ tối qua',
            controller: _noiDungCtrl,
            maxLines: 4,
          ),
          AppSpacing.md.verticalSpace,

          AppTextField(
            label: 'Mô tả vị trí',
            hint: 'Vd: Phòng tắm chung, gần cửa sổ...',
            controller: _moTaViTriCtrl,
            maxLines: 2,
          ),
          AppSpacing.md.verticalSpace,

          AppFileUploadField(
            label: 'Ảnh hiện trạng',
            targetContainer: 'yeu-cau-sua-chua',
            uploadFn:
                ({
                  required List<File> files,
                  required String targetContainer,
                }) => UploadService.instance.uploadMedia(
                  files: files,
                  targetContainer: targetContainer,
                ),
            initialFiles: _uploadedFiles,
            maxFiles: 5,
            onChanged: (files) => _uploadedFiles = files.toList(),
          ),
          AppSpacing.xl.verticalSpace,

          AppButton(
            label: _isEditMode ? 'Gửi lại yêu cầu' : 'Gửi yêu cầu ngay',
            isLoading: _isSubmitting,
            leadingIcon: Icons.send_outlined,
            onPressed: _isSubmitting ? null : () => _submit(isSubmit: true),
          ),
          AppSpacing.sm.verticalSpace,
          AppButton(
            label: 'Lưu nháp',
            variant: AppButtonVariant.outline,
            leadingIcon: Icons.save_outlined,
            onPressed: _isSubmitting ? null : () => _submit(isSubmit: false),
          ),
          AppSpacing.lg.verticalSpace,
        ],
      ),
    );
  }
}
