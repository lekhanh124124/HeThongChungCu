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
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzRateModule } from 'ng-zorro-antd/rate';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';

import { PhanAnhListComponent } from './phan-anh-list/phan-anh-list.component';
import { PhanAnhDetailComponent } from './phan-anh-detail/phan-anh-detail.component';

const routes: Routes = [
  { path: '', component: PhanAnhListComponent }
];

@NgModule({
  declarations: [
    PhanAnhListComponent,
    PhanAnhDetailComponent
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
    NzDropDownModule,
    NzSpinModule,
    NzEmptyModule,
    NzAvatarModule,
    NzImageModule,
    NzRateModule,
    NzDrawerModule
  ]
})
export class PhanAnhFeatureModule { }
