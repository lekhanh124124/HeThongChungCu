import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TriThucRoutingModule } from './tri-thuc-routing.module';
import { TriThucListComponent } from './tri-thuc-list/tri-thuc-list.component';
import { TriThucFormComponent } from './tri-thuc-form/tri-thuc-form.component';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { SharedUiModule } from '@shared/ui';

@NgModule({
  declarations: [TriThucListComponent, TriThucFormComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    TriThucRoutingModule,
    NzModalModule,
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzSelectModule,
    NzPaginationModule,
    NzTableModule,
    NzDropDownModule,
    NzMenuModule,
    NzCheckboxModule,
    NzNotificationModule,
    NzInputNumberModule,
    NzSwitchModule,
    NzIconModule,
    SharedUiModule,
  ],
})
export class TriThucFeatureModule {}
