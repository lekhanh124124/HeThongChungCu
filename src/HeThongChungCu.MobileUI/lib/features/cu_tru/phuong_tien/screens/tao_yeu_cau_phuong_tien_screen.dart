import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import 'package:klks_app/features/shared/widgets/selector_field.dart';
import 'package:klks_app/features/shared/widgets/file_upload_field.dart';

import 'package:klks_app/features/cu_tru/quan_he/widgets/shared_widget.dart';

import '../models/phuong_tien_model.dart';
import '../services/phuong_tien_service.dart';

enum _LoaiYeuCau {
  them(1, 'Đăng ký xe mới'),
  sua(2, 'Sửa thông tin xe'),
  xoa(3, 'Huỷ đăng ký xe');

  const _LoaiYeuCau(this.id, this.label);
  final int id;
  final String label;

  static _LoaiYeuCau fromId(int id) =>
      _LoaiYeuCau.values.firstWhere((e) => e.id == id, orElse: () => them);
}

class TaoYeuCauPhuongTienScreen extends StatefulWidget {
  final QuanHeCuTruModel canHoInfo;
  final int loaiYeuCauId;
  final PhuongTien? phuongTien;

  const TaoYeuCauPhuongTienScreen({
    super.key,
    required this.canHoInfo,
    this.loaiYeuCauId = 1,
    this.phuongTien,
  }) : assert(
         loaiYeuCauId == 1 || phuongTien != null,
         'phuongTien bắt buộc khi loaiYeuCauId = 2 hoặc 3',
       );

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final args = state.extra as Map<String, dynamic>;
    return TaoYeuCauPhuongTienScreen(
      canHoInfo: args['canHoInfo'] as QuanHeCuTruModel,
      loaiYeuCauId: args['loaiYeuCauId'] as int? ?? 1,
      phuongTien: args['phuongTien'] as PhuongTien?,
    );
  }

  @override
  State<TaoYeuCauPhuongTienScreen> createState() =>
      _TaoYeuCauPhuongTienScreenState();
}

class _TaoYeuCauPhuongTienScreenState extends State<TaoYeuCauPhuongTienScreen> {
  final _ptService = PhuongTienService.instance;
  final _formKey = GlobalKey<FormState>();

  late final _LoaiYeuCau _loai;

  late final TextEditingController _tenXeCtrl;
  late final TextEditingController _bienSoCtrl;
  late final TextEditingController _mauXeCtrl;
  late final TextEditingController _noiDungCtrl;

  SelectorItem? _loaiPhuongTien;

  final List<UploadedFile> _uploadedFiles = [];

  bool _isSubmitting = false;

  @override
  void initState() {
    super.initState();
    _loai = _LoaiYeuCau.fromId(widget.loaiYeuCauId);

    final pt = widget.phuongTien;

    _tenXeCtrl = TextEditingController(text: pt?.tenPhuongTien ?? '');
    _bienSoCtrl = TextEditingController(text: pt?.bienSo ?? '');
    _mauXeCtrl = TextEditingController(text: pt?.mauXe ?? '');
    _noiDungCtrl = TextEditingController();

    if (pt != null && pt.loaiPhuongTienId != 0) {
      _loaiPhuongTien = SelectorItem(
        id: pt.loaiPhuongTienId,
        name: pt.tenLoaiPhuongTien,
      );
    }
  }

  @override
  void dispose() {
    _tenXeCtrl.dispose();
    _bienSoCtrl.dispose();
    _mauXeCtrl.dispose();
    _noiDungCtrl.dispose();
    super.dispose();
  }

  bool get _isXoa => _loai == _LoaiYeuCau.xoa;
  bool get _isThem => _loai == _LoaiYeuCau.them;

