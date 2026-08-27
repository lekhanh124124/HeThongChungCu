import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzTreeViewModule } from 'ng-zorro-antd/tree-view';
import { NzImageModule } from 'ng-zorro-antd/image';
import { SharedUiModule } from '@shared/ui';
import { CauTrucChungCuTreeModule } from '@features/resident/feature';

import { DichVuListComponent } from './dich-vu-list/dich-vu-list.component';
import { DichVuFormComponent } from './dich-vu-form/dich-vu-form.component';
import { BangGiaListComponent } from './bang-gia-list/bang-gia-list.component';
import { BangGiaFormComponent } from './bang-gia-form/bang-gia-form.component';
import { DichVuDetailComponent } from './dich-vu-detail/dich-vu-detail.component';
import { KhungGioListComponent } from './khung-gio-list/khung-gio-list.component';
import { KhungGioFormComponent } from './khung-gio-form/khung-gio-form.component';
import { ChiSoTieuThuListComponent } from './chi-so-tieu-thu/chi-so-tieu-thu-list/chi-so-tieu-thu-list.component';
import { ChiSoTieuThuManualComponent } from './chi-so-tieu-thu/chi-so-tieu-thu-manual/chi-so-tieu-thu-manual.component';
import { ChiSoTieuThuImportComponent } from './chi-so-tieu-thu/chi-so-tieu-thu-import/chi-so-tieu-thu-import.component';
import { ChiSoTieuThuFormComponent } from './chi-so-tieu-thu/chi-so-tieu-thu-form/chi-so-tieu-thu-form.component';

const routes: Routes = [
  { path: '', component: DichVuListComponent },
  { path: 'create', component: DichVuFormComponent },
  { path: ':id/edit', component: DichVuFormComponent },
  { path: 'chi-so-tieu-thu', component: ChiSoTieuThuListComponent },
  { path: 'chi-so-tieu-thu/ghi-nhan', component: ChiSoTieuThuManualComponent },
];

@NgModule({
  declarations: [
    DichVuListComponent,
    DichVuFormComponent,
    DichVuDetailComponent,
    BangGiaListComponent,
    BangGiaFormComponent,
    KhungGioListComponent,
    KhungGioFormComponent,
    ChiSoTieuThuListComponent,
    ChiSoTieuThuManualComponent,
    ChiSoTieuThuImportComponent,
    ChiSoTieuThuFormComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes),
    NzTabsModule,
    NzTableModule,
    NzButtonModule,
    NzModalModule,
    NzInputModule,
    NzFormModule,
    NzSelectModule,
    NzCheckboxModule,
    NzDropDownModule,
    NzPopconfirmModule,
    NzPaginationModule,
    NzDescriptionsModule,
    NzTagModule,
    NzSpinModule,
    NzStepsModule,
    NzInputNumberModule,
    NzEmptyModule,
    NzToolTipModule,
    NzIconModule,
    NzDatePickerModule,
    NzUploadModule,
    NzTreeViewModule,
    NzImageModule,
    CauTrucChungCuTreeModule,
    SharedUiModule
  ],
  exports: [DichVuListComponent]
})
export class DichVuFeatureModule {}
