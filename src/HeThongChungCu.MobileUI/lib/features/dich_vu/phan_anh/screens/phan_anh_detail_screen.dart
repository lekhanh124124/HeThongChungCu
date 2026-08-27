import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import 'package:url_launcher/url_launcher.dart';

import '../models/phan_anh_model.dart';
import '../services/phan_anh_service.dart';

class PhanAnhDetailScreen extends StatefulWidget {
  final int phanAnhId;

  const PhanAnhDetailScreen({super.key, required this.phanAnhId});

  @override
  State<PhanAnhDetailScreen> createState() => _PhanAnhDetailScreenState();
}

class _PhanAnhDetailScreenState extends State<PhanAnhDetailScreen> {
  final _service = PhanAnhService.instance;
  final _chatCtrl = TextEditingController();
  final _scrollCtrl = ScrollController();

  static const _maxChatLength = 500;

  PhanAnhDetailResponse? _detail;
  bool _isLoading = true;
  bool _isSending = false;
  String? _errorMsg;

  @override
  void initState() {
    super.initState();
    _fetchDetail();
  }

  @override
  void dispose() {
    _chatCtrl.dispose();
    _scrollCtrl.dispose();
    super.dispose();
  }

  Future<void> _fetchDetail() async {
    setState(() {
      _isLoading = true;
      _errorMsg = null;
    });
    try {
      final detail = await _service.getById(widget.phanAnhId);
      setState(() => _detail = detail);
      WidgetsBinding.instance.addPostFrameCallback((_) => _scrollToBottom());
    } catch (e) {
      setState(() => _errorMsg = 'Đã xảy ra lỗi khi tải thông tin phản ánh.');
    } finally {
      setState(() => _isLoading = false);
    }
  }

  void _scrollToBottom() {
    if (_scrollCtrl.hasClients) {
      _scrollCtrl.animateTo(
        _scrollCtrl.position.maxScrollExtent,
        duration: const Duration(milliseconds: 300),
        curve: Curves.easeOut,
      );
    }
  }

  Future<void> _sendChat() async {
    final text = _chatCtrl.text.trim();
    if (text.isEmpty) return;

    if (text.length > _maxChatLength) {
      _showError('Tin nhắn không được vượt quá $_maxChatLength ký tự.');
      return;
    }

    setState(() => _isSending = true);
    try {
      await _service.submitTraLoi(phanAnhId: widget.phanAnhId, noiDung: text);
      _chatCtrl.clear();
      await _fetchDetail();
    } catch (e) {
      _showError('Đã xảy ra lỗi khi gửi tin nhắn.');
    } finally {
      if (mounted) setState(() => _isSending = false);
    }
  }

  Future<void> _withdraw() async {
    final confirmed = await _confirmDialog(
      title: 'Thu hồi phản ánh',
      content:
          'Phản ánh sẽ chuyển về trạng thái "Đã thu hồi". Bạn có thể chỉnh sửa và gửi lại sau.',
    );
    if (!confirmed) return;

    setState(() => _isSending = true);
    try {
      await _service.withdraw(widget.phanAnhId);
      _showSuccess('Đã thu hồi phản ánh.');
      await _fetchDetail();
    } catch (e) {
      _showError('Đã xảy ra lỗi khi thu hồi phản ánh.');
    } finally {
      if (mounted) setState(() => _isSending = false);
    }
  }

  Future<void> _openEdit() async {
    if (_detail == null) return;
    final updated = await context.push<bool>(
      '/dich-vu/phan-anh/create',
      extra: {'existing': _detail},
    );
    if (updated == true && mounted) await _fetchDetail();
  }

