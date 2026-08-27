import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/widgets/shared_widget.dart';
import 'package:klks_app/features/shared/widgets/selector_field.dart';

import '../models/thanh_vien_model.dart';
import '../services/thanh_vien_service.dart';
import '../widgets/tai_lieu_cu_tru_editor.dart';
import '../widgets/tv_shared_widgets.dart';

sealed class YeuCauFormMode {
  const YeuCauFormMode();
}

class YeuCauFormCreate extends YeuCauFormMode {
  final QuanHeCuTruModel canHoInfo;
  const YeuCauFormCreate({required this.canHoInfo});
}

class YeuCauFormEdit extends YeuCauFormMode {
  final ThanhVienCuTruModel thanhVien;
  final QuanHeCuTruModel canHoInfo;
  final ThongTinCuDanModel thongTinCuDan;

  const YeuCauFormEdit({
    required this.thanhVien,
    required this.canHoInfo,
    required this.thongTinCuDan,
  });
}

class YeuCauFormDraft extends YeuCauFormMode {
  final int yeuCauId;
  const YeuCauFormDraft({required this.yeuCauId});
}

class YeuCauCuTruFormScreen extends StatefulWidget {
  final YeuCauFormMode mode;

  const YeuCauCuTruFormScreen({super.key, required this.mode});

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;

    switch (e['mode']) {
      case 'create':
        return YeuCauCuTruFormScreen(
          mode: YeuCauFormCreate(canHoInfo: e['canHoInfo']),
        );
      case 'edit':
        return YeuCauCuTruFormScreen(
          mode: YeuCauFormEdit(
            thanhVien: e['thanhVien'],
            canHoInfo: e['canHoInfo'],
            thongTinCuDan: e['thongTinCuDan'],
          ),
        );
      case 'draft':
        return YeuCauCuTruFormScreen(
          mode: YeuCauFormDraft(yeuCauId: e['yeuCauId']),
        );
      default:
        throw StateError('Unknown mode: ${e['mode']}');
    }
  }

  @override
  State<YeuCauCuTruFormScreen> createState() => _YeuCauCuTruFormScreenState();
}

class _YeuCauCuTruFormScreenState extends State<YeuCauCuTruFormScreen> {
  final _thanhVienSvc = ThanhVienService.instance;

  final _formKey = GlobalKey<FormState>();
  final _scrollCtrl = ScrollController();

  bool _isLoading = false;
  String? _loadError;

  YeuCauCuTruModel? _draftYeuCau;
  ThongTinCuDanModel? _cuDan;

  final _hoCtrl = TextEditingController();
  final _tenCtrl = TextEditingController();
  final _cccdCtrl = TextEditingController();
  final _phoneCtrl = TextEditingController();
  final _diaChiCtrl = TextEditingController();
  final _noiDungCtrl = TextEditingController();

  DateTime? _dob;
  SelectorItem? _gioiTinh;
  SelectorItem? _loaiQuanHe;

  final _taiLieuNotifier = ValueNotifier<List<TaiLieuCuTruRequest>>([]);

  bool _isSubmitting = false;

  late final Future<List<SelectorItem>> _gioiTinhFuture = _thanhVienSvc
      .getGioiTinhSelector();
  late final Future<List<SelectorItem>> _loaiQuanHeFuture = _thanhVienSvc
      .getLoaiQuanHeCuTruSelector();

  bool get _isCreate => widget.mode is YeuCauFormCreate;
  bool get _isEdit => widget.mode is YeuCauFormEdit;
  bool get _isDraft => widget.mode is YeuCauFormDraft;

  QuanHeCuTruModel? get _canHoInfo => switch (widget.mode) {
    YeuCauFormCreate(canHoInfo: final c) => c,
    YeuCauFormEdit(canHoInfo: final c) => c,
    _ => null,
  };

  ThanhVienCuTruModel? get _thanhVien => switch (widget.mode) {
    YeuCauFormEdit(thanhVien: final m) => m,
    _ => null,
  };

