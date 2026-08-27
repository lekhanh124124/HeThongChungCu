import 'package:flutter/material.dart';

import 'package:klks_app/design/design.dart';

import '../models/quan_he_cu_tru_model.dart';

class CanHoSelector extends StatelessWidget {
  final List<QuanHeCuTruModel> dsCanHo;
  final QuanHeCuTruModel? selected;
  final ValueChanged<QuanHeCuTruModel> onChanged;

  const CanHoSelector({
    super.key,
    required this.dsCanHo,
    required this.selected,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    if (dsCanHo.length == 1) {
      return _SingleCanHoBanner(canHo: dsCanHo.first);
    }
    return _CanHoDropdown(
      dsCanHo: dsCanHo,
      selected: selected,
      onChanged: onChanged,
    );
  }
}

class _SingleCanHoBanner extends StatelessWidget {
  final QuanHeCuTruModel canHo;
  const _SingleCanHoBanner({required this.canHo});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm2,
      ),
      decoration: BoxDecoration(
        color: AppColors.primary,
        borderRadius: AppRadius.inputField,
        boxShadow: AppElevation.level1,
      ),
      child: Row(
        children: [
          const Icon(Icons.apartment, color: AppColors.textOnPrimary, size: 18),
          const SizedBox(width: AppSpacing.sm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(canHo.tenCanHo, style: AppTypography.subhead.onPrimary),
                Text(
                  '${canHo.tenToaNha} · ${canHo.tenTang}',
                  style: AppTypography.captionSmall.copyWith(
                    color: AppColors.textOnPrimary.withAlpha(210),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _CanHoDropdown extends StatelessWidget {
  final List<QuanHeCuTruModel> dsCanHo;
  final QuanHeCuTruModel? selected;
  final ValueChanged<QuanHeCuTruModel> onChanged;

  const _CanHoDropdown({
    required this.dsCanHo,
    required this.selected,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      decoration: BoxDecoration(
        color: AppColors.background,
        boxShadow: AppElevation.level1,
      ),
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Căn hộ', style: AppTypography.captionSmall.secondary),
          const SizedBox(height: 6),
          DropdownButtonFormField<QuanHeCuTruModel>(
            initialValue: selected,
            isExpanded: true,
            decoration: InputDecoration(
              prefixIcon: const Icon(
                Icons.apartment_outlined,
                size: 18,
                color: AppColors.textSecondary,
              ),
              contentPadding: AppSpacing.inputPadding,
              border: OutlineInputBorder(
                borderRadius: AppRadius.inputField,
                borderSide: const BorderSide(color: AppColors.border),
              ),
              enabledBorder: OutlineInputBorder(
                borderRadius: AppRadius.inputField,
                borderSide: const BorderSide(color: AppColors.border),
              ),
              focusedBorder: OutlineInputBorder(
                borderRadius: AppRadius.inputField,
                borderSide: const BorderSide(
                  color: AppColors.borderFocused,
                  width: 1.5,
                ),
              ),
              filled: true,
              fillColor: AppColors.inputFill,
            ),
            items: dsCanHo.map((canHo) {
              return DropdownMenuItem<QuanHeCuTruModel>(
                value: canHo,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(canHo.tenCanHo, style: AppTypography.subhead),
                    Text(
                      '${canHo.tenToaNha} · ${canHo.tenTang}',
                      style: AppTypography.captionSmall.secondary,
                    ),
                  ],
                ),
              );
            }).toList(),
            selectedItemBuilder: (context) => dsCanHo.map((canHo) {
              return Align(
                alignment: Alignment.centerLeft,
                child: Text(
                  '${canHo.tenCanHo}  ·  ${canHo.tenToaNha}',
                  style: AppTypography.bodyMedium,
                  overflow: TextOverflow.ellipsis,
                ),
              );
            }).toList(),
            onChanged: (canHo) {
              if (canHo != null) onChanged(canHo);
            },
          ),
        ],
      ),
    );
  }
}