  Future<void> _openRatingDialog() async {
    if (_detail == null) return;
    await showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => _RatingDialog(
        phanAnhId: widget.phanAnhId,
        service: _service,
        onDone: () {
          Navigator.pop(context);
          _fetchDetail();
        },
      ),
    );
  }

  Future<void> _openAttachment(FileAttachment tep) async {
    final raw = tep.fileUrl.trim();
    if (raw.isEmpty) {
      _showError('Đường dẫn file không hợp lệ.');
      return;
    }

    final uri = Uri.tryParse(raw);
    if (uri == null || !uri.hasScheme) {
      _showError(
        'Không thể mở file này trực tiếp (đường dẫn tương đối).\nURL: $raw',
      );
      return;
    }

    if (!await launchUrl(uri, mode: LaunchMode.externalApplication)) {
      _showError('Không thể mở file: ${tep.fileName}');
    }
  }

  Future<bool> _confirmDialog({
    required String title,
    required String content,
  }) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(title),
        content: Text(content),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Hủy'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Xác nhận'),
          ),
        ],
      ),
    );
    return result ?? false;
  }

  void _showError(String msg) {
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(msg), backgroundColor: Colors.red));
  }

  void _showSuccess(String msg) {
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(msg), backgroundColor: Colors.green));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(
          _detail?.tieuDe ?? 'Chi tiết phản ánh',
          overflow: TextOverflow.ellipsis,
        ),
        actions: _buildActions(),
      ),
      body: _buildBody(),
    );
  }

  List<Widget> _buildActions() {
    if (_detail == null) return [];
    final status = _detail!.trangThaiPhanAnhId;
    return [
      if (status == 1)
        TextButton(
          onPressed: _isSending ? null : _withdraw,
          child: const Text('Thu hồi', style: TextStyle(color: Colors.white)),
        ),
      if (status == 8 || status == 9)
        IconButton(
          icon: const Icon(Icons.edit_outlined),
          tooltip: 'Chỉnh sửa',
          onPressed: _isSending ? null : _openEdit,
        ),
      if (status == 5)
        TextButton(
          onPressed: _isSending ? null : _openRatingDialog,
          child: const Text('Đánh giá', style: TextStyle(color: Colors.white)),
        ),
    ];
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_errorMsg != null) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.error_outline, color: Colors.red, size: 48),
              const SizedBox(height: 12),
              Text(
                _errorMsg!,
                textAlign: TextAlign.center,
                style: const TextStyle(color: Colors.red),
              ),
              const SizedBox(height: 16),
              ElevatedButton.icon(
                onPressed: _fetchDetail,
                icon: const Icon(Icons.refresh),
                label: const Text('Thử lại'),
              ),
            ],
          ),
        ),
      );
    }

    final d = _detail!;
    final fmt = DateFormat('dd/MM/yyyy HH:mm');
    final canChat = ![6, 7, 8].contains(d.trangThaiPhanAnhId);

    return Column(
      children: [
        Expanded(
          child: ListView(
            controller: _scrollCtrl,
            padding: const EdgeInsets.all(16),
            children: [
              Card(
                child: Padding(
                  padding: const EdgeInsets.all(14),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      _InfoRow('Căn hộ', d.tenCanHo),
                      _InfoRow('Loại', d.loaiPhanAnhTen),
                      _InfoRow('Trạng thái', d.trangThaiPhanAnhTen),
                      if (d.tenNguoiXuLy != null)
                        _InfoRow('Người xử lý', d.tenNguoiXuLy!),
                      _InfoRow('Ngày tạo', fmt.format(d.createdAt.toLocal())),
                      _InfoRow('Người gửi', d.tenNguoiGui),
                      if (d.noiDung != null && d.noiDung!.isNotEmpty) ...[
                        const Divider(height: 20),
                        Text(
                          d.noiDung!,
                          style: TextStyle(color: Colors.grey[800]),
                        ),
                      ],
                    ],
                  ),
                ),
              ),

              if (d.trangThaiPhanAnhId == 6 && d.diemDanhGia != null) ...[
                const SizedBox(height: 12),
                Card(
                  color: const Color(0xFFE6FFED),
                  child: Padding(
                    padding: const EdgeInsets.all(14),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          'Đánh giá của cư dân',
                          style: TextStyle(fontWeight: FontWeight.bold),
                        ),
                        const SizedBox(height: 6),
                        Row(
                          children: List.generate(
                            5,
                            (i) => Icon(
                              i < d.diemDanhGia!
                                  ? Icons.star
                                  : Icons.star_border,
                              color: Colors.amber,
                              size: 20,
                            ),
                          ),
                        ),
                        if (d.nhanXetDanhGia != null &&
                            d.nhanXetDanhGia!.isNotEmpty)
                          Padding(
                            padding: const EdgeInsets.only(top: 6),
                            child: Text(d.nhanXetDanhGia!),
                          ),
                        if (d.ngayDanhGia != null)
                          Padding(
                            padding: const EdgeInsets.only(top: 4),
                            child: Text(
                              fmt.format(d.ngayDanhGia!.toLocal()),
                              style: TextStyle(
                                fontSize: 11,
                                color: Colors.grey[600],
                              ),
                            ),
                          ),
                      ],
                    ),
                  ),
                ),
              ],

              if (d.danhSachTep.isNotEmpty) ...[
                const SizedBox(height: 12),
                const Text(
                  'Tệp đính kèm',
                  style: TextStyle(fontWeight: FontWeight.w600),
                ),
                const SizedBox(height: 4),
                ...d.danhSachTep.map(
                  (f) =>
                      _AttachmentTile(tep: f, onTap: () => _openAttachment(f)),
                ),
              ],

              if (d.traLoiPhanAnhs.isNotEmpty) ...[
                const SizedBox(height: 16),
                const Text(
                  'Trao đổi',
                  style: TextStyle(fontWeight: FontWeight.w600),
                ),
                const SizedBox(height: 8),
                ...d.traLoiPhanAnhs.map((t) => _ChatBubble(traLoi: t)),
              ],

              const SizedBox(height: 8),
            ],
          ),
        ),

        if (canChat)
          _ChatInput(
            controller: _chatCtrl,
            isSending: _isSending,
            maxLength: _maxChatLength,
            onSend: _sendChat,
          ),
      ],
    );
  }
}

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;

  const _InfoRow(this.label, this.value);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 100,
            child: Text(
              label,
              style: TextStyle(color: Colors.grey[600], fontSize: 13),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w500, fontSize: 13),
            ),
          ),
        ],
      ),
    );
  }
}

