import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import 'package:klks_app/design/design.dart';
import '../models/chat_model.dart';
import '../services/chat_service.dart';

class ChatScreen extends StatefulWidget {
  const ChatScreen({super.key});

  static void show(BuildContext context) {
    Navigator.of(context).push(
      PageRouteBuilder(
        pageBuilder: (_, _, _) => const ChatScreen(),
        transitionsBuilder: (_, animation, _, child) => SlideTransition(
          position: Tween<Offset>(
            begin: const Offset(0, 1),
            end: Offset.zero,
          ).animate(CurvedAnimation(parent: animation, curve: Curves.easeOut)),
          child: child,
        ),
        transitionDuration: const Duration(milliseconds: 300),
      ),
    );
  }

  @override
  State<ChatScreen> createState() => _ChatScreenState();
}

class _ChatScreenState extends State<ChatScreen> {
  final _service = ChatService.instance;
  final _inputCtrl = TextEditingController();
  final _scrollCtrl = ScrollController();
  final _focusNode = FocusNode();

  final List<_ChatBubble> _bubbles = [];
  final List<ChatMessage> _history = [];

  bool _isTyping = false;

  @override
  void initState() {
    super.initState();
    _bubbles.add(
      const _ChatBubble.assistant(
        text:
            'Xin chào! Tôi là trợ lý ảo của chung cư. '
            'Bạn có thể hỏi tôi về quy định, dịch vụ, hoặc bất kỳ thông tin nào liên quan đến tòa nhà.',
      ),
    );
  }

  @override
  void dispose() {
    _inputCtrl.dispose();
    _scrollCtrl.dispose();
    _focusNode.dispose();
    super.dispose();
  }

  Future<void> _send() async {
    final text = _inputCtrl.text.trim();
    if (text.isEmpty || _isTyping) return;

    _inputCtrl.clear();
    _focusNode.unfocus();

    setState(() {
      _bubbles.add(_ChatBubble.user(text: text));
      _isTyping = true;
    });
    _scrollToBottom();

    try {
      final response = await _service.chat(
        prompt: text,
        history: List.of(_history),
      );

      _history.add(ChatMessage.user(text));
      _history.add(ChatMessage.assistant(response.answer));

      if (!mounted) return;
      setState(() {
        _bubbles.add(
          _ChatBubble.assistant(
            text: response.answer,
            sources: response.sources,
          ),
        );
        _isTyping = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _bubbles.add(
          _ChatBubble.assistant(
            text: 'Xin lỗi, đã có lỗi xảy ra. Vui lòng thử lại.',
            isError: true,
          ),
        );
        _isTyping = false;
      });
    }

    _scrollToBottom();
  }

