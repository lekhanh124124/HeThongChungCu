import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import 'package:klks_app/design/design.dart';

import '../services/profile_service.dart';

class ChangeAvatarScreen extends StatefulWidget {
  const ChangeAvatarScreen({super.key});

  @override
  State<ChangeAvatarScreen> createState() => _ChangeAvatarScreenState();
}

class _ChangeAvatarScreenState extends State<ChangeAvatarScreen> {
  File? _file;
  bool _loading = false;

  Future<void> _pick(ImageSource source) async {
    final res = await ImagePicker().pickImage(
      source: source,
      imageQuality: 85,
      maxWidth: 1024,
    );
    if (res != null && mounted) {
      setState(() => _file = File(res.path));
    }
  }

  void _showPickOptions() {
    showModalBottomSheet<void>(
      context: context,
      shape: const RoundedRectangleBorder(borderRadius: AppRadius.modal),
      builder: (_) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const SizedBox(height: AppSpacing.sm),
            ListTile(
              leading: const Icon(Icons.photo_library_outlined),
              title: const Text('Chọn từ thư viện'),
              onTap: () {
                Navigator.pop(context);
                _pick(ImageSource.gallery);
              },
            ),
            ListTile(
              leading: const Icon(Icons.camera_alt_outlined),
              title: const Text('Chụp ảnh'),
              onTap: () {
                Navigator.pop(context);
                _pick(ImageSource.camera);
              },
            ),
            const SizedBox(height: AppSpacing.sm),
          ],
        ),
      ),
    );
  }

  Future<void> _upload() async {
    if (_file == null) return;

    setState(() => _loading = true);
    try {
      await ProfileService.instance.changeAvatar(_file!);

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Cập nhật ảnh đại diện thành công')),
      );
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
      title: 'Đổi ảnh đại diện',
      body: Padding(
        padding: AppSpacing.insetAll16,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            const SizedBox(height: AppSpacing.xl),

            GestureDetector(
              onTap: _showPickOptions,
              child: Stack(
                alignment: Alignment.bottomRight,
                children: [
                  CircleAvatar(
                    radius: 72,
                    backgroundColor: context.colorScheme.primaryContainer,
                    backgroundImage: _file != null ? FileImage(_file!) : null,
                    child: _file == null
                        ? Icon(
                            Icons.person,
                            size: 72,
                            color: context.colorScheme.primary,
                          )
                        : null,
                  ),
                  Container(
                    padding: const EdgeInsets.all(6),
                    decoration: BoxDecoration(
                      color: context.colorScheme.primary,
                      shape: BoxShape.circle,
                      border: Border.all(color: Colors.white, width: 2),
                    ),
                    child: const Icon(
                      Icons.camera_alt,
                      color: Colors.white,
                      size: 18,
                    ),
                  ),
                ],
              ),
            ),

            const SizedBox(height: AppSpacing.sm),
            Text(
              'Nhấn vào ảnh để thay đổi',
              style: context.textTheme.bodySmall?.copyWith(
                color: AppColors.textSecondary,
              ),
            ),

            const SizedBox(height: AppSpacing.xxl),

            AppButton(
              label: 'Chọn ảnh',
              variant: AppButtonVariant.outline,
              leadingIcon: Icons.photo_library_outlined,
              onPressed: _showPickOptions,
            ),
            const SizedBox(height: AppSpacing.sm),

            AppButton(
              label: _loading ? 'Đang tải lên...' : 'Lưu ảnh đại diện',
              isLoading: _loading,
              onPressed: (_file == null || _loading) ? null : _upload,
            ),
          ],
        ),
      ),
    );
  }
}
