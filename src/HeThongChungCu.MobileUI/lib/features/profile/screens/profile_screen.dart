import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/core/storage/user_session.dart';
import 'package:klks_app/design/design.dart';

import '../services/profile_service.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final session = UserSession.instance;

    return AppScaffold(
      title: 'Cá nhân',
      body: ListView(
        padding: AppSpacing.insetAll16,
        children: [
          _ProfileHeader(
            fullName: session.fullName ?? '',
            email: session.email ?? '',
          ),
          const SizedBox(height: AppSpacing.xl),

          Text('Chức năng', style: context.textTheme.titleMedium),
          const SizedBox(height: AppSpacing.sm),

          AppServiceCard(
            icon: Icons.person_outline,
            title: 'Xem chi tiết hồ sơ',
            onTap: () => context.push('/profile/detail'),
          ),
          const SizedBox(height: AppSpacing.sm),
          AppServiceCard(
            icon: Icons.lock_outline,
            title: 'Đổi mật khẩu',
            onTap: () => context.push('/profile/change-password'),
          ),
          const SizedBox(height: AppSpacing.sm),
          AppServiceCard(
            icon: Icons.camera_alt_outlined,
            title: 'Đổi ảnh đại diện',
            onTap: () => context.push('/profile/change-avatar'),
          ),
          const SizedBox(height: AppSpacing.xl),

          AppButton(
            label: 'Đăng xuất',
            variant: AppButtonVariant.outline,
            foregroundColor: AppColors.error,
            leadingIcon: Icons.logout,
            onPressed: () => _logout(context),
          ),
        ],
      ),
    );
  }

  Future<void> _logout(BuildContext context) async {
    final confirmed = await AppConfirmDialog.show(
      context,
      title: 'Đăng xuất',
      message: 'Bạn có chắc muốn đăng xuất không?',
      confirmLabel: 'Đăng xuất',
      isDangerous: true,
    );
    if (confirmed != true || !context.mounted) return;

    await ProfileService.instance.logout();
  }
}

class _ProfileHeader extends StatelessWidget {
  final String fullName;
  final String email;

  const _ProfileHeader({required this.fullName, required this.email});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        ValueListenableBuilder(
          valueListenable: UserSession.instance.anhDaiDienUrlNotifier,
          builder: (context, url, _) => CircleAvatar(
            radius: 44,
            backgroundColor: context.colorScheme.primaryContainer,
            backgroundImage: url != null ? NetworkImage(url) : null,
            child: url == null
                ? Icon(
                    Icons.person,
                    size: 44,
                    color: context.colorScheme.primary,
                  )
                : null,
          ),
        ),
        const SizedBox(height: AppSpacing.md),

        if (fullName.isNotEmpty)
          Text(fullName, style: context.textTheme.headlineMedium),

        if (email.isNotEmpty) ...[
          const SizedBox(height: AppSpacing.xs),
          Text(
            email,
            style: context.textTheme.bodyMedium?.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ],
    );
  }
}
