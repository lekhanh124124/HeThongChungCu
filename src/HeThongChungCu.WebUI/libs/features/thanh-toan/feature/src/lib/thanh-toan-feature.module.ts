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
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';

import { DotThanhToanListComponent } from './dot-thanh-toan-list/dot-thanh-toan-list.component';
import { DotThanhToanFormComponent } from './dot-thanh-toan-list/dot-thanh-toan-form.component';
import { HoaDonListComponent } from './hoa-don-list/hoa-don-list.component';
import { HoaDonDetailComponent } from './hoa-don-list/hoa-don-detail.component';
import { HoaDonDoiTacListComponent } from './hoa-don-doi-tac-list/hoa-don-doi-tac-list.component';
import { HoaDonDoiTacFormComponent } from './hoa-don-doi-tac-list/hoa-don-doi-tac-form.component';

const routes: Routes = [
  { path: 'dot-thanh-toan', component: DotThanhToanListComponent },
  { path: 'hoa-don', component: HoaDonListComponent },
  { path: 'hoa-don-doi-tac', component: HoaDonDoiTacListComponent },
];

@NgModule({
  declarations: [
    DotThanhToanListComponent,
    DotThanhToanFormComponent,
    HoaDonListComponent,
    HoaDonDetailComponent,
    HoaDonDoiTacListComponent,
    HoaDonDoiTacFormComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    SharedUiModule,
    NzTableModule,
    NzButtonModule,
    NzIconModule,
    NzInputModule,
    NzSelectModule,
    NzTagModule,
    NzModalModule,
    NzNotificationModule,
    NzDatePickerModule,
    NzTabsModule,
    NzDescriptionsModule,
    NzBadgeModule,
    NzDropDownModule,
    NzSpinModule,
    NzToolTipModule,
    NzPopconfirmModule,
    NzEmptyModule,
    NzFormModule,
    NzInputNumberModule,
    NzDrawerModule
  ]
})
export class ThanhToanFeatureModule {}