  void _scrollToBottom() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scrollCtrl.hasClients) {
        _scrollCtrl.animateTo(
          _scrollCtrl.position.maxScrollExtent,
          duration: const Duration(milliseconds: 300),
          curve: Curves.easeOut,
        );
      }
    });
  }

  void _clearHistory() {
    setState(() {
      _bubbles.clear();
      _history.clear();
      _bubbles.add(
        const _ChatBubble.assistant(
          text: 'Cuộc trò chuyện đã được xóa. Bạn có thể bắt đầu câu hỏi mới.',
        ),
      );
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        backgroundColor: AppColors.primary,
        foregroundColor: AppColors.textOnPrimary,
        surfaceTintColor: Colors.transparent,
        centerTitle: false,
        title: Row(
          children: [
            Container(
              width: 32,
              height: 32,
              decoration: BoxDecoration(
                color: AppColors.textOnPrimary.withAlpha(30),
                shape: BoxShape.circle,
              ),
              child: const Icon(Icons.smart_toy_outlined, size: 18),
            ),
            AppSpacing.sm.horizontalSpace,
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text('Trợ lý ảo PKK', style: AppTypography.subhead.onPrimary),
                Text(
                  'Powered by AI',
                  style: AppTypography.captionSmall.copyWith(
                    color: AppColors.textOnPrimary.withAlpha(180),
                  ),
                ),
              ],
            ),
          ],
        ),
        systemOverlayStyle: const SystemUiOverlayStyle(
          statusBarColor: Colors.transparent,
          statusBarIconBrightness: Brightness.light,
        ),
        actions: [
          if (_history.isNotEmpty)
            IconButton(
              icon: const Icon(Icons.delete_outline),
              tooltip: 'Xóa cuộc trò chuyện',
              onPressed: _clearHistory,
            ),
        ],
      ),
      body: Column(
        children: [
          Expanded(child: _buildMessageList()),
          if (_isTyping) _TypingIndicator(),
          _buildInput(),
        ],
      ),
    );
  }

  Widget _buildMessageList() {
    return ListView.builder(
      controller: _scrollCtrl,
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm,
      ),
      itemCount: _bubbles.length,
      itemBuilder: (_, i) => _BubbleWidget(bubble: _bubbles[i]),
    );
  }

  Widget _buildInput() {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.surface,
        boxShadow: AppElevation.level2,
      ),
      padding: EdgeInsets.fromLTRB(
        AppSpacing.md,
        AppSpacing.sm,
        AppSpacing.sm,
        AppSpacing.sm + MediaQuery.of(context).padding.bottom,
      ),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _inputCtrl,
              focusNode: _focusNode,
              enabled: !_isTyping,
              maxLines: 4,
              minLines: 1,
              textInputAction: TextInputAction.send,
              onSubmitted: (_) => _send(),
              style: AppTypography.body,
              cursorColor: AppColors.primary,
              decoration: InputDecoration(
                hintText: 'Nhập câu hỏi của bạn...',
                hintStyle: AppTypography.body.disabled,
                filled: true,
                fillColor: AppColors.inputFill,
                contentPadding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.md,
                  vertical: AppSpacing.sm2,
                ),
                border: OutlineInputBorder(
                  borderRadius: AppRadius.button,
                  borderSide: BorderSide.none,
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: AppRadius.button,
                  borderSide: BorderSide.none,
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: AppRadius.button,
                  borderSide: const BorderSide(
                    color: AppColors.borderFocused,
                    width: 1.5,
                  ),
                ),
              ),
            ),
          ),
          AppSpacing.xs.horizontalSpace,
          AnimatedContainer(
            duration: const Duration(milliseconds: 200),
            child: IconButton(
              onPressed: _isTyping ? null : _send,
              icon: const Icon(Icons.send_rounded),
              color: _isTyping ? AppColors.textDisabled : AppColors.primary,
              style: IconButton.styleFrom(
                backgroundColor: _isTyping
                    ? AppColors.secondaryLight
                    : AppColors.primaryLight,
                minimumSize: const Size(44, 44),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _ChatBubble {
  final bool isUser;
  final String text;
  final List<ChatSource> sources;
  final bool isError;

  const _ChatBubble.user({required this.text})
    : isUser = true,
      sources = const [],
      isError = false;

  const _ChatBubble.assistant({
    required this.text,
    this.sources = const [],
    this.isError = false,
  }) : isUser = false;
}

class _BubbleWidget extends StatelessWidget {
  final _ChatBubble bubble;
  const _BubbleWidget({required this.bubble});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.xs),
      child: bubble.isUser
          ? _UserBubble(bubble: bubble)
          : _AssistantBubble(bubble: bubble),
    );
  }
}

class _UserBubble extends StatelessWidget {
  final _ChatBubble bubble;
  const _UserBubble({required this.bubble});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.end,
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        const Spacer(),
        Flexible(
          flex: 4,
          child: GestureDetector(
            onLongPress: () {
              Clipboard.setData(ClipboardData(text: bubble.text));
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Đã sao chép'),
                  duration: Duration(seconds: 1),
                ),
              );
            },
            child: Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.md,
                vertical: AppSpacing.sm2,
              ),
              decoration: BoxDecoration(
                color: AppColors.primary,
                borderRadius: const BorderRadius.only(
                  topLeft: Radius.circular(AppRadius.standard),
                  topRight: Radius.circular(AppRadius.standard),
                  bottomLeft: Radius.circular(AppRadius.standard),
                  bottomRight: Radius.circular(AppRadius.xs),
                ),
              ),
              child: Text(bubble.text, style: AppTypography.body.onPrimary),
            ),
          ),
        ),
        AppSpacing.sm.horizontalSpace,
        CircleAvatar(
          radius: 14,
          backgroundColor: AppColors.primaryLight,
          child: const Icon(Icons.person, size: 16, color: AppColors.primary),
        ),
      ],
    );
  }
}

