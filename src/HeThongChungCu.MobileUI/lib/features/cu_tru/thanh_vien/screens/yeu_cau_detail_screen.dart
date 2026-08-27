import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';

import 'package:klks_app/design/design.dart';

import '../models/thanh_vien_model.dart';
import '../services/thanh_vien_service.dart';
import '../widgets/tv_shared_widgets.dart';

class YeuCauThanhVienDetailScreen extends StatefulWidget {
  final int yeuCauId;

  const YeuCauThanhVienDetailScreen({super.key, required this.yeuCauId});

  @override
  State<YeuCauThanhVienDetailScreen> createState() =>
      _YeuCauThanhVienDetailScreenState();
}

class _YeuCauThanhVienDetailScreenState
    extends State<YeuCauThanhVienDetailScreen> {
  final _service = ThanhVienService.instance;

  bool _isLoading = true;
  String? _error;
  YeuCauCuTruModel? _data;

  @override
  void initState() {
    super.initState();
    _loadDetail();
  }

  Future<void> _loadDetail() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final result = await _service.getYeuCauById(widget.yeuCauId);
      if (!mounted) return;
      setState(() => _data = result);
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      appBar: AppTopBar(
        title: 'Chi tiết yêu cầu',
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Tải lại',
            onPressed: _isLoading ? null : _loadDetail,
          ),
        ],
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(color: AppColors.primary),
      );
    }

    if (_error != null) {
      return ErrorDisplay(error: _error, onRetry: _loadDetail);
    }

    if (_data == null) return const SizedBox.shrink();

    final d = _data!;

    return RefreshIndicator(
      onRefresh: _loadDetail,
      color: AppColors.primary,
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: AppSpacing.insetAll16,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            TvStatusBanner(
              trangThaiId: d.trangThaiId,
              tenTrangThai: d.tenTrangThai,
            ),
            const SizedBox(height: AppSpacing.md),

            TvSectionCard(
              title: 'Thông tin yêu cầu',
              children: [
                TvInfoRow(
                  label: 'Loại yêu cầu',
                  value: d.tenLoaiYeuCau,
                  labelWidth: 130,
                ),
                TvInfoRow(
                  label: 'Căn hộ',
                  value: d.diaChiCanHo,
                  labelWidth: 130,
                ),
                TvInfoRow(
                  label: 'Người gửi',
                  value: d.tenNguoiGui,
                  labelWidth: 130,
                ),
                if (d.createdAt != null)
                  TvInfoRow(
                    label: 'Ngày tạo',
                    value: d.createdAt!.tvFormatted,
                    labelWidth: 130,
                  ),
                if (d.tenNguoiXuLy != null)
                  TvInfoRow(
                    label: 'Người xử lý',
                    value: d.tenNguoiXuLy!,
                    labelWidth: 130,
                  ),
                if (d.ngayXuLy != null)
                  TvInfoRow(
                    label: 'Ngày xử lý',
                    value: d.ngayXuLy!.tvFormatted,
                    labelWidth: 130,
                  ),
                if (d.lyDo != null && d.lyDo!.isNotEmpty)
                  TvInfoRow(
                    label: 'Lý do từ chối',
                    value: d.lyDo!,
                    highlight: true,
                    labelWidth: 130,
                  ),
              ],
            ),
            const SizedBox(height: AppSpacing.sm),

            if (_hasPersonInfo(d)) ...[
              TvSectionCard(
                title: 'Thông tin người được yêu cầu',
                children: [
                  if (d.hoTenDayDu != null)
                    TvInfoRow(label: 'Họ tên', value: d.hoTenDayDu!),
                  if (d.yeuCauNgaySinh != null)
                    TvInfoRow(
                      label: 'Ngày sinh',
                      value: d.yeuCauNgaySinh!.tvFormatted,
                    ),
                  if (d.yeuCauGioiTinhTen != null)
                    TvInfoRow(label: 'Giới tính', value: d.yeuCauGioiTinhTen!),
                  if (d.yeuCauCCCD != null && d.yeuCauCCCD!.isNotEmpty)
                    TvInfoRow(label: 'CCCD', value: d.yeuCauCCCD!),
                  if (d.yeuCauSoDienThoai != null &&
                      d.yeuCauSoDienThoai!.isNotEmpty)
                    TvInfoRow(label: 'SĐT', value: d.yeuCauSoDienThoai!),
                  if (d.yeuCauDiaChi != null && d.yeuCauDiaChi!.isNotEmpty)
                    TvInfoRow(label: 'Địa chỉ', value: d.yeuCauDiaChi!),
                  if (d.yeuCauLoaiQuanHeTen != null)
                    TvInfoRow(
                      label: 'Quan hệ cư trú',
                      value: d.yeuCauLoaiQuanHeTen!,
                    ),
                ],
              ),
              const SizedBox(height: AppSpacing.sm),
            ],

            if (d.noiDung != null && d.noiDung!.isNotEmpty) ...[
              TvSectionCard(
                title: 'Nội dung',
                children: [Text(d.noiDung!, style: AppTypography.body)],
              ),
              const SizedBox(height: AppSpacing.sm),
            ],

            if (d.documents.isNotEmpty) ...[
              TvSectionCard(
                title: 'Tài liệu đính kèm (${d.documents.length})',
                children: d.documents
                    .map((doc) => _DocumentItem(doc: doc))
                    .toList(),
              ),
            ],

            const SizedBox(height: AppSpacing.lg),
          ],
        ),
      ),
    );
  }

  bool _hasPersonInfo(YeuCauCuTruModel d) =>
      d.hoTenDayDu != null ||
      d.yeuCauNgaySinh != null ||
      d.yeuCauGioiTinhTen != null ||
      (d.yeuCauCCCD != null && d.yeuCauCCCD!.isNotEmpty) ||
      (d.yeuCauSoDienThoai != null && d.yeuCauSoDienThoai!.isNotEmpty) ||
      (d.yeuCauDiaChi != null && d.yeuCauDiaChi!.isNotEmpty) ||
      d.yeuCauLoaiQuanHeTen != null;
}

