import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DashboardHomeComponent } from './dashboard/home/dashboard-home.component';
import { SharedUiModule } from '@shared/ui';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NotificationBellComponent } from './notification-bell/notification-bell.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { FormsModule } from '@angular/forms';

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: DashboardHomeComponent },
      { path: 'notifications', component: NotificationsComponent },
      { path: 'resident', loadChildren: () => import('@features/resident/feature').then((m) => m.ResidentFeatureModule) },
      { path: 'profile', loadChildren: () => import('@features/profile/feature').then((m) => m.ProfileFeatureModule) },
      { path: 'doi-tac', loadChildren: () => import('@features/doi-tac/feature').then((m) => m.DoiTacFeatureModule) },
      { path: 'dich-vu', loadChildren: () => import('@features/dich-vu/feature').then((m) => m.DichVuFeatureModule) },
      { path: 'thanh-toan', loadChildren: () => import('@features/thanh-toan/feature').then((m) => m.ThanhToanFeatureModule) },
      { path: 'bao-tri', loadChildren: () => import('@features/bao-tri/feature').then((m) => m.BaoTriFeatureModule) },
      { path: 'khao-sat', loadChildren: () => import('@features/khao-sat/feature').then((m) => m.KhaoSatFeatureModule) },
      { path: 'phan-anh', loadChildren: () => import('@features/phan-anh/feature').then((m) => m.PhanAnhFeatureModule) },
      { path: 'sao-luu', loadChildren: () => import('@features/sao-luu/feature').then((m) => m.SaoLuuFeatureModule) },
      { path: 'quy-thu-chi', loadChildren: () => import('@features/quy-thu-chi/feature').then((m) => m.QuyThuChiFeatureModule) },
      { path: 'tri-thuc', loadChildren: () => import('@features/tri-thuc/feature').then((m) => m.TriThucFeatureModule) }
    ]
  }
];

@NgModule({
  declarations: [DashboardComponent, DashboardHomeComponent, NotificationBellComponent, NotificationsComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedUiModule,
    FormsModule,
    NzLayoutModule,
    NzMenuModule,
    NzIconModule,
    NzDropDownModule,
    NzBadgeModule,
    NzListModule,
    NzEmptyModule,
    NzButtonModule,
    NzAvatarModule,
    NzSpinModule,
    NzNotificationModule,
    NzInputModule,
    NzSelectModule,
    NzTagModule,
    NzPaginationModule,
    NzCardModule,
    NzDatePickerModule
  ]
})
export class DashboardFeatureModule {}
