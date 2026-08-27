import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedUiModule } from '@shared/ui';

import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzFormModule } from 'ng-zorro-antd/form';

import { QuyThuChiListComponent } from './quy-thu-chi-list/quy-thu-chi-list.component';
import { PhieuThuFormComponent } from './phieu-thu-form/phieu-thu-form.component';
import { PhieuChiFormComponent } from './phieu-chi-form/phieu-chi-form.component';
import { GiaoDichDetailComponent } from './giao-dich-detail/giao-dich-detail.component';
import { BaoCaoComponent } from './bao-cao/bao-cao.component';

const routes: Routes = [
  { path: '', component: QuyThuChiListComponent },
  { path: 'bao-cao', component: BaoCaoComponent }
];

@NgModule({
  declarations: [
    QuyThuChiListComponent,
    PhieuThuFormComponent,
    PhieuChiFormComponent,
    GiaoDichDetailComponent,
    BaoCaoComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    SharedUiModule,
    NzTableModule, NzButtonModule, NzIconModule, NzInputModule,
    NzSelectModule, NzTagModule, NzModalModule, NzNotificationModule,
    NzDatePickerModule, NzDropDownModule, NzSpinModule, NzEmptyModule,
    NzInputNumberModule, NzDrawerModule, NzTabsModule, NzDescriptionsModule,
    NzDividerModule, NzCardModule, NzPaginationModule, NzFormModule
  ]
})
export class QuyThuChiFeatureModule {}
