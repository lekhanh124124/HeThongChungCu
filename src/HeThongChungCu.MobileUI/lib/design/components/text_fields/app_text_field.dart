import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import 'package:klks_app/design/tokens/colors.dart';
import 'package:klks_app/design/tokens/typography.dart';

class AppTextField extends StatefulWidget {
  const AppTextField({
    super.key,
    this.label,
    this.hint,
    this.errorText,
    this.helperText,
    this.controller,
    this.focusNode,
    this.onChanged,
    this.onSubmitted,
    this.onTap,
    this.keyboardType,
    this.textInputAction,
    this.inputFormatters,
    this.maxLength,
    this.maxLines = 1,
    this.enabled = true,
    this.readOnly = false,
    this.prefixIcon,
    this.suffixIcon,
    this.autofocus = false,
  }) : _isPassword = false,
       _isSearch = false;

  const AppTextField.password({
    super.key,
    this.label,
    this.hint = 'Nhập mật khẩu',
    this.errorText,
    this.helperText,
    this.controller,
    this.focusNode,
    this.onChanged,
    this.onSubmitted,
    this.onTap,
    this.inputFormatters,
    this.maxLength,
    this.enabled = true,
    this.autofocus = false,
  }) : _isPassword = true,
       _isSearch = false,
       keyboardType = TextInputType.visiblePassword,
       textInputAction = TextInputAction.done,
       maxLines = 1,
       readOnly = false,
       prefixIcon = const Icon(
         Icons.lock_outline,
         color: AppColors.textSecondary,
         size: 20,
       ),
       suffixIcon = null;

  const AppTextField.search({
    super.key,
    this.hint = 'Tìm kiếm...',
    this.controller,
    this.focusNode,
    this.onChanged,
    this.onSubmitted,
    this.onTap,
    this.enabled = true,
    this.autofocus = false,
  }) : _isPassword = false,
       _isSearch = true,
       label = null,
       errorText = null,
       helperText = null,
       keyboardType = TextInputType.text,
       textInputAction = TextInputAction.search,
       inputFormatters = null,
       maxLength = null,
       maxLines = 1,
       readOnly = false,
       prefixIcon = null,
       suffixIcon = null;

  final String? label;
  final String? hint;
  final String? errorText;
  final String? helperText;
  final TextEditingController? controller;
  final FocusNode? focusNode;
  final ValueChanged<String>? onChanged;
  final ValueChanged<String>? onSubmitted;
  final VoidCallback? onTap;
  final TextInputType? keyboardType;
  final TextInputAction? textInputAction;
  final List<TextInputFormatter>? inputFormatters;
  final int? maxLength;
  final int maxLines;
  final bool enabled;
  final bool readOnly;
  final Widget? prefixIcon;
  final Widget? suffixIcon;
  final bool autofocus;

  final bool _isPassword;
  final bool _isSearch;

  @override
  State<AppTextField> createState() => _AppTextFieldState();
}

class _AppTextFieldState extends State<AppTextField> {
  bool _obscureText = true;

  Widget? get _prefix {
    if (widget._isSearch) {
      return const Icon(Icons.search, color: AppColors.textSecondary, size: 20);
    }
    return widget.prefixIcon;
  }

  Widget? get _suffix {
    if (widget._isPassword) {
      return GestureDetector(
        onTap: () => setState(() => _obscureText = !_obscureText),
        child: Icon(
          _obscureText
              ? Icons.visibility_off_outlined
              : Icons.visibility_outlined,
          color: AppColors.textSecondary,
          size: 20,
        ),
      );
    }
    if (widget.suffixIcon != null) return widget.suffixIcon;
    if (widget.errorText != null) {
      return const Icon(Icons.error_outline, color: AppColors.error, size: 20);
    }
    return null;
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        if (widget.label != null) ...[
          Text(
            widget.label!,
            style: AppTypography.subhead.copyWith(color: AppColors.textPrimary),
          ),
          const SizedBox(height: 6),
        ],

        TextFormField(
          controller: widget.controller,
          focusNode: widget.focusNode,
          onChanged: widget.onChanged,
          onFieldSubmitted: widget.onSubmitted,
          onTap: widget.onTap,
          keyboardType: widget.keyboardType,
          textInputAction: widget.textInputAction,
          inputFormatters: widget.inputFormatters,
          maxLength: widget.maxLength,
          maxLines: widget.maxLines,
          enabled: widget.enabled,
          readOnly: widget.readOnly,
          autofocus: widget.autofocus,
          obscureText: widget._isPassword && _obscureText,
          style: AppTypography.input.copyWith(color: AppColors.textPrimary),
          cursorColor: AppColors.primary,
          decoration: InputDecoration(
            hintText: widget.hint,
            errorText: widget.errorText,
            helperText: widget.helperText,
            prefixIcon: _prefix != null
                ? Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 12),
                    child: _prefix,
                  )
                : null,
            prefixIconConstraints: const BoxConstraints(
              minWidth: 44,
              minHeight: 44,
            ),
            suffixIcon: _suffix != null
                ? Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 12),
                    child: _suffix,
                  )
                : null,
            suffixIconConstraints: const BoxConstraints(
              minWidth: 44,
              minHeight: 44,
            ),
            counterText: '',
          ),
        ),
      ],
    );
  }
}
