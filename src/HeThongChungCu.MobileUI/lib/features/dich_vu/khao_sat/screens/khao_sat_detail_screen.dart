import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/cu_tru/quan_he/widgets/can_ho_selector.dart';

import '../models/khao_sat_model.dart';
import '../services/khao_sat_service.dart';

class KhaoSatDetailScreen extends StatefulWidget {
  final int khaoSatId;

  final QuanHeCuTruModel selectedCanHo;

  final List<QuanHeCuTruModel> dsCanHo;

  const KhaoSatDetailScreen({
    super.key,
    required this.khaoSatId,
    required this.selectedCanHo,
    required this.dsCanHo,
  });

  static Widget fromRoute(BuildContext context, GoRouterState state) {
    final e = state.extra! as Map<String, dynamic>;
    return KhaoSatDetailScreen(
      khaoSatId: e['khaoSatId'] as int,
      selectedCanHo: e['selectedCanHo'] as QuanHeCuTruModel,
      dsCanHo: e['dsCanHo'] as List<QuanHeCuTruModel>,
    );
  }

  @override
  State<KhaoSatDetailScreen> createState() => _KhaoSatDetailScreenState();
}

class _KhaoSatDetailScreenState extends State<KhaoSatDetailScreen> {
  final _service = KhaoSatService.instance;

  late QuanHeCuTruModel _activeCanHo;

  KhaoSatDetailResponse? _detail;
  bool _isLoading = true;
  String? _loadError;

  final Map<int, Set<int>> _selectedAnswers = {};
  bool _isSendingOtp = false;
  bool _isSubmitting = false;
  bool _otpSent = false;
  final _otpController = TextEditingController();

  int get _canHoId => _activeCanHo.canHoId;

  int get _nguoiDungId => int.parse(_service.getNguoiDungID()!);

  @override
  void initState() {
    super.initState();
    _activeCanHo = widget.selectedCanHo;
    _loadDetail();
  }

  @override
  void dispose() {
    _otpController.dispose();
    super.dispose();
  }

  void _onCanHoChanged(QuanHeCuTruModel canHo) {
    setState(() {
      _activeCanHo = canHo;
      _selectedAnswers.clear();
      _otpSent = false;
      _otpController.clear();
    });
    _loadDetail();
  }

  Future<void> _loadDetail() async {
    setState(() {
      _isLoading = true;
      _loadError = null;
    });
    try {
      final detail = await _service.getById(widget.khaoSatId);
      if (!mounted) return;
      setState(() {
        _detail = detail;
        _isLoading = false;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() {
        _loadError = e.toString();
        _isLoading = false;
      });
    }
  }

  Future<void> _sendOtp() async {
    setState(() => _isSendingOtp = true);
    try {
      final msg = await _service.guiOtpBieuQuyet(
        khaoSatId: widget.khaoSatId,
        canHoId: _canHoId,
        nguoiDungId: _nguoiDungId,
      );
      if (!mounted) return;
      setState(() {
        _otpSent = true;
        _isSendingOtp = false;
      });
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _isSendingOtp = false);
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    }
  }