  Future<void> _submit(bool isSubmit) async {
    if (!_formKey.currentState!.validate()) return;

    if (!_isXoa && _loaiPhuongTien == null) {
      _showSnack('Vui lòng chọn loại phương tiện');
      return;
    }

    if (_isXoa) {
      final ok = await AppConfirmDialog.show(
        context,
        title: 'Xác nhận huỷ đăng ký',
        message:
            'Bạn có chắc muốn gửi yêu cầu huỷ đăng ký xe '
            '"${widget.phuongTien!.bienSo}" không?',
        confirmLabel: 'Gửi yêu cầu',
        isDangerous: true,
      );
      if (ok != true || !mounted) return;
    }

    setState(() => _isSubmitting = true);

    try {
      await _ptService.taoYeuCau(
        TaoYeuCauPhuongTienRequest(
          canHoId: widget.canHoInfo.canHoId,
          loaiYeuCauId: widget.loaiYeuCauId,
          isSubmit: isSubmit,
          yeuCauPhuongTienId: widget.phuongTien?.id,
          yeuCauLoaiPhuongTienId: _loaiPhuongTien?.id,
          yeuCauTenPhuongTien: _tenXeCtrl.text.trim().nullIfEmpty,
          yeuCauBienSo: _bienSoCtrl.text.trim().nullIfEmpty,
          yeuCauMauXe: _mauXeCtrl.text.trim().nullIfEmpty,
          noiDung: _noiDungCtrl.text.trim().nullIfEmpty,
          fileIds: _uploadedFiles.isNotEmpty
              ? _uploadedFiles.map((f) => f.fileId).toList()
              : null,
        ),
      );

      if (!mounted) return;
      _showSnack(isSubmit ? 'Đã nộp yêu cầu thành công' : 'Đã lưu nháp');
      Navigator.pop(context, true);
    } on Exception catch (e) {
      if (!mounted) return;
      _showSnack(e.toString());
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  void _showSnack(String msg) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      appBar: AppTopBar(title: _loai.label),
      body: _isSubmitting
          ? const Center(
              child: CircularProgressIndicator(color: AppColors.primary),
            )
          : Form(
              key: _formKey,
              child: ListView(
                padding: AppSpacing.insetAll16,
                children: [
                  ReadonlyCanHoCard(canHoInfo: widget.canHoInfo),
                  const SizedBox(height: AppSpacing.lg),

                  if (!_isThem && widget.phuongTien != null) ...[
                    _CurrentVehicleCard(pt: widget.phuongTien!),
                    const SizedBox(height: AppSpacing.lg),
                  ],

                  if (!_isXoa) ...[
                    Text(
                      _isThem
                          ? 'Thông tin phương tiện mới'
                          : 'Thông tin cập nhật',
                      style: AppTypography.subhead,
                    ),
                    const SizedBox(height: AppSpacing.sm),

                    SelectorField.future(
                      label: 'Loại phương tiện *',
                      future: _ptService.getLoaiPhuongTienSelector(),
                      selectedItems: _loaiPhuongTien != null
                          ? [_loaiPhuongTien!]
                          : [],
                      isRequired: true,
                      onChangedSingle: (v) =>
                          setState(() => _loaiPhuongTien = v as SelectorItem),
                    ),
                    const SizedBox(height: AppSpacing.sm2),

                    Field(
                      controller: _tenXeCtrl,
                      label: 'Tên xe *',
                      hint: 'VD: Honda Wave Alpha, Toyota Vios...',
                      validator: _required,
                    ),
                    const SizedBox(height: AppSpacing.sm2),

                    Field(
                      controller: _bienSoCtrl,
                      label: 'Biển số *',
                      hint: 'VD: 51A-123.45',
                      textCapitalization: TextCapitalization.characters,
                      validator: _required,
                    ),
                    const SizedBox(height: AppSpacing.sm2),

                    Field(
                      controller: _mauXeCtrl,
                      label: 'Màu xe',
                      hint: 'VD: Đỏ, Trắng, Đen...',
                    ),
                    const SizedBox(height: AppSpacing.sm2),

                    Field(
                      controller: _noiDungCtrl,
                      label: 'Ghi chú',
                      maxLines: 3,
                    ),
                    const SizedBox(height: AppSpacing.lg),

                    Text('Hình ảnh phương tiện', style: AppTypography.subhead),
                    const SizedBox(height: AppSpacing.sm),

                    AppFileUploadField(
                      label: 'Ảnh xe (tùy chọn)',
                      targetContainer: 'tai-lieu-phuong-tien',
                      uploadFn: _ptService.uploadMedia,
                      initialFiles: _uploadedFiles,
                      allowMultiple: true,
                      onChanged: (files) => setState(() {
                        _uploadedFiles
                          ..clear()
                          ..addAll(files);
                      }),
                    ),
                    const SizedBox(height: AppSpacing.lg),
                  ],

                  if (_isXoa) ...[
                    Field(
                      controller: _noiDungCtrl,
                      label: 'Lý do huỷ đăng ký',
                      hint: 'Nhập lý do (tùy chọn)',
                      maxLines: 3,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                  ],

                  if (_isXoa)
                    AppButton(
                      label: 'Gửi yêu cầu huỷ đăng ký',
                      variant: AppButtonVariant.danger,
                      onPressed: () => _submit(true),
                    )
                  else
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

                  const SizedBox(height: AppSpacing.lg),
                ],
              ),
            ),
    );
  }

  String? _required(String? v) =>
      (v == null || v.trim().isEmpty) ? 'Trường này là bắt buộc' : null;
}

class _CurrentVehicleCard extends StatelessWidget {
  final PhuongTien pt;
  const _CurrentVehicleCard({required this.pt});

  @override
  Widget build(BuildContext context) {
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: AppColors.primaryLight,
                  borderRadius: AppRadius.buttonSmall,
                ),
                child: Icon(
                  _loaiIcon(pt.loaiPhuongTienId),
                  color: AppColors.primary,
                  size: 20,
                ),
              ),
              const SizedBox(width: AppSpacing.sm),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(pt.bienSo, style: AppTypography.subhead),
                    Text(
                      '${pt.tenLoaiPhuongTien} • ${pt.tenPhuongTien}',
                      style: AppTypography.caption.secondary,
                    ),
                  ],
                ),
              ),
              AppStatusBadge(
                label: pt.tenTrangThaiPhuongTien,
                variant: _trangThaiVariant(pt.trangThaiPhuongTienId),
              ),
            ],
          ),
          const Divider(height: AppSpacing.lg),
          _Row('Màu xe', pt.mauXe),
          _Row('Vị trí', pt.viTriNgan),
          if (pt.thePhuongTiens.isNotEmpty)
            _Row('Số thẻ', '${pt.thePhuongTiens.length} thẻ'),
        ],
      ),
    );
  }

  AppBadgeVariant _trangThaiVariant(int id) => switch (id) {
    1 => AppBadgeVariant.success,
    2 => AppBadgeVariant.info,
    _ => AppBadgeVariant.warning,
  };

  IconData _loaiIcon(int id) => switch (id) {
    1 => Icons.two_wheeler,
    2 => Icons.directions_car,
    3 => Icons.pedal_bike,
    _ => Icons.commute,
  };
}

class _Row extends StatelessWidget {
  final String label;
  final String value;
  const _Row(this.label, this.value);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        children: [
          SizedBox(
            width: 72,
            child: Text(label, style: AppTypography.caption.secondary),
          ),
          Expanded(child: Text(value, style: AppTypography.bodyMedium)),
        ],
      ),
    );
  }
}

extension _StringNullIfEmpty on String {
  String? get nullIfEmpty => trim().isEmpty ? null : trim();
}
