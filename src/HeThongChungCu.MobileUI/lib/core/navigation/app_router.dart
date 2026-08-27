import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/core/guards/auth_guard.dart';
import 'package:klks_app/design/design.dart';
import 'package:klks_app/features/dich_vu/phan_anh/screens/phan_anh_create_screen.dart';
import 'package:klks_app/features/dich_vu/phan_anh/screens/phan_anh_detail_screen.dart';
import 'package:klks_app/features/dich_vu/sua_chua/screens/sua_chua_create_screen.dart';
import 'package:klks_app/features/dich_vu/sua_chua/screens/sua_chua_detail_screen.dart';
import 'package:klks_app/features/dich_vu/thi_cong/screens/thi_cong_detail_screen.dart';
import 'package:klks_app/features/dich_vu/thi_cong/screens/thi_cong_form_screen.dart';
import 'package:klks_app/features/dich_vu/tien_ich/screens/dang_ky_dich_vu_screen.dart';
import 'package:klks_app/features/dich_vu/tien_ich/screens/tien_ich_detail_screen.dart';

import 'main_screen.dart';

import 'package:klks_app/features/splash/screens/splash_screen.dart';

import 'package:klks_app/features/auth/screens/login_screen.dart';
import 'package:klks_app/features/auth/screens/register_screen.dart';
import 'package:klks_app/features/auth/screens/forgot_password_screen.dart';
import 'package:klks_app/features/auth/screens/reset_password_screen.dart';

import 'package:klks_app/features/home/screens/home_screen.dart';

import 'package:klks_app/features/thong_bao/screens/thong_bao_list_screen.dart';
import 'package:klks_app/features/thong_bao/screens/thong_bao_detail_screen.dart';

import 'package:klks_app/features/dich_vu/screens/dich_vu_screen.dart';

import 'package:klks_app/features/dich_vu/tien_ich/screens/tien_ich_screen.dart';

import 'package:klks_app/features/dich_vu/sua_chua/screens/sua_chua_list_screen.dart';

import 'package:klks_app/features/dich_vu/thi_cong/screens/thi_cong_list_screen.dart';

import 'package:klks_app/features/dich_vu/hoa_don/screens/hoa_don_list_screen.dart';
import 'package:klks_app/features/dich_vu/hoa_don/screens/chi_tiet_phi_screen.dart';
import 'package:klks_app/features/dich_vu/hoa_don/screens/hoa_don_detail_screen.dart';
import 'package:klks_app/features/dich_vu/hoa_don/screens/thanh_toan_screen.dart';

import 'package:klks_app/features/dich_vu/phan_anh/screens/phan_anh_list_screen.dart';

import 'package:klks_app/features/dich_vu/khao_sat/screens/khao_sat_list_screen.dart';
import 'package:klks_app/features/dich_vu/khao_sat/screens/khao_sat_detail_screen.dart';
import 'package:klks_app/features/dich_vu/khao_sat/screens/khao_sat_ket_qua_screen.dart';

import 'package:klks_app/features/cu_tru/quan_he/screens/cu_tru_list_screen.dart';

import 'package:klks_app/features/cu_tru/thanh_vien/screens/thanh_vien_screen.dart';
import 'package:klks_app/features/cu_tru/thanh_vien/screens/thanh_vien_detail_screen.dart';
import 'package:klks_app/features/cu_tru/thanh_vien/screens/yeu_cau_detail_screen.dart';
import 'package:klks_app/features/cu_tru/thanh_vien/screens/yeu_cau_cu_tru_form_screen.dart';
import 'package:klks_app/features/cu_tru/thanh_vien/screens/xoa_yeu_cau_thanh_vien_screen.dart';

import 'package:klks_app/features/cu_tru/phuong_tien/screens/phuong_tien_screen.dart';
import 'package:klks_app/features/cu_tru/phuong_tien/screens/phuong_tien_detail_screen.dart';
import 'package:klks_app/features/cu_tru/phuong_tien/screens/yeu_cau_phuong_tien_detail_screen.dart';
import 'package:klks_app/features/cu_tru/phuong_tien/screens/tao_yeu_cau_phuong_tien_screen.dart';