class _DocumentItem extends StatelessWidget {
  final TaiLieuCuTruModel doc;
  const _DocumentItem({required this.doc});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            const Icon(
              Icons.article_outlined,
              size: 16,
              color: AppColors.textSecondary,
            ),
            const SizedBox(width: 6),
            Expanded(
              child: Text(
                doc.tenLoaiGiayTo.isNotEmpty ? doc.tenLoaiGiayTo : 'Tài liệu',
                style: AppTypography.subhead,
              ),
            ),
          ],
        ),
        if (doc.soGiayTo.isNotEmpty)
          Padding(
            padding: const EdgeInsets.only(left: 22, top: 2),
            child: Text(
              'Số: ${doc.soGiayTo}',
              style: AppTypography.captionSmall.secondary,
            ),
          ),
        if (doc.ngayPhatHanh != null)
          Padding(
            padding: const EdgeInsets.only(left: 22, top: 2),
            child: Text(
              'Ngày phát hành: ${doc.ngayPhatHanh!.tvFormatted}',
              style: AppTypography.captionSmall.secondary,
            ),
          ),
        if (doc.files.isNotEmpty)
          Padding(
            padding: const EdgeInsets.only(left: 22, top: 6),
            child: Wrap(
              spacing: 8,
              runSpacing: 8,
              children: doc.files.map((f) => _FileChip(file: f)).toList(),
            ),
          ),
        const Divider(height: AppSpacing.lg),
      ],
    );
  }
}

class _FileChip extends StatelessWidget {
  final FileAttachment file;
  const _FileChip({required this.file});

  Future<void> _openFile(BuildContext context) async {
    if (file.fileUrl.isEmpty) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Không có đường dẫn tệp')));
      return;
    }
    final uri = Uri.tryParse(file.fileUrl);
    if (uri == null) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Đường dẫn không hợp lệ')));
      return;
    }
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri, mode: LaunchMode.externalApplication);
    } else if (context.mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không thể mở tệp trên thiết bị này')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return ActionChip(
      avatar: Icon(
        file.isImage ? Icons.image_outlined : Icons.picture_as_pdf_outlined,
        size: 16,
        color: AppColors.primary,
      ),
      label: Text(
        file.fileName,
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
        style: AppTypography.captionSmall,
      ),
      onPressed: () => _openFile(context),
    );
  }
}
