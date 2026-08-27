import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../services/auth_service.dart';

class ForgotPasswordScreen extends StatefulWidget {
  const ForgotPasswordScreen({super.key});

  @override
  State<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  final _authService = AuthService.instance;
  final _usernameController = TextEditingController();
  bool _loading = false;
  String? _errorText;

  @override
  void dispose() {
    _usernameController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final username = _usernameController.text.trim();

    setState(() {
      _loading = true;
      _errorText = null;
    });

    try {
      await _authService.forgotPassword(username: username);

      if (!mounted) return;

      context.push('/auth/reset-password/$username');
    } catch (e) {
      if (!mounted) return;
      setState(() => _errorText = e.toString());
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      showAppBar: false,
      body: SafeArea(
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.sm,
                vertical: AppSpacing.sm,
              ),
              child: Row(
                children: [
                  IconButton(
                    onPressed: () => context.pop(),
                    icon: const Icon(
                      Icons.arrow_back_ios_new_rounded,
                      size: 20,
                    ),
                    color: AppColors.textPrimary,
                  ),
                ],
              ),
            ),

            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: AppSpacing.md),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    AppSpacing.md.verticalSpace,

                    Container(
                      width: 64,
                      height: 64,
                      decoration: BoxDecoration(
                        color: AppColors.primaryLight,
                        shape: BoxShape.circle,
                      ),
                      child: const Icon(
                        Icons.lock_reset_rounded,
                        color: AppColors.primary,
                        size: 32,
                      ),
                    ),

                    AppSpacing.lg.verticalSpace,

                    Text(
                      'Quên mật khẩu?',
                      style: AppTypography.display.copyWith(
                        color: AppColors.textPrimary,
                        fontSize: 26,
                      ),
                    ),

                    AppSpacing.xs.verticalSpace,

                    Text(
                      'Nhập email của bạn. Chúng tôi sẽ gửi mã xác nhận để đặt lại mật khẩu.',
                      style: AppTypography.body.copyWith(
                        color: AppColors.textSecondary,
                        height: 1.6,
                      ),
                    ),

                    AppSpacing.xl.verticalSpace,

                    if (_errorText != null) ...[
                      ErrorDisplay(error: _errorText, compact: true),
                      AppSpacing.md.verticalSpace,
                    ],

                    AppTextField(
                      label: 'EMAIL',
                      hint: 'Nhập email của bạn',
                      controller: _usernameController,
                      keyboardType: TextInputType.text,
                      textInputAction: TextInputAction.done,
                      prefixIcon: const Icon(
                        Icons.person_outline,
                        color: AppColors.textSecondary,
                        size: 20,
                      ),
                    ),

                    AppSpacing.xl.verticalSpace,

                    AppButton(
                      label: 'Gửi mã xác nhận',
                      isLoading: _loading,
                      leadingIcon: Icons.send_rounded,
                      onPressed: _loading ? null : _submit,
                    ),

                    AppSpacing.md.verticalSpace,

                    AppButton(
                      label: 'Quay lại đăng nhập',
                      variant: AppButtonVariant.secondary,
                      onPressed: () => context.pop(),
                    ),

                    AppSpacing.xl.verticalSpace,
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