  String get _appBarTitle => switch (widget.mode) {
    YeuCauFormCreate() => 'Thêm thành viên',
    YeuCauFormEdit() => 'Yêu cầu sửa thành viên',
    YeuCauFormDraft(yeuCauId: final id) => 'Chỉnh sửa yêu cầu #$id',
  };

  String get _sectionLabel => switch (widget.mode) {
    YeuCauFormCreate() => 'Thông tin người thêm *',
    YeuCauFormEdit() => 'Thông tin cần sửa *',
    YeuCauFormDraft() => 'Thông tin người được yêu cầu *',
  };

  @override
  void initState() {
    super.initState();
    _initLoad();
  }

  @override
  void dispose() {
    _hoCtrl.dispose();
    _tenCtrl.dispose();
    _cccdCtrl.dispose();
    _phoneCtrl.dispose();
    _diaChiCtrl.dispose();
    _noiDungCtrl.dispose();
    _scrollCtrl.dispose();
    _taiLieuNotifier.dispose();
    super.dispose();
  }

  Future<void> _initLoad() async {
    switch (widget.mode) {
      case YeuCauFormCreate():
        break;
      case YeuCauFormEdit(thongTinCuDan: final data):
        {
          _prefillFromCuDan(data);
          await _loadCatalogAndPreselect(data);
        }
      case YeuCauFormDraft(yeuCauId: final id):
        await _loadDraftAndCatalog(id);
    }
  }

