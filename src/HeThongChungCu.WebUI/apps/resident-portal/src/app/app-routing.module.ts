import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@core/guards';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'auth/login' },

  {
    path: 'auth',
    loadChildren: () =>
      import('@features/auth/feature').then((m) => m.AuthFeatureModule)
  },

  {
    path: 'dashboard',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('@features/dashboard/feature').then((m) => m.DashboardFeatureModule)
  },

  {
    path: 'resident',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('@features/resident/feature').then((m) => m.ResidentFeatureModule)
  },

  {
    path: 'doi-tac',
    canActivate: [AuthGuard],
    loadChildren: () => import('../../../../libs/features/doi-tac/feature/src/lib').then((m) => m.DoiTacFeatureModule)
  },

  {
    path: 'profile',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('../../../../libs/features/profile/feature/src/lib').then((m) => m.ProfileFeatureModule)
  },

  { path: '**', redirectTo: 'auth/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