  Future<void> _submitVote() async {
    final detail = _detail;
    if (detail == null) return;

    for (final cau in detail.cauHois) {
      if (cau.isBatBuoc && (_selectedAnswers[cau.id]?.isEmpty ?? true)) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Vui lòng trả lời: "${cau.noiDungCauHoi}"')),
        );
        return;
      }
    }

    final otp = _otpController.text.trim();
    if (otp.isEmpty) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Vui lòng nhập mã OTP')));
      return;
    }

    final traLois = <TraLoiRequest>[];
    for (final entry in _selectedAnswers.entries) {
      for (final luaChonId in entry.value) {
        traLois.add(TraLoiRequest(luaChonId: luaChonId));
      }
    }

    setState(() => _isSubmitting = true);
    try {
      await _service.xacNhanBieuQuyet(
        XacNhanBieuQuyetRequest(
          khaoSatId: widget.khaoSatId,
          canHoId: _canHoId,
          otpCode: otp,
          traLois: traLois,
        ),
      );
      if (!mounted) return;
      setState(() => _isSubmitting = false);
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Bỏ phiếu thành công!')));
      await _loadDetail();
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _isSubmitting = false);
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    }
  }

  void _toggleAnswer(int cauHoiId, int luaChonId, bool isMultiSelect) {
    setState(() {
      final set = _selectedAnswers.putIfAbsent(cauHoiId, () => {});
      if (isMultiSelect) {
        set.contains(luaChonId) ? set.remove(luaChonId) : set.add(luaChonId);
      } else {
        set
          ..clear()
          ..add(luaChonId);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppTopBar(title: 'Chi tiết khảo sát'),
      body: Column(
        children: [
          CanHoSelector(
            dsCanHo: widget.dsCanHo,
            selected: _activeCanHo,
            onChanged: _onCanHoChanged,
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
    if (_loadError != null) {
      return ErrorDisplay.fullScreen(error: _loadError!, onRetry: _loadDetail);
    }
    final detail = _detail!;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        _HeaderCard(detail: detail),
        const SizedBox(height: 16),

        if (detail.isVoted) ...[
          _VotedBanner(
            canHoTen: _activeCanHo.tenCanHo,
            onViewResult: () =>
                context.push('/dich-vu/khao-sat/ket-qua/${widget.khaoSatId}'),
          ),
          const SizedBox(height: 16),
        ],

        if (detail.trangThaiId == KhaoSatTrangThai.closed &&
            !detail.isVoted) ...[
          _ClosedBanner(
            onViewResult: () =>
                context.push('/dich-vu/khao-sat/ket-qua/${widget.khaoSatId}'),
          ),
          const SizedBox(height: 16),
        ],

        ...detail.cauHois.asMap().entries.map(
          (entry) => Padding(
            padding: const EdgeInsets.only(bottom: 12),
            child: _CauHoiCard(
              index: entry.key + 1,
              cauHoi: entry.value,
              selectedIds: _selectedAnswers[entry.value.id] ?? {},
              onToggle: detail.canVote
                  ? (luaChonId) => _toggleAnswer(
                      entry.value.id,
                      luaChonId,
                      entry.value.isMultiSelect,
                    )
                  : null,
            ),
          ),
        ),

        if (detail.canVote) ...[
          const SizedBox(height: 4),
          _OtpSection(
            canHoTen: _activeCanHo.tenCanHo,
            otpController: _otpController,
            otpSent: _otpSent,
            isSendingOtp: _isSendingOtp,
            isSubmitting: _isSubmitting,
            onSendOtp: _sendOtp,
            onSubmit: _submitVote,
          ),
          const SizedBox(height: 24),
        ],
      ],
    );
  }
}

class _HeaderCard extends StatelessWidget {
  final KhaoSatDetailResponse detail;
  const _HeaderCard({required this.detail});

  @override
  Widget build(BuildContext context) {
    final fmt = DateFormat('dd/MM/yyyy HH:mm');
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(detail.tieuDe, style: AppTypography.headline),
          const SizedBox(height: 6),
          Text(
            detail.moTa,
            style: AppTypography.body.copyWith(color: AppColors.textSecondary),
          ),
          const Divider(height: 20),
          _Row(
            icon: Icons.category_outlined,
            label: 'Loại',
            value: detail.loaiKhaoSatTen,
          ),
          const SizedBox(height: 6),
          _Row(
            icon: Icons.calculate_outlined,
            label: 'Cơ chế',
            value: detail.coCheTinhDiemTen,
          ),
          const SizedBox(height: 6),
          _Row(
            icon: Icons.calendar_today_outlined,
            label: 'Từ',
            value: fmt.format(detail.ngayBatDau),
          ),
          const SizedBox(height: 6),
          _Row(
            icon: Icons.event_outlined,
            label: 'Đến',
            value: fmt.format(detail.ngayKetThuc),
          ),
          const SizedBox(height: 6),
          _Row(
            icon: Icons.people_outline,
            label: 'Tham gia tối thiểu',
            value: '${detail.tyleThamGiaToiThieu.toInt()}%',
          ),
          const SizedBox(height: 6),
          _Row(
            icon: Icons.thumb_up_outlined,
            label: 'Đồng ý tối thiểu',
            value: '${detail.tyLeDongYToiThieu.toInt()}%',
          ),
        ],
      ),
    );
  }
}

