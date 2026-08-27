import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../../models/thanh_vien_model.dart';
import '../../services/thanh_vien_service.dart';
import '../../widgets/tv_shared_widgets.dart';

const int _kNhap = 4;
const int _kChoDuyet = 1;
const Set<int> _kCoTheRut = {_kNhap, _kChoDuyet};

class LichSuYeuCauThanhVienTab extends StatefulWidget {
  final QuanHeCuTruModel item;

  const LichSuYeuCauThanhVienTab({super.key, required this.item});

  @override
  State<LichSuYeuCauThanhVienTab> createState() =>
      LichSuYeuCauThanhVienTabState();
}

class LichSuYeuCauThanhVienTabState extends State<LichSuYeuCauThanhVienTab>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  void reload() => _loadData();

  final _service = ThanhVienService.instance;
  final _scrollCtrl = ScrollController();

  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _error;
  List<YeuCauCuTruModel> _list = [];
  int _pageNumber = 1;
  static const _pageSize = 10;
  bool _hasMore = true;

  @override
  void initState() {
    super.initState();
    _loadData();
    _scrollCtrl.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollCtrl.dispose();
    super.dispose();
  }

  void _onScroll() {
    final pos = _scrollCtrl.position;
    if (pos.pixels >= pos.maxScrollExtent - 100 &&
        _hasMore &&
        !_isLoadingMore) {
      _loadMore();
    }
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
      _error = null;
      _pageNumber = 1;
      _hasMore = true;
    });
    try {
      final result = await _service.getYeuCauList(
        GetListYeuCauCuTruRequest(
          pageNumber: 1,
          pageSize: _pageSize,
          canHoId: widget.item.canHoId,
          sortCol: 'createdAt',
          isAsc: false,
        ),
      );
      if (!mounted) return;
      setState(() {
        _list = result.items;
        _hasMore = result.pagingInfo.hasNextPage;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _loadMore() async {
    setState(() => _isLoadingMore = true);
    try {
      final nextPage = _pageNumber + 1;
      final result = await _service.getYeuCauList(
        GetListYeuCauCuTruRequest(
          pageNumber: nextPage,
          pageSize: _pageSize,
          canHoId: widget.item.canHoId,
          sortCol: 'createdAt',
          isAsc: false,
        ),
      );
      if (!mounted) return;
      setState(() {
        _list.addAll(result.items);
        _pageNumber = nextPage;
        _hasMore = result.pagingInfo.hasNextPage;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    } finally {
      if (mounted) setState(() => _isLoadingMore = false);
    }
  }

  Future<void> _withdraw(YeuCauCuTruModel yeuCau) async {
    final confirmed = await AppConfirmDialog.show(
      context,
      title: 'Xác nhận thu hồi',
      message: 'Bạn có chắc muốn thu hồi yêu cầu này không?',
      confirmLabel: 'Thu hồi',
      isDangerous: true,
    );
    if (confirmed != true || !mounted) return;

    try {
      await _service.updateYeuCau(
        CapNhatYeuCauCuTruRequest(id: yeuCau.id, isWithdraw: true),
      );
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Đã thu hồi yêu cầu')));
      _loadData();
    } on Exception catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    }
  }

  void _onTapCard(YeuCauCuTruModel yeuCau) {
    if (yeuCau.trangThaiId == _kNhap) {
      _openEditDraft(yeuCau);
    } else {
      _openDetail(yeuCau);
    }
  }

  void _openDetail(YeuCauCuTruModel yeuCau) {
    context.push('/cu-tru/thanh-vien/yc-detail/${yeuCau.id}');
  }

  Future<void> _openEditDraft(YeuCauCuTruModel yeuCau) async {
    final reload = await context.push<bool>(
      '/cu-tru/thanh-vien/yc-form',
      extra: {'mode': 'draft', 'yeuCauId': yeuCau.id},
    );
    if (reload == true && mounted) _loadData();
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);

    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_error != null) {
      return ErrorDisplay(error: _error, onRetry: _loadData);
    }

    if (_list.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.history_outlined,
              size: 56,
              color: AppColors.textDisabled,
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              'Chưa có lịch sử yêu cầu thành viên',
              style: AppTypography.body.secondary,
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadData,
      color: AppColors.primary,
      child: ListView.separated(
        controller: _scrollCtrl,
        padding: AppSpacing.insetAll16,
        itemCount: _list.length + (_isLoadingMore ? 1 : 0),
        separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.sm),
        itemBuilder: (_, i) {
          if (i == _list.length) {
            return const Center(
              child: Padding(
                padding: EdgeInsets.all(AppSpacing.md),
                child: CircularProgressIndicator(color: AppColors.primary),
              ),
            );
          }
          final yeuCau = _list[i];
          return _YeuCauCard(
            yeuCau: yeuCau,
            onTap: () => _onTapCard(yeuCau),
            onWithdraw: _kCoTheRut.contains(yeuCau.trangThaiId)
                ? () => _withdraw(yeuCau)
                : null,
          );
        },
      ),
    );
  }
}

