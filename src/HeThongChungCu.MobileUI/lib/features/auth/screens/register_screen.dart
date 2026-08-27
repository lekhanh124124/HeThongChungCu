import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

import '../services/auth_service.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _authService = AuthService.instance;
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmController = TextEditingController();
  bool _loading = false;
  String? _errorText;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _register() async {
    setState(() {
      _loading = true;
      _errorText = null;
    });
    try {
      await _authService.register(
        email: _emailController.text,
        password: _passwordController.text,
        confirmPassword: _confirmController.text,
      );
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Đăng ký thành công! Vui lòng đăng nhập.'),
          ),
        );
        context.go('/auth/login');
      }
    } catch (e) {
      if (mounted) {
        setState(() => _errorText = e.toString());
      }
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

                    Text(
                      'Tạo tài khoản',
                      style: AppTypography.display.copyWith(
                        color: AppColors.textPrimary,
                        fontSize: 26,
                      ),
                    ),

                    AppSpacing.xs.verticalSpace,

                    Text(
                      'Điền thông tin để bắt đầu sử dụng PKK Resident',
                      style: AppTypography.body.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),

                    AppSpacing.xl.verticalSpace,

                    if (_errorText != null) ...[
                      ErrorDisplay(error: _errorText, compact: true),
                      AppSpacing.md.verticalSpace,
                    ],

                    AppTextField(
                      label: 'EMAIL',
                      hint: 'username@gmail.com',
                      controller: _emailController,
                      keyboardType: TextInputType.emailAddress,
                      textInputAction: TextInputAction.next,
                      prefixIcon: const Icon(
                        Icons.email_outlined,
                        color: AppColors.textSecondary,
                        size: 20,
                      ),
                    ),

                    AppSpacing.md.verticalSpace,

                    AppTextField.password(
                      label: 'MẬT KHẨU',
                      controller: _passwordController,
                    ),

                    AppSpacing.md.verticalSpace,

                    AppTextField.password(
                      label: 'XÁC NHẬN MẬT KHẨU',
                      hint: 'Nhập lại mật khẩu',
                      controller: _confirmController,
                    ),

                    AppSpacing.xl.verticalSpace,

                    AppButton(
                      label: 'Đăng ký',
                      isLoading: _loading,
                      onPressed: _loading ? null : _register,
                    ),

                    AppSpacing.lg.verticalSpace,

                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Text(
                          'Đã có tài khoản? ',
                          style: AppTypography.body.copyWith(
                            color: AppColors.textSecondary,
                          ),
                        ),
                        GestureDetector(
                          onTap: () => context.pop(),
                          child: Text(
                            'Đăng nhập',
                            style: AppTypography.bodyMedium.copyWith(
                              color: AppColors.primary,
                            ),
                          ),
                        ),
                      ],
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
