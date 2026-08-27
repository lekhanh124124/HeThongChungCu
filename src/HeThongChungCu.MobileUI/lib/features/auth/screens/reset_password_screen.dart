import 'dart:async';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../services/auth_service.dart';

class ResetPasswordScreen extends StatefulWidget {
  final String username;

  const ResetPasswordScreen({super.key, required this.username});

  @override
  State<ResetPasswordScreen> createState() => _ResetPasswordScreenState();
}

class _ResetPasswordScreenState extends State<ResetPasswordScreen> {
  final _authService = AuthService.instance;

  final _resetCodeController = TextEditingController();
  final _newPasswordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();

  bool _loading = false;
  bool _resending = false;
  String? _errorText;
  String? _successMessage;

  int _resendCooldown = 0;
  Timer? _cooldownTimer;

  @override
  void dispose() {
    _resetCodeController.dispose();
    _newPasswordController.dispose();
    _confirmPasswordController.dispose();
    _cooldownTimer?.cancel();
    super.dispose();
  }

  void _startCooldown() {
    _resendCooldown = 60;
    _cooldownTimer?.cancel();
    _cooldownTimer = Timer.periodic(const Duration(seconds: 1), (t) {
      if (!mounted) {
        t.cancel();
        return;
      }
      setState(() {
        _resendCooldown--;
        if (_resendCooldown <= 0) t.cancel();
      });
    });
  }

  Future<void> _resendCode() async {
    setState(() {
      _resending = true;
      _errorText = null;
      _successMessage = null;
    });
    try {
      await _authService.forgotPassword(username: widget.username);
      if (!mounted) return;
      setState(() => _successMessage = 'Đã gửi lại mã xác nhận.');
      _startCooldown();
    } catch (e) {
      if (!mounted) return;
      setState(() => _errorText = e.toString());
    } finally {
      if (mounted) setState(() => _resending = false);
    }
  }

  Future<void> _resetPassword() async {
    setState(() {
      _loading = true;
      _errorText = null;
      _successMessage = null;
    });

    try {
      await _authService.resetPassword(
        username: widget.username,
        resetCode: _resetCodeController.text.trim(),
        newPassword: _newPasswordController.text.trim(),
        confirmPassword: _confirmPasswordController.text.trim(),
      );

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Đặt lại mật khẩu thành công! Vui lòng đăng nhập.'),
        ),
      );
      context.go('/auth/login');
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
                        Icons.verified_user_rounded,
                        color: AppColors.primary,
                        size: 32,
                      ),
                    ),

                    AppSpacing.lg.verticalSpace,

                    Text(
                      'Đặt lại mật khẩu',
                      style: AppTypography.display.copyWith(
                        color: AppColors.textPrimary,
                        fontSize: 26,
                      ),
                    ),

                    AppSpacing.xs.verticalSpace,

                    RichText(
                      text: TextSpan(
                        style: AppTypography.body.copyWith(
                          color: AppColors.textSecondary,
                        ),
                        children: [
                          const TextSpan(
                            text: 'Mã xác nhận đã được gửi cho tài khoản ',
                          ),
                          TextSpan(
                            text: widget.username,
                            style: AppTypography.bodyMedium.copyWith(
                              color: AppColors.textPrimary,
                            ),
                          ),
                          const TextSpan(text: '.'),
                        ],
                      ),
                    ),

                    AppSpacing.xl.verticalSpace,

                    if (_errorText != null) ...[
                      ErrorDisplay(error: _errorText, compact: true),
                      AppSpacing.md.verticalSpace,
                    ],
                    if (_successMessage != null) ...[
                      _SuccessBanner(message: _successMessage!),
                      AppSpacing.md.verticalSpace,
                    ],

                    AppTextField(
                      label: 'MÃ XÁC NHẬN',
                      hint: 'Nhập mã xác nhận',
                      controller: _resetCodeController,
                      keyboardType: TextInputType.number,
                      textInputAction: TextInputAction.next,
                      prefixIcon: const Icon(
                        Icons.tag_rounded,
                        color: AppColors.textSecondary,
                        size: 20,
                      ),
                    ),

                    AppSpacing.xs.verticalSpace,

                    Align(
                      alignment: Alignment.centerRight,
                      child: _resending
                          ? const SizedBox(
                              width: 16,
                              height: 16,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                color: AppColors.primary,
                              ),
                            )
                          : TextButton(
                              onPressed: _resendCooldown > 0
                                  ? null
                                  : _resendCode,
                              style: TextButton.styleFrom(
                                padding: EdgeInsets.zero,
                                minimumSize: Size.zero,
                                tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                              ),
                              child: Text(
                                _resendCooldown > 0
                                    ? 'Gửi lại sau ${_resendCooldown}s'
                                    : 'Gửi lại mã',
                                style: AppTypography.bodyMedium.copyWith(
                                  color: _resendCooldown > 0
                                      ? AppColors.textDisabled
                                      : AppColors.primary,
                                ),
                              ),
                            ),
                    ),

                    AppSpacing.md.verticalSpace,

                    AppTextField.password(
                      label: 'MẬT KHẨU MỚI',
                      hint: 'Nhập mật khẩu mới',
                      controller: _newPasswordController,
                    ),

                    AppSpacing.md.verticalSpace,

                    AppTextField.password(
                      label: 'XÁC NHẬN MẬT KHẨU',
                      hint: 'Nhập lại mật khẩu mới',
                      controller: _confirmPasswordController,
                    ),

                    AppSpacing.xl.verticalSpace,

                    AppButton(
                      label: 'Đặt lại mật khẩu',
                      isLoading: _loading,
                      onPressed: _loading ? null : _resetPassword,
                    ),

                    AppSpacing.md.verticalSpace,

                    AppButton(
                      label: 'Quay lại đăng nhập',
                      variant: AppButtonVariant.secondary,
                      onPressed: () => context.go('/auth/login'),
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

class _SuccessBanner extends StatelessWidget {
  final String message;
  const _SuccessBanner({required this.message});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      decoration: BoxDecoration(
        color: AppColors.successLight,
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: AppColors.success.withAlpha(60)),
      ),
      child: Row(
        children: [
          const Icon(
            Icons.check_circle_outline,
            color: AppColors.success,
            size: 18,
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              message,
              style: AppTypography.body.copyWith(
                color: AppColors.success,
                fontSize: 13,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
