import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzTreeViewModule } from 'ng-zorro-antd/tree-view';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { QuanLyCuDanComponent } from './quan-ly-cu-dan/quan-ly-cu-dan.component';
import { ToaNhaListComponent } from './toa-nha/toa-nha-list.component';
import { ToaNhaFormComponent } from './toa-nha/toa-nha-form.component';
import { CanHoListComponent } from './can-ho/can-ho-list.component';
import { CanHoFormComponent } from './can-ho/can-ho-form.component';
import { CanHoResidentsComponent } from './can-ho/can-ho-residents.component';
import { TangFormComponent } from './tang/tang-form.component';
import { CauTrucChungCuTreeModule } from './cau-truc-chung-cu-tree/cau-truc-chung-cu-tree.module';
import { ResidentLayoutComponent } from './resident-layout/resident-layout.component';
import { TangListComponent } from './tang/tang-list.component';
import { QuanHeCuTruComponent } from './quan-he-cu-tru/quan-he-cu-tru.component';
import { QuanHeCuTruFormComponent } from './quan-he-cu-tru/quan-he-cu-tru-form.component';
import { HoSoCuDanComponent} from './quan-he-cu-tru/ho-so-cu-dan.component';
import { YeuCauCuTruListComponent } from './yeu-cau-cu-dan/yeu-cau-cu-tru-list.component';
import { YeuCauCuDanComponent } from './yeu-cau-cu-dan/yeu-cau-cu-dan.component';
import { YeuCauPhuongTienListComponent } from './yeu-cau-cu-dan/yeu-cau-phuong-tien-list.component';
import { PhuongTienListComponent } from './phuong-tien/phuong-tien-list.component';
import { PhuongTienFormComponent } from './phuong-tien/phuong-tien-form.component';
import { TaoTheFormComponent } from './phuong-tien/tao-the-form.component';
import { NhanVienListComponent } from './nhan-vien/nhan-vien-list.component';
import { NhanVienFormComponent } from './nhan-vien/nhan-vien-form.component';
import { YeuCauSuaChuaListComponent } from './yeu-cau-cu-dan/yeu-cau-sua-chua-list.component';
import { YeuCauThiCongListComponent } from './yeu-cau-cu-dan/yeu-cau-thi-cong-list.component';
import { SharedUiModule } from '@shared/ui';

const routes: Routes = [
  {
    path: '',
    component: ResidentLayoutComponent,
    children: [
      { path: '', component: QuanLyCuDanComponent },
      { path: 'quan-he-cu-tru', component: QuanHeCuTruComponent },
      { path: 'requests', component: YeuCauCuDanComponent },
      { path: 'thi-cong', component: YeuCauThiCongListComponent },
      { path: 'phuong-tien', component: PhuongTienListComponent },
      { path: 'phuong-tien/create', component: PhuongTienFormComponent },
      { path: 'phuong-tien/:id/edit', component: PhuongTienFormComponent },
      { path: 'toa-nha', component: ToaNhaListComponent },
      { path: 'tang', component: TangListComponent },
      { path: 'toa-nha/create', component: ToaNhaFormComponent },
      { path: 'toa-nha/:id/edit', component: ToaNhaFormComponent },
      { path: 'can-ho', component: CanHoListComponent },
      { path: 'can-ho/create', component: CanHoFormComponent },
      { path: 'can-ho/:id/edit', component: CanHoFormComponent },
      { path: 'can-ho/:id', component: CanHoResidentsComponent },
      { path: 'nhan-vien', component: NhanVienListComponent },
      { path: 'nhan-vien/create', component: NhanVienFormComponent },
      { path: 'nhan-vien/:id/edit', component: NhanVienFormComponent }
    ]
  }
];

@NgModule({
  declarations: [
    QuanLyCuDanComponent,
    ToaNhaListComponent,
    ToaNhaFormComponent,
    CanHoListComponent,
    CanHoFormComponent,
    CanHoResidentsComponent,
    TangFormComponent,
    ResidentLayoutComponent,
    TangListComponent,
    QuanHeCuTruComponent,
    QuanHeCuTruFormComponent,
    HoSoCuDanComponent,
    YeuCauCuTruListComponent,
    YeuCauCuDanComponent,
    YeuCauPhuongTienListComponent,
    PhuongTienListComponent,
    PhuongTienFormComponent,
    TaoTheFormComponent,
    NhanVienListComponent,
    NhanVienFormComponent,
    YeuCauSuaChuaListComponent,
    YeuCauThiCongListComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HttpClientModule,
    RouterModule.forChild(routes),
    NzFormModule,
    NzInputModule,
    NzInputNumberModule,
    NzButtonModule,
    NzIconModule,
    NzDropDownModule,
    NzMenuModule,
    NzTableModule,
    NzPaginationModule,
    NzModalModule,
    NzGridModule,
    NzSelectModule,
    NzRadioModule,
    NzCheckboxModule,
    NzTreeViewModule,
    NzTabsModule,
    FormsModule,
    NzNotificationModule,
    NzTagModule,
    NzCardModule,
    NzEmptyModule,
    NzUploadModule,
    NzStepsModule,
    NzDatePickerModule,
    NzDrawerModule,
    NzSpinModule,
    NzBadgeModule,
    CauTrucChungCuTreeModule,
    SharedUiModule
  ],
  providers: []
})
export class ResidentFeatureModule {}