class _AssistantBubble extends StatelessWidget {
  final _ChatBubble bubble;
  const _AssistantBubble({required this.bubble});

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        CircleAvatar(
          radius: 14,
          backgroundColor: AppColors.primary,
          child: const Icon(
            Icons.smart_toy_outlined,
            size: 16,
            color: AppColors.textOnPrimary,
          ),
        ),
        AppSpacing.sm.horizontalSpace,
        Flexible(
          flex: 4,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              GestureDetector(
                onLongPress: () {
                  Clipboard.setData(ClipboardData(text: bubble.text));
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Đã sao chép'),
                      duration: Duration(seconds: 1),
                    ),
                  );
                },
                child: Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.md,
                    vertical: AppSpacing.sm2,
                  ),
                  decoration: BoxDecoration(
                    color: bubble.isError
                        ? AppColors.errorLight
                        : AppColors.surface,
                    borderRadius: const BorderRadius.only(
                      topLeft: Radius.circular(AppRadius.xs),
                      topRight: Radius.circular(AppRadius.standard),
                      bottomLeft: Radius.circular(AppRadius.standard),
                      bottomRight: Radius.circular(AppRadius.standard),
                    ),
                    boxShadow: AppElevation.level1,
                  ),
                  child: Text(
                    bubble.text,
                    style: AppTypography.body.withColor(
                      bubble.isError ? AppColors.error : AppColors.textPrimary,
                    ),
                  ),
                ),
              ),
              if (bubble.sources.isNotEmpty) ...[
                AppSpacing.xs.verticalSpace,
                _SourcesSection(sources: bubble.sources),
              ],
            ],
          ),
        ),
        const Spacer(),
      ],
    );
  }
}

class _SourcesSection extends StatefulWidget {
  final List<ChatSource> sources;
  const _SourcesSection({required this.sources});

  @override
  State<_SourcesSection> createState() => _SourcesSectionState();
}

class _SourcesSectionState extends State<_SourcesSection> {
  bool _expanded = false;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        GestureDetector(
          onTap: () => setState(() => _expanded = !_expanded),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(
                Icons.library_books_outlined,
                size: 12,
                color: AppColors.textSecondary,
              ),
              AppSpacing.xs.horizontalSpace,
              Text(
                '${widget.sources.length} nguồn tham khảo',
                style: AppTypography.captionSmall.secondary,
              ),
              Icon(
                _expanded ? Icons.expand_less : Icons.expand_more,
                size: 14,
                color: AppColors.textSecondary,
              ),
            ],
          ),
        ),
        if (_expanded) ...[
          AppSpacing.xs.verticalSpace,
          ...widget.sources.map(
            (s) => Padding(
              padding: const EdgeInsets.only(bottom: 4),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Icon(
                    Icons.circle,
                    size: 5,
                    color: AppColors.textSecondary,
                  ),
                  AppSpacing.xs.horizontalSpace,
                  Expanded(
                    child: Text(
                      s.displayTitle,
                      style: AppTypography.captionSmall.secondary,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ],
    );
  }
}

class _TypingIndicator extends StatefulWidget {
  @override
  State<_TypingIndicator> createState() => _TypingIndicatorState();
}

class _TypingIndicatorState extends State<_TypingIndicator>
    with TickerProviderStateMixin {
  late final List<AnimationController> _controllers;
  late final List<Animation<double>> _anims;

  @override
  void initState() {
    super.initState();
    _controllers = List.generate(
      3,
      (i) => AnimationController(
        vsync: this,
        duration: const Duration(milliseconds: 400),
      ),
    );
    _anims = _controllers.map((c) {
      return Tween<double>(
        begin: 0,
        end: -6,
      ).animate(CurvedAnimation(parent: c, curve: Curves.easeInOut));
    }).toList();

    for (int i = 0; i < 3; i++) {
      Future.delayed(Duration(milliseconds: i * 150), () {
        if (mounted) _controllers[i].repeat(reverse: true);
      });
    }
  }

  @override
  void dispose() {
    for (final c in _controllers) {
      c.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.md + 28 + AppSpacing.sm,
        0,
        AppSpacing.md,
        AppSpacing.xs,
      ),
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.md,
          vertical: AppSpacing.sm,
        ),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: const BorderRadius.only(
            topLeft: Radius.circular(AppRadius.xs),
            topRight: Radius.circular(AppRadius.standard),
            bottomLeft: Radius.circular(AppRadius.standard),
            bottomRight: Radius.circular(AppRadius.standard),
          ),
          boxShadow: AppElevation.level1,
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: List.generate(3, (i) {
            return AnimatedBuilder(
              animation: _anims[i],
              builder: (_, _) => Transform.translate(
                offset: Offset(0, _anims[i].value),
                child: Container(
                  width: 7,
                  height: 7,
                  margin: const EdgeInsets.symmetric(horizontal: 2),
                  decoration: const BoxDecoration(
                    color: AppColors.textDisabled,
                    shape: BoxShape.circle,
                  ),
                ),
              ),
            );
          }),
        ),
      ),
    );
  }
}