class _AttachmentTile extends StatelessWidget {
  final FileAttachment tep;
  final VoidCallback onTap;

  const _AttachmentTile({required this.tep, required this.onTap});

  IconData _iconForFile(String fileName) {
    final ext = fileName.split('.').last.toLowerCase();
    switch (ext) {
      case 'jpg':
      case 'jpeg':
      case 'png':
      case 'gif':
      case 'webp':
        return Icons.image_outlined;
      case 'pdf':
        return Icons.picture_as_pdf_outlined;
      case 'doc':
      case 'docx':
        return Icons.description_outlined;
      case 'xls':
      case 'xlsx':
        return Icons.table_chart_outlined;
      case 'mp4':
      case 'mov':
      case 'avi':
        return Icons.video_file_outlined;
      default:
        return Icons.attach_file;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 6),
      child: ListTile(
        dense: true,
        leading: Icon(_iconForFile(tep.fileName), color: Colors.blue[600]),
        title: Text(
          tep.fileName,
          style: const TextStyle(fontSize: 13),
          overflow: TextOverflow.ellipsis,
        ),
        subtitle: Text(tep.contentType, style: const TextStyle(fontSize: 11)),
        trailing: const Icon(Icons.open_in_new, size: 18),
        onTap: onTap,
      ),
    );
  }
}

class _ChatBubble extends StatelessWidget {
  final TraLoiPhanAnh traLoi;

  const _ChatBubble({required this.traLoi});

  @override
  Widget build(BuildContext context) {
    final isStaff = traLoi.isNhanVien;
    final fmt = DateFormat('HH:mm dd/MM');
    return Align(
      alignment: isStaff ? Alignment.centerLeft : Alignment.centerRight,
      child: Container(
        margin: const EdgeInsets.only(bottom: 8),
        constraints: BoxConstraints(
          maxWidth: MediaQuery.of(context).size.width * 0.75,
        ),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        decoration: BoxDecoration(
          color: isStaff ? const Color(0xFFEBF8FF) : const Color(0xFFEBF8F0),
          borderRadius: BorderRadius.only(
            topLeft: const Radius.circular(16),
            topRight: const Radius.circular(16),
            bottomLeft: Radius.circular(isStaff ? 4 : 16),
            bottomRight: Radius.circular(isStaff ? 16 : 4),
          ),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              traLoi.tenNguoiGui,
              style: TextStyle(
                fontSize: 11,
                fontWeight: FontWeight.bold,
                color: isStaff ? Colors.blue[700] : Colors.green[700],
              ),
            ),
            const SizedBox(height: 2),
            Text(traLoi.noiDung, style: const TextStyle(fontSize: 14)),
            const SizedBox(height: 2),
            Text(
              fmt.format(traLoi.createdAt.toLocal()),
              style: TextStyle(fontSize: 10, color: Colors.grey[500]),
            ),
          ],
        ),
      ),
    );
  }
}

class _ChatInput extends StatefulWidget {
  final TextEditingController controller;
  final bool isSending;
  final int maxLength;
  final VoidCallback onSend;

  const _ChatInput({
    required this.controller,
    required this.isSending,
    required this.maxLength,
    required this.onSend,
  });

  @override
  State<_ChatInput> createState() => _ChatInputState();
}

