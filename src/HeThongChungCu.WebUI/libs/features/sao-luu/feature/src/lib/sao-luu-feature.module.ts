import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SharedUiModule } from '@shared/ui';

import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';

import { SaoLuuListComponent } from './sao-luu-list/sao-luu-list.component';

const routes: Routes = [
  { path: '', component: SaoLuuListComponent }
];

@NgModule({
  declarations: [SaoLuuListComponent],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes),
    SharedUiModule,
    NzTableModule,
    NzButtonModule,
    NzIconModule,
    NzInputModule,
    NzTagModule,
    NzModalModule,
    NzNotificationModule,
    NzDropDownModule,
    NzSpinModule,
    NzEmptyModule,
    NzDividerModule,
    NzPaginationModule
  ]
})
export class SaoLuuFeatureModule {}