  Future<void> _loadCatalogAndPreselect(ThongTinCuDanModel d) async {
    _setLoading(true);
    try {
      final results = await Future.wait([_gioiTinhFuture, _loaiQuanHeFuture]);
      if (!mounted) return;
      setState(() {
        _gioiTinh = results[0].where((e) => e.id == d.gioiTinhId).firstOrNull;
        _loaiQuanHe = results[1]
            .where((e) => e.id == d.loaiQuanHeCuTruId)
            .firstOrNull;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _loadError = e.toString());
    } finally {
      _setLoading(false);
    }
  }

  Future<void> _loadDraftAndCatalog(int yeuCauId) async {
    _setLoading(true);
    try {
      final results = await Future.wait([
        _thanhVienSvc.getYeuCauById(yeuCauId),
        _gioiTinhFuture,
        _loaiQuanHeFuture,
      ]);
      if (!mounted) return;
      final d = results[0] as YeuCauCuTruModel;
      _prefillFromDraft(d);
      setState(() {
        _draftYeuCau = d;
        _gioiTinh = (results[1] as List<SelectorItem>)
            .where((e) => e.id == d.yeuCauGioiTinhId)
            .firstOrNull;
        _loaiQuanHe = (results[2] as List<SelectorItem>)
            .where((e) => e.id == d.yeuCauLoaiQuanHeId)
            .firstOrNull;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _loadError = e.toString());
    } finally {
      _setLoading(false);
    }
  }

  void _prefillFromCuDan(ThongTinCuDanModel d) {
    _cuDan = d;
    _hoCtrl.text = d.lastName;
    _tenCtrl.text = d.firstName;
    _cccdCtrl.text = d.idCard ?? '';
    _phoneCtrl.text = d.phoneNumber ?? '';
    _diaChiCtrl.text = d.diaChi ?? '';
    _dob = d.dob;
  }

  void _prefillFromDraft(YeuCauCuTruModel d) {
    _hoCtrl.text = d.yeuCauHo ?? '';
    _tenCtrl.text = d.yeuCauTen ?? '';
    _cccdCtrl.text = d.yeuCauCCCD ?? '';
    _phoneCtrl.text = d.yeuCauSoDienThoai ?? '';
    _diaChiCtrl.text = d.yeuCauDiaChi ?? '';
    _noiDungCtrl.text = d.noiDung ?? '';
    _dob = d.yeuCauNgaySinh;
  }

  void _setLoading(bool v) {
    if (mounted) setState(() => _isLoading = v);
  }

  bool _validateRequiredFields() {
    final missing = <String>[
      if (_dob == null) 'Ngày sinh',
      if (_gioiTinh == null) 'Giới tính',
      if (_loaiQuanHe == null) 'Loại quan hệ',
    ];
    if (missing.isNotEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Vui lòng điền: ${missing.join(', ')}')),
      );
      return false;
    }
    return true;
  }

  Future<void> _submit(bool isSubmit) async {
    if (!_formKey.currentState!.validate()) return;
    if (!_validateRequiredFields()) return;
    if (_isSubmitting) return;

    if (_isDraft && isSubmit) {
      final ok = await AppConfirmDialog.show(
        context,
        title: 'Xác nhận gửi yêu cầu',
        message:
            'Sau khi gửi, yêu cầu sẽ chuyển sang trạng thái chờ duyệt '
            'và không thể chỉnh sửa. Tiếp tục?',
        confirmLabel: 'Gửi',
      );
      if (ok != true || !mounted) return;
    }

    setState(() => _isSubmitting = true);

    try {
      final taiLieus = _taiLieuNotifier.value;

      if (_isDraft) {
        final mode = widget.mode as YeuCauFormDraft;
        await _thanhVienSvc.updateYeuCau(
          CapNhatYeuCauCuTruRequest(
            id: mode.yeuCauId,
            isSubmit: isSubmit,
            lastName: _trim(_hoCtrl),
            firstName: _trim(_tenCtrl),
            dob: _dob,
            gioiTinhId: _gioiTinh?.id,
            loaiQuanHeId: _loaiQuanHe?.id,
            cccd: _trimOrNull(_cccdCtrl),
            phoneNumber: _trimOrNull(_phoneCtrl),
            diaChi: _trimOrNull(_diaChiCtrl),
            noiDung: _trimOrNull(_noiDungCtrl),
            taiLieuCuTrus: taiLieus.isEmpty ? null : taiLieus,
          ),
        );
      } else {
        await _thanhVienSvc.createYeuCau(
          TaoYeuCauCuTruRequest(
            canHoId: _canHoInfo!.canHoId,
            loaiYeuCauId: _isCreate ? 1 : 2,
            isSubmit: isSubmit,
            targetQuanHeCuTruId: _isEdit ? _thanhVien!.quanHeCuTruId : null,
            firstName: _trim(_tenCtrl),
            lastName: _trim(_hoCtrl),
            dob: _dob,
            gioiTinhId: _gioiTinh!.id,
            loaiQuanHeId: _loaiQuanHe!.id,
            cccd: _trimOrNull(_cccdCtrl),
            phoneNumber: _trimOrNull(_phoneCtrl),
            diaChi: _trimOrNull(_diaChiCtrl),
            noiDung: _trimOrNull(_noiDungCtrl),
            taiLieuCuTrus: taiLieus.isEmpty ? null : taiLieus,
          ),
        );
      }

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(isSubmit ? 'Đã nộp yêu cầu thành công' : 'Đã lưu nháp'),
        ),
      );
      Navigator.pop(context, true);
    } on Exception catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  Future<void> _pickDob() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _dob ?? DateTime(1990),
      firstDate: DateTime(1900),
      lastDate: DateTime.now(),
    );
    if (picked != null && mounted) setState(() => _dob = picked);
  }

  String _trim(TextEditingController c) => c.text.trim();
  String? _trimOrNull(TextEditingController c) {
    final v = c.text.trim();
    return v.isEmpty ? null : v;
  }

  String? _required(String? v) =>
      (v == null || v.trim().isEmpty) ? 'Trường này là bắt buộc' : null;

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      appBar: AppTopBar(title: _appBarTitle),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_loadError != null) {
      return ErrorDisplay(error: _loadError, onRetry: _initLoad);
    }

    if (_isSubmitting) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    return Form(
      key: _formKey,
      child: ListView(
        controller: _scrollCtrl,
        padding: AppSpacing.insetAll16,
        children: [
          _buildReadonlyCard(),
          const SizedBox(height: AppSpacing.lg),

          SectionLabel(_sectionLabel),

          Row(
            children: [
              Expanded(
                child: Field(
                  controller: _hoCtrl,
                  label: 'Họ *',
                  validator: _required,
                ),
              ),
              const SizedBox(width: AppSpacing.sm2),
              Expanded(
                child: Field(
                  controller: _tenCtrl,
                  label: 'Tên *',
                  validator: _required,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppSpacing.sm2),

          DatePickerField(label: 'Ngày sinh *', value: _dob, onTap: _pickDob),
          const SizedBox(height: AppSpacing.sm2),

          SelectorField.future(
            label: 'Giới tính *',
            future: _gioiTinhFuture,
            selectedItems: _gioiTinh != null ? [_gioiTinh!] : [],
            isRequired: true,
            onChangedSingle: (v) => setState(() => _gioiTinh = v),
          ),
          const SizedBox(height: AppSpacing.sm2),

          SelectorField.future(
            label: 'Loại quan hệ *',
            future: _loaiQuanHeFuture,
            selectedItems: _loaiQuanHe != null ? [_loaiQuanHe!] : [],
            isRequired: true,
            onChangedSingle: (v) => setState(() => _loaiQuanHe = v),
          ),
          const SizedBox(height: AppSpacing.lg),

          const SectionLabel('Thông tin bổ sung'),

          Field(
            controller: _cccdCtrl,
            label: 'CMND/CCCD',
            keyboardType: TextInputType.number,
          ),
          const SizedBox(height: AppSpacing.sm2),

          Field(
            controller: _phoneCtrl,
            label: 'Số điện thoại',
            keyboardType: TextInputType.phone,
          ),
          const SizedBox(height: AppSpacing.sm2),

          Field(controller: _diaChiCtrl, label: 'Địa chỉ thường trú'),
          const SizedBox(height: AppSpacing.sm2),

          Field(
            controller: _noiDungCtrl,
            label: _isDraft ? 'Ghi chú' : 'Nội dung',
            maxLines: 3,
          ),
          const SizedBox(height: AppSpacing.lg),

          const SectionLabel('Tài liệu đính kèm'),

          TaiLieuCuTruEditor(
            key: const ValueKey('tai_lieu_editor'),
            initialDocuments: _isDraft
                ? _draftYeuCau?.documents
                : _cuDan?.taiLieuCuTrus,
            onChanged: (list) => _taiLieuNotifier.value = list,
          ),
          const SizedBox(height: AppSpacing.lg),

          Row(
            children: [
              Expanded(
                child: AppButton(
                  label: 'Lưu nháp',
                  variant: AppButtonVariant.outline,
                  onPressed: () => _submit(false),
                ),
              ),
              const SizedBox(width: AppSpacing.sm2),
              Expanded(
                child: AppButton(
                  label: 'Nộp yêu cầu',
                  onPressed: () => _submit(true),
                ),
              ),
            ],
          ),
          const SizedBox(height: AppSpacing.md),
        ],
      ),
    );
  }

  Widget _buildReadonlyCard() => switch (widget.mode) {
    YeuCauFormCreate(canHoInfo: final c) => ReadonlyCanHoCard(canHoInfo: c),
    YeuCauFormEdit(thanhVien: final tv, canHoInfo: final c) =>
      TvMemberReadonlyCard(
        thanhVien: tv,
        diaChiCanHo: c.diaChiDayDu,
        badgeLabel: 'Sửa',
        badgeVariant: AppBadgeVariant.warning,
      ),
    YeuCauFormDraft() =>
      _draftYeuCau != null
          ? _DraftReadonlyCard(yeuCau: _draftYeuCau!)
          : const SizedBox.shrink(),
  };
}

class _DraftReadonlyCard extends StatelessWidget {
  final YeuCauCuTruModel yeuCau;
  const _DraftReadonlyCard({required this.yeuCau});

  @override
  Widget build(BuildContext context) {
    return AppCard(
      color: AppColors.secondaryLight,
      child: Row(
        children: [
          const Icon(
            Icons.apartment_outlined,
            color: AppColors.primary,
            size: 20,
          ),
          const SizedBox(width: AppSpacing.sm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(yeuCau.diaChiCanHo, style: AppTypography.subhead),
                Text(
                  'Loại yêu cầu: ${yeuCau.tenLoaiYeuCau}',
                  style: AppTypography.captionSmall.secondary,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