class _ChatInputState extends State<_ChatInput> {
  int _charCount = 0;

  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_onTextChanged);
  }

  @override
  void dispose() {
    widget.controller.removeListener(_onTextChanged);
    super.dispose();
  }

  void _onTextChanged() {
    setState(() => _charCount = widget.controller.text.length);
  }

  bool get _overLimit => _charCount > widget.maxLength;

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: Container(
        padding: const EdgeInsets.fromLTRB(12, 8, 8, 8),
        decoration: BoxDecoration(
          color: Theme.of(context).scaffoldBackgroundColor,
          border: const Border(top: BorderSide(color: Colors.black12)),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                Expanded(
                  child: TextField(
                    controller: widget.controller,
                    decoration: InputDecoration(
                      hintText: 'Nhập tin nhắn...',
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(24),
                      ),
                      isDense: true,
                      contentPadding: const EdgeInsets.symmetric(
                        horizontal: 14,
                        vertical: 10,
                      ),
                      focusedBorder: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(24),
                        borderSide: BorderSide(
                          color: _overLimit
                              ? Colors.red
                              : Theme.of(context).primaryColor,
                        ),
                      ),
                    ),
                    minLines: 1,
                    maxLines: 4,
                    textInputAction: TextInputAction.newline,
                  ),
                ),
                const SizedBox(width: 8),
                widget.isSending
                    ? const SizedBox(
                        width: 36,
                        height: 36,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : IconButton(
                        icon: const Icon(Icons.send),
                        color: _overLimit
                            ? Colors.grey
                            : Theme.of(context).primaryColor,
                        onPressed: _overLimit ? null : widget.onSend,
                      ),
              ],
            ),
            if (_charCount > widget.maxLength * 0.8)
              Padding(
                padding: const EdgeInsets.only(top: 2, right: 44),
                child: Text(
                  '$_charCount / ${widget.maxLength}',
                  style: TextStyle(
                    fontSize: 11,
                    color: _overLimit ? Colors.red : Colors.grey[600],
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

class _RatingDialog extends StatefulWidget {
  final int phanAnhId;
  final PhanAnhService service;
  final VoidCallback onDone;

  const _RatingDialog({
    required this.phanAnhId,
    required this.service,
    required this.onDone,
  });

  @override
  State<_RatingDialog> createState() => _RatingDialogState();
}

class _RatingDialogState extends State<_RatingDialog> {
  int _stars = 0;
  final _commentCtrl = TextEditingController();
  bool _isSubmitting = false;

  static const _maxComment = 500;

  @override
  void dispose() {
    _commentCtrl.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_stars == 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chọn số sao đánh giá.')),
      );
      return;
    }

    final comment = _commentCtrl.text.trim();
    if (comment.length > _maxComment) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Nhận xét không được vượt quá $_maxComment ký tự.'),
        ),
      );
      return;
    }

    setState(() => _isSubmitting = true);
    try {
      await widget.service.danhGia(
        phanAnhId: widget.phanAnhId,
        diemDanhGia: _stars,
        nhanXetDanhGia: comment,
      );
      widget.onDone();
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Đã xảy ra lỗi khi gửi đánh giá.'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Đánh giá chất lượng xử lý'),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Text('Chọn số sao:', style: TextStyle(fontSize: 14)),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: List.generate(
              5,
              (i) => GestureDetector(
                onTap: () => setState(() => _stars = i + 1),
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 4),
                  child: Icon(
                    i < _stars ? Icons.star : Icons.star_border,
                    color: Colors.amber,
                    size: 36,
                  ),
                ),
              ),
            ),
          ),
          if (_stars > 0)
            Padding(
              padding: const EdgeInsets.only(top: 4),
              child: Text(
                _starLabel(_stars),
                style: TextStyle(fontSize: 12, color: Colors.amber[700]),
              ),
            ),
          const SizedBox(height: 12),
          TextField(
            controller: _commentCtrl,
            decoration: InputDecoration(
              hintText: 'Nhận xét (không bắt buộc)...',
              border: const OutlineInputBorder(),
              isDense: true,
              counterText: '${_commentCtrl.text.length}/$_maxComment',
            ),
            minLines: 2,
            maxLines: 4,
            maxLength: _maxComment,
            buildCounter:
                (_, {required currentLength, required isFocused, maxLength}) =>
                    Text(
                      '$currentLength/${maxLength ?? _maxComment}',
                      style: TextStyle(
                        fontSize: 11,
                        color: currentLength > (maxLength ?? _maxComment) * 0.8
                            ? Colors.orange
                            : Colors.grey,
                      ),
                    ),
            onChanged: (_) => setState(() {}),
          ),
        ],
      ),
      actions: [
        TextButton(
          onPressed: _isSubmitting ? null : () => Navigator.pop(context),
          child: const Text('Hủy'),
        ),
        ElevatedButton(
          onPressed: _isSubmitting ? null : _submit,
          child: _isSubmitting
              ? const SizedBox(
                  width: 18,
                  height: 18,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Text('Gửi đánh giá'),
        ),
      ],
    );
  }

  String _starLabel(int stars) {
    switch (stars) {
      case 1:
        return 'Rất không hài lòng';
      case 2:
        return 'Không hài lòng';
      case 3:
        return 'Bình thường';
      case 4:
        return 'Hài lòng';
      case 5:
        return 'Rất hài lòng';
      default:
        return '';
    }
  }
}
