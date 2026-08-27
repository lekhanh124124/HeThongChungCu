import 'package:flutter/material.dart';

import 'package:klks_app/design/design.dart';

import '../screens/chat_screen.dart';

class ChatFab extends StatelessWidget {
  const ChatFab({super.key});

  @override
  Widget build(BuildContext context) {
    return FloatingActionButton.extended(
      onPressed: () => ChatScreen.show(context),
      backgroundColor: AppColors.primary,
      foregroundColor: AppColors.textOnPrimary,
      elevation: 3,
      icon: const Icon(Icons.smart_toy_outlined),
      label: Text('Trợ lý AI', style: AppTypography.buttonLabel.onPrimary),
    );
  }
}

class ChatIconButton extends StatelessWidget {
  const ChatIconButton({super.key});

  @override
  Widget build(BuildContext context) {
    return IconButton(
      onPressed: () => ChatScreen.show(context),
      tooltip: 'Trợ lý AI',
      icon: const Icon(Icons.smart_toy_outlined),
    );
  }
}

class ChatBannerButton extends StatelessWidget {
  const ChatBannerButton({super.key});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => ChatScreen.show(context),
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.md,
          vertical: AppSpacing.sm2,
        ),
        decoration: BoxDecoration(
          gradient: const LinearGradient(
            colors: [AppColors.primary, AppColors.primaryDark],
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
          ),
          borderRadius: AppRadius.card,
          boxShadow: AppElevation.level1,
        ),
        child: Row(
          children: [
            Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                color: AppColors.textOnPrimary.withAlpha(30),
                shape: BoxShape.circle,
              ),
              child: const Icon(
                Icons.smart_toy_outlined,
                color: AppColors.textOnPrimary,
                size: 22,
              ),
            ),
            AppSpacing.md.horizontalSpace,
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Trợ lý ảo PKK', style: AppTypography.subhead.onPrimary),
                  Text(
                    'Hỏi về quy định, dịch vụ, tòa nhà...',
                    style: AppTypography.captionSmall.copyWith(
                      color: AppColors.textOnPrimary.withAlpha(200),
                    ),
                  ),
                ],
              ),
            ),
            const Icon(
              Icons.arrow_forward_ios,
              size: 14,
              color: AppColors.textOnPrimary,
            ),
          ],
        ),
      ),
    );
  }
}
