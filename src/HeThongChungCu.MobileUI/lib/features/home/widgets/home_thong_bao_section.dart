import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/core/navigation/app_navigation.dart';
import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/thong_bao/models/thong_bao_model.dart';

import '../services/home_service.dart';

class HomeThongBaoSection extends StatelessWidget {
  const HomeThongBaoSection({super.key});

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: HomeService.instance,
      builder: (context, _) {
        final service = HomeService.instance;

        if (service.isLoadingThongBao && service.thongBaoMoi.isEmpty) {
          return const _SectionShimmer();
        }

        if (service.thongBaoError != null && service.thongBaoMoi.isEmpty) {
          return const SizedBox.shrink();
        }

        if (service.thongBaoMoi.isEmpty) {
          return const SizedBox.shrink();
        }

        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding: AppSpacing.insetH16,
              child: Row(
                children: [
                  Text('Thông báo mới', style: AppTypography.subhead),
                  const Spacer(),
                  TextButton(
                    onPressed: AppNavigation.goNotification,
                    style: TextButton.styleFrom(
                      padding: EdgeInsets.zero,
                      tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    ),
                    child: Text(
                      'Xem tất cả',
                      style: AppTypography.captionSmall.copyWith(
                        color: AppColors.primary,
                      ),
                    ),
                  ),
                ],
              ),
            ),
            AppSpacing.sm.verticalSpace,
            ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              padding: AppSpacing.insetH16,
              itemCount: service.thongBaoMoi.length,
              separatorBuilder: (_, _) => AppSpacing.xs.verticalSpace,
              itemBuilder: (_, i) =>
                  _ThongBaoCard(item: service.thongBaoMoi[i]),
            ),
          ],
        );
      },
    );
  }
}

class _ThongBaoCard extends StatelessWidget {
  final ThongBaoItem item;
  const _ThongBaoCard({required this.item});

  @override
  Widget build(BuildContext context) {
    return AppCard(
      onTap: () {
        AppNavigation.goNotification();
        context.push('/thong-bao/detail', extra: item);
      },
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm,
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(top: 6, right: 10),
            child: Container(
              width: 8,
              height: 8,
              decoration: const BoxDecoration(
                shape: BoxShape.circle,
                color: AppColors.primary,
              ),
            ),
          ),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Text(
                        item.tieuDe,
                        style: AppTypography.subhead,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                    const SizedBox(width: 8),
                    Text(
                      item.thoiGianHienThi,
                      style: AppTypography.captionSmall.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  item.noiDung,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: AppTypography.body.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
                if (item.tenLoaiThongBao.isNotEmpty) ...[
                  const SizedBox(height: 6),
                  AppStatusBadge(
                    label: item.tenLoaiThongBao,
                    variant: AppBadgeVariant.info,
                  ),
                ],
              ],
            ),
          ),
          const SizedBox(width: 4),
          const Icon(
            Icons.chevron_right,
            size: 18,
            color: AppColors.textDisabled,
          ),
        ],
      ),
    );
  }
}

class _SectionShimmer extends StatelessWidget {
  const _SectionShimmer();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: AppSpacing.insetH16,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 120,
            height: 16,
            decoration: BoxDecoration(
              color: AppColors.primaryLight,
              borderRadius: BorderRadius.circular(4),
            ),
          ),
          AppSpacing.sm.verticalSpace,
          for (var i = 0; i < 3; i++) ...[
            Container(
              height: 72,
              decoration: BoxDecoration(
                color: AppColors.primaryLight.withAlpha(80),
                borderRadius: AppRadius.card,
              ),
            ),
            if (i < 2) AppSpacing.xs.verticalSpace,
          ],
        ],
      ),
    );
  }
}