class _YeuCauCard extends StatelessWidget {
  final YeuCauCuTruModel yeuCau;
  final VoidCallback onTap;
  final VoidCallback? onWithdraw;

  const _YeuCauCard({
    required this.yeuCau,
    required this.onTap,
    this.onWithdraw,
  });

  @override
  Widget build(BuildContext context) {
    final isNhap = yeuCau.trangThaiId == _kNhap;

    return AppCard(
      onTap: onTap,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: AppColors.primaryLight,
              borderRadius: AppRadius.buttonSmall,
            ),
            child: Icon(
              tvLoaiYeuCauIcon(yeuCau.loaiYeuCauId),
              size: 18,
              color: AppColors.primary,
            ),
          ),
          const SizedBox(width: AppSpacing.sm),

          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Text(
                        yeuCau.tenLoaiYeuCau,
                        style: AppTypography.subhead,
                      ),
                    ),
                    const SizedBox(width: AppSpacing.sm),
                    AppStatusBadge(
                      label: isNhap
                          ? '${yeuCau.tenTrangThai} (nháp)'
                          : yeuCau.tenTrangThai,
                      variant: tvTrangThaiVariant(yeuCau.trangThaiId),
                    ),
                  ],
                ),
                const SizedBox(height: 4),

                if (yeuCau.hoTenDayDu != null)
                  _IconRow(
                    icon: Icons.person_outline,
                    text: 'Đối tượng: ${yeuCau.hoTenDayDu}',
                  ),
                _IconRow(
                  icon: Icons.send_outlined,
                  text: 'Người gửi: ${yeuCau.tenNguoiGui}',
                ),
                if (yeuCau.createdAt != null)
                  _IconRow(
                    icon: Icons.calendar_today_outlined,
                    text: 'Ngày tạo: ${yeuCau.createdAt!.tvFormatted}',
                    muted: true,
                  ),

                if (onWithdraw != null) ...[
                  const SizedBox(height: AppSpacing.sm),
                  Align(
                    alignment: Alignment.centerRight,
                    child: AppButton(
                      label: 'Thu hồi',
                      variant: AppButtonVariant.danger,
                      expanded: false,
                      height: 32,
                      leadingIcon: Icons.undo,
                      onPressed: onWithdraw,
                    ),
                  ),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _IconRow extends StatelessWidget {
  final IconData icon;
  final String text;
  final bool muted;

  const _IconRow({required this.icon, required this.text, this.muted = false});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 2),
      child: Row(
        children: [
          Icon(
            icon,
            size: 13,
            color: muted ? AppColors.textDisabled : AppColors.textSecondary,
          ),
          const SizedBox(width: 4),
          Expanded(
            child: Text(
              text,
              style: muted
                  ? AppTypography.captionSmall.secondary
                  : AppTypography.caption.secondary,
            ),
          ),
        ],
      ),
    );
  }
}
