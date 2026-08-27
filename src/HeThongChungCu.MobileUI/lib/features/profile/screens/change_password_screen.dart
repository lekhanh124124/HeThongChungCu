import 'package:flutter/material.dart';

import 'package:klks_app/design/design.dart';
import '../services/profile_service.dart';

class ChangePasswordScreen extends StatefulWidget {
  const ChangePasswordScreen({super.key});

  @override
  State<ChangePasswordScreen> createState() => _ChangePasswordScreenState();
}

class _ChangePasswordScreenState extends State<ChangePasswordScreen> {
  final _oldCtrl = TextEditingController();
  final _newCtrl = TextEditingController();
  final _confirmCtrl = TextEditingController();

  String? _oldError;
  String? _newError;
  String? _confirmError;
  bool _loading = false;

  @override
  void dispose() {
    _oldCtrl.dispose();
    _newCtrl.dispose();
    _confirmCtrl.dispose();
    super.dispose();
  }

  bool _validate() {
    String? oldErr, newErr, confirmErr;

    if (_oldCtrl.text.isEmpty) oldErr = 'Vui lòng nhập mật khẩu hiện tại';
    if (_newCtrl.text.isEmpty) {
      newErr = 'Vui lòng nhập mật khẩu mới';
    } else if (_newCtrl.text.length < 6) {
      newErr = 'Mật khẩu mới phải có ít nhất 6 ký tự';
    } else if (_newCtrl.text == _oldCtrl.text) {
      newErr = 'Mật khẩu mới không được trùng mật khẩu cũ';
    }
    if (_confirmCtrl.text.isEmpty) {
      confirmErr = 'Vui lòng xác nhận mật khẩu mới';
    } else if (_confirmCtrl.text != _newCtrl.text) {
      confirmErr = 'Mật khẩu xác nhận không khớp';
    }

    setState(() {
      _oldError = oldErr;
      _newError = newErr;
      _confirmError = confirmErr;
    });

    return oldErr == null && newErr == null && confirmErr == null;
  }

  Future<void> _submit() async {
    if (!_validate()) return;

    setState(() => _loading = true);
    try {
      await ProfileService.instance.changePassword(
        oldPassword: _oldCtrl.text,
        newPassword: _newCtrl.text,
        confirmPassword: _confirmCtrl.text,
      );
      if (!mounted) return;

      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Đổi mật khẩu thành công')));
      Navigator.pop(context);
    } catch (e) {
      if (!mounted) return;
      ErrorDisplay.showSnackBar(context, error: e);
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Đổi mật khẩu',
      body: ListView(
        padding: AppSpacing.insetAll16,
        children: [
          AppTextField.password(
            label: 'Mật khẩu hiện tại',
            hint: 'Nhập mật khẩu hiện tại',
            controller: _oldCtrl,
            errorText: _oldError,
            onChanged: (_) => setState(() => _oldError = null),
          ),
          const SizedBox(height: AppSpacing.md),

          AppTextField.password(
            label: 'Mật khẩu mới',
            hint: 'Nhập mật khẩu mới',
            controller: _newCtrl,
            errorText: _newError,
            onChanged: (_) => setState(() => _newError = null),
          ),
          const SizedBox(height: AppSpacing.md),

          AppTextField.password(
            label: 'Xác nhận mật khẩu mới',
            hint: 'Nhập lại mật khẩu mới',
            controller: _confirmCtrl,
            errorText: _confirmError,
            onChanged: (_) => setState(() => _confirmError = null),
          ),
          const SizedBox(height: AppSpacing.xl),

          AppButton(
            label: _loading ? 'Đang xử lý...' : 'Xác nhận',
            isLoading: _loading,
            onPressed: _loading ? null : _submit,
          ),
        ],
      ),
    );
  }
}