import 'package:klks_app/features/profile/screens/profile_screen.dart';
import 'package:klks_app/features/profile/screens/profile_detail_screen.dart';
import 'package:klks_app/features/profile/screens/change_password_screen.dart';
import 'package:klks_app/features/profile/screens/change_avatar_screen.dart';

class AppRouter {
  AppRouter._();

  static final router = GoRouter(
    initialLocation: '/splash',
    refreshListenable: AuthGuard.instance,

    redirect: (context, state) {
      final status = AuthGuard.instance.status;
      final location = state.uri.path;

      final isAuthRoute = location.startsWith('/auth');
      final isSplash = location == '/splash';

      if (status == AuthStatus.unknown) {
        return isSplash ? null : '/splash';
      }

      if (status == AuthStatus.unauthenticated) {
        if (isAuthRoute) return null;
        return '/auth/login';
      }

      if (status == AuthStatus.authenticated) {
        if (isAuthRoute || isSplash) return '/home';
      }

      return null;
    },

    routes: [
      GoRoute(path: '/splash', builder: (_, _) => const SplashScreen()),

      GoRoute(
        path: '/auth',
        redirect: (_, state) {
          if (state.uri.path == '/auth') return '/auth/login';
          return null;
        },
        routes: [
          GoRoute(path: 'login', builder: (_, _) => const LoginScreen()),
          GoRoute(path: 'register', builder: (_, _) => const RegisterScreen()),
          GoRoute(
            path: 'forgot-password',
            builder: (_, _) => const ForgotPasswordScreen(),
          ),
          GoRoute(
            path: 'reset-password/:username',
            builder: (_, state) => ResetPasswordScreen(
              username: state.pathParameters['username']!,
            ),
          ),
        ],
      ),

      StatefulShellRoute.indexedStack(
        builder: (context, state, navigationShell) {
          return MainScreen(shell: navigationShell);
        },
        branches: [
          StatefulShellBranch(
            routes: [
              GoRoute(path: '/home', builder: (_, _) => const HomeScreen()),
            ],
          ),

          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/thong-bao',
                builder: (_, _) => const ThongBaoListScreen(),
                routes: [
                  GoRoute(
                    path: 'detail',
                    builder: ThongBaoDetailScreen.fromRoute,
                  ),
                ],
              ),
            ],
          ),

          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/dich-vu',
                builder: (_, _) => const DichVuScreen(),
                routes: [
                  GoRoute(
                    path: 'tien-ich',
                    builder: (_, _) => const TienIchScreen(),
                    routes: [
                      GoRoute(
                        path: 'detail/:id',
                        builder: (context, state) {
                          final int dichVuId = int.parse(
                            state.pathParameters['id']!,
                          );
                          return TienIchDetailScreen(dichVuId: dichVuId);
                        },
                      ),
                      GoRoute(
                        path: 'dang-ky',
                        builder: DangKyTienIchScreen.fromRoute,
                      ),
                    ],
                  ),
                  GoRoute(
                    path: 'sua-chua',
                    builder: (_, _) => const SuaChuaListScreen(),
                    routes: [
                      GoRoute(
                        path: 'detail/:id',
                        builder: (context, state) {
                          final int yeuCauId = int.parse(
                            state.pathParameters['id']!,
                          );
                          return SuaChuaDetailScreen(yeuCauId: yeuCauId);
                        },
                      ),
                      GoRoute(
                        path: 'create',
                        builder: SuaChuaCreateScreen.fromRoute,
                      ),
                    ],
                  ),
                  GoRoute(
                    path: 'thi-cong',
                    builder: (_, _) => const YeuCauThiCongListScreen(),
                    routes: [
                      GoRoute(
                        path: 'detail/:id',
                        builder: (context, state) {
                          final int id = int.parse(state.pathParameters['id']!);
                          return ThiCongDetailScreen(id: id);
                        },
                      ),
                      GoRoute(
                        path: 'form',
                        builder: ThiCongFormScreen.fromRoute,
                      ),
                    ],
                  ),
                  GoRoute(
                    path: 'hoa-don',
                    builder: (_, _) => HoaDonListScreen(),
                    routes: [
                      GoRoute(
                        path: 'detail',
                        builder: HoaDonDetailScreen.fromRoute,
                        routes: [
                          GoRoute(
                            path: 'thanh-toan',
                            builder: ThanhToanScreen.fromRoute,
                          ),
                          GoRoute(
                            path: 'chi-tiet-phi',
                            builder: ChiTietPhiScreen.fromRoute,
                          ),
                        ],
                      ),
                    ],
                  ),
                  GoRoute(
                    path: 'phan-anh',
                    builder: (_, _) => const PhanAnhListScreen(),
                    routes: [
                      GoRoute(
                        path: 'detail/:id',
                        builder: (context, state) {
                          final int phanAnhId = int.parse(
                            state.pathParameters['id']!,
                          );
                          return PhanAnhDetailScreen(phanAnhId: phanAnhId);
                        },
                      ),
                      GoRoute(
                        path: 'create',
                        builder: PhanAnhCreateScreen.fromRoute,
                      ),
                    ],
                  ),
                  GoRoute(
                    path: 'khao-sat',
                    builder: (_, _) => const KhaoSatListScreen(),
                    routes: [
                      GoRoute(
                        path: 'detail',
                        builder: KhaoSatDetailScreen.fromRoute,
                        routes: [
                          GoRoute(
                            path: 'ket-qua/:id',
                            builder: (context, state) {
                              final int khaoSatId = int.parse(
                                state.pathParameters['id']!,
                              );
                              return KhaoSatKetQuaScreen(khaoSatId: khaoSatId);
                            },
                          ),
                        ],
                      ),
                    ],
                  ),
                ],
              ),
            ],
          ),

          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/cu-tru',
                builder: (_, _) => const QuanHeCuTruListScreen(),
                routes: [
                  GoRoute(
                    path: 'thanh-vien',
                    builder: ThanhVienScreen.fromRoute,
                    routes: [
                      GoRoute(
                        path: 'tv-detail',
                        builder: ThanhVienDetailScreen.fromRoute,
                      ),
                      GoRoute(
                        path: 'yc-detail/:id',
                        builder: (_, state) {
                          final int id = int.parse(state.pathParameters['id']!);
                          return YeuCauThanhVienDetailScreen(yeuCauId: id);
                        },
                      ),
                      GoRoute(
                        path: 'yc-form',
                        builder: YeuCauCuTruFormScreen.fromRoute,
                      ),
                      GoRoute(
                        path: 'xoa-yeu-cau',
                        builder: XoaYeuCauThanhVienScreen.fromRoute,
                      ),
                    ],
                  ),
                  GoRoute(
                    path: 'phuong-tien',
                    builder: PhuongTienScreen.fromRoute,
                    routes: [
                      GoRoute(
                        path: 'pt-detail',
                        builder: PhuongTienDetailScreen.fromRoute,
                      ),
                      GoRoute(
                        path: 'tao-yeu-cau',
                        builder: TaoYeuCauPhuongTienScreen.fromRoute,
                      ),
                      GoRoute(
                        path: 'yc-detail',
                        builder: YeuCauPhuongTienDetailScreen.fromRoute,
                      ),
                    ],
                  ),
                ],
              ),
            ],
          ),

          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/profile',
                builder: (_, _) => const ProfileScreen(),
                routes: [
                  GoRoute(
                    path: 'detail',
                    builder: (_, _) => const ProfileDetailScreen(),
                  ),
                  GoRoute(
                    path: 'change-password',
                    builder: (_, _) => const ChangePasswordScreen(),
                  ),
                  GoRoute(
                    path: 'change-avatar',
                    builder: (_, _) => const ChangeAvatarScreen(),
                  ),
                ],
              ),
            ],
          ),
        ],
      ),
    ],

    errorBuilder: (context, state) => AppScaffold(
      title: 'Không tìm thấy trang',
      body: Center(
        child: Text(
          'Không tìm thấy: ${state.uri}',
          style: AppTypography.body.copyWith(color: AppColors.textSecondary),
        ),
      ),
    ),
  );
}