class _Row extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;
  const _Row({required this.icon, required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(icon, size: 15, color: AppColors.textSecondary),
        const SizedBox(width: 6),
        Text(
          '$label: ',
          style: AppTypography.captionSmall.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
        Expanded(
          child: Text(
            value,
            style: AppTypography.captionSmall.copyWith(
              color: AppColors.textPrimary,
            ),
          ),
        ),
      ],
    );
  }
}

class _VotedBanner extends StatelessWidget {
  final String canHoTen;
  final VoidCallback onViewResult;
  const _VotedBanner({required this.canHoTen, required this.onViewResult});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.successLight,
        borderRadius: AppRadius.card,
        border: Border.all(color: AppColors.success.withAlpha(80)),
      ),
      child: Row(
        children: [
          const Icon(Icons.check_circle, color: AppColors.success, size: 22),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              'Căn hộ $canHoTen đã hoàn thành bỏ phiếu.',
              style: AppTypography.body.copyWith(color: AppColors.success),
            ),
          ),
          TextButton(
            onPressed: onViewResult,
            child: Text(
              'Xem kết quả',
              style: AppTypography.buttonLabel.copyWith(
                color: AppColors.primary,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _ClosedBanner extends StatelessWidget {
  final VoidCallback onViewResult;
  const _ClosedBanner({required this.onViewResult});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.secondaryLight,
        borderRadius: AppRadius.card,
        border: Border.all(color: AppColors.border),
      ),
      child: Row(
        children: [
          const Icon(Icons.lock_outline, color: AppColors.secondary, size: 22),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              'Đợt khảo sát đã kết thúc.',
              style: AppTypography.body.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ),
          TextButton(
            onPressed: onViewResult,
            child: Text(
              'Xem kết quả',
              style: AppTypography.buttonLabel.copyWith(
                color: AppColors.primary,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _CauHoiCard extends StatelessWidget {
  final int index;
  final CauHoiModel cauHoi;
  final Set<int> selectedIds;
  final void Function(int luaChonId)? onToggle;

  const _CauHoiCard({
    required this.index,
    required this.cauHoi,
    required this.selectedIds,
    this.onToggle,
  });

  @override
  Widget build(BuildContext context) {
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 24,
                height: 24,
                decoration: const BoxDecoration(
                  color: AppColors.primaryLight,
                  shape: BoxShape.circle,
                ),
                child: Center(
                  child: Text(
                    '$index',
                    style: AppTypography.captionSmall.copyWith(
                      color: AppColors.primary,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(cauHoi.noiDungCauHoi, style: AppTypography.subhead),
                    if (cauHoi.isBatBuoc || cauHoi.isMultiSelect)
                      Wrap(
                        spacing: 6,
                        children: [
                          if (cauHoi.isBatBuoc)
                            Text(
                              '* Bắt buộc',
                              style: AppTypography.captionSmall.copyWith(
                                color: AppColors.error,
                              ),
                            ),
                          if (cauHoi.isMultiSelect)
                            Text(
                              '(Chọn nhiều)',
                              style: AppTypography.captionSmall.copyWith(
                                color: AppColors.textSecondary,
                              ),
                            ),
                        ],
                      ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          ...cauHoi.luaChons.map(
            (lc) => _LuaChonTile(
              luaChon: lc,
              isSelected: selectedIds.contains(lc.id),
              isMultiSelect: cauHoi.isMultiSelect,
              onTap: onToggle != null ? () => onToggle!(lc.id) : null,
            ),
          ),
        ],
      ),
    );
  }
}

class _LuaChonTile extends StatelessWidget {
  final LuaChonModel luaChon;
  final bool isSelected;
  final bool isMultiSelect;
  final VoidCallback? onTap;

  const _LuaChonTile({
    required this.luaChon,
    required this.isSelected,
    required this.isMultiSelect,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final borderColor = isSelected ? AppColors.primary : AppColors.border;
    final bgColor = isSelected ? AppColors.primaryLight : AppColors.surface;

    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: AppConstants.animFast,
        margin: const EdgeInsets.only(bottom: 8),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
        decoration: BoxDecoration(
          color: bgColor,
          borderRadius: AppRadius.inputField,
          border: Border.all(color: borderColor, width: 1.5),
        ),
        child: Row(
          children: [
            Icon(
              isMultiSelect
                  ? (isSelected
                        ? Icons.check_box
                        : Icons.check_box_outline_blank)
                  : (isSelected
                        ? Icons.radio_button_checked
                        : Icons.radio_button_unchecked),
              color: isSelected ? AppColors.primary : AppColors.textDisabled,
              size: 20,
            ),
            const SizedBox(width: 10),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    luaChon.noiDungLuaChon,
                    style: AppTypography.body.copyWith(
                      color: isSelected
                          ? AppColors.primary
                          : AppColors.textPrimary,
                      fontWeight: isSelected
                          ? FontWeight.w600
                          : FontWeight.w400,
                    ),
                  ),
                  if (luaChon.isUngVienBQT &&
                      (luaChon.tieuSuUngVien?.isNotEmpty ?? false)) ...[
                    const SizedBox(height: 4),
                    Text(
                      luaChon.tieuSuUngVien!,
                      style: AppTypography.captionSmall.copyWith(
                        color: AppColors.textSecondary,
                      ),
                      maxLines: 3,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _OtpSection extends StatelessWidget {
  final String canHoTen;
  final TextEditingController otpController;
  final bool otpSent;
  final bool isSendingOtp;
  final bool isSubmitting;
  final VoidCallback onSendOtp;
  final VoidCallback onSubmit;

  const _OtpSection({
    required this.canHoTen,
    required this.otpController,
    required this.otpSent,
    required this.isSendingOtp,
    required this.isSubmitting,
    required this.onSendOtp,
    required this.onSubmit,
  });

  @override
  Widget build(BuildContext context) {
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(
                Icons.security_outlined,
                color: AppColors.primary,
                size: 20,
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Xác thực — $canHoTen',
                  style: AppTypography.subhead.copyWith(
                    color: AppColors.primary,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          Text(
            'Mã OTP gửi đến email đăng ký của căn hộ. Hiệu lực 5 phút.',
            style: AppTypography.captionSmall.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: 14),

          AppButton(
            label: otpSent ? 'Gửi lại mã OTP' : 'Nhận mã OTP qua Email',
            variant: otpSent
                ? AppButtonVariant.outline
                : AppButtonVariant.secondary,
            isLoading: isSendingOtp,
            leadingIcon: Icons.email_outlined,
            onPressed: isSendingOtp ? null : onSendOtp,
          ),

          if (otpSent) ...[
            const SizedBox(height: 14),
            AppTextField(
              label: 'Mã OTP (6 chữ số)',
              hint: 'Nhập mã OTP từ email',
              controller: otpController,
              keyboardType: TextInputType.number,
              maxLength: 6,
              prefixIcon: const Icon(
                Icons.pin_outlined,
                color: AppColors.textSecondary,
                size: 20,
              ),
            ),
            const SizedBox(height: 14),
            AppButton(
              label: 'Xác nhận & Nộp phiếu bầu',
              isLoading: isSubmitting,
              leadingIcon: Icons.how_to_vote_outlined,
              onPressed: isSubmitting ? null : onSubmit,
            ),
          ],
        ],
      ),
    );
  }
}
