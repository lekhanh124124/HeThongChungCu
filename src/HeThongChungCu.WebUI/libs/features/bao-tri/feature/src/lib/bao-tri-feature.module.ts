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
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTimelineModule } from 'ng-zorro-antd/timeline';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';

import { ThietBiListComponent } from './thiet-bi-list/thiet-bi-list.component';
import { ThietBiFormComponent } from './thiet-bi-list/thiet-bi-form.component';
import { HangMucListComponent } from './hang-muc-list/hang-muc-list.component';
import { HangMucFormComponent } from './hang-muc-list/hang-muc-form.component';
import { LichBaoTriListComponent } from './lich-bao-tri-list/lich-bao-tri-list.component';
import { LichBaoTriFormComponent } from './lich-bao-tri-list/lich-bao-tri-form.component';
import { PhieuBaoTriListComponent } from './phieu-bao-tri-list/phieu-bao-tri-list.component';
import { PhieuBaoTriDetailComponent } from './phieu-bao-tri-list/phieu-bao-tri-detail.component';
import { PhieuBaoTriFormComponent } from './phieu-bao-tri-list/phieu-bao-tri-form.component';

const routes: Routes = [
  { path: 'thiet-bi', component: ThietBiListComponent },
  { path: 'hang-muc', component: HangMucListComponent },
  { path: 'lich-bao-tri', component: LichBaoTriListComponent },
  { path: 'phieu-bao-tri', component: PhieuBaoTriListComponent },
];

@NgModule({
  declarations: [
    ThietBiListComponent,
    ThietBiFormComponent,
    HangMucListComponent,
    HangMucFormComponent,
    LichBaoTriListComponent,
    LichBaoTriFormComponent,
    PhieuBaoTriListComponent,
    PhieuBaoTriDetailComponent,
    PhieuBaoTriFormComponent
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
    NzDividerModule,
    NzCardModule,
    NzTimelineModule,
    NzStepsModule,
    NzCheckboxModule,
    NzSpaceModule,
    NzDrawerModule
  ]
})
export class BaoTriFeatureModule {}
