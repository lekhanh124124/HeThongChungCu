import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DoiTacRoutingModule } from './doi-tac-routing.module';
import { DoiTacListComponent } from './doi-tac-list/doi-tac-list.component';
import { DoiTacCreateComponent } from './doi-tac-form/doi-tac-form.component';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { SharedUiModule } from '@shared/ui';

@NgModule({
  declarations: [DoiTacListComponent, DoiTacCreateComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    DoiTacRoutingModule,
    NzModalModule,
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzCardModule,
    NzPaginationModule,
    NzTableModule,
    NzDropDownModule,
    NzMenuModule,
    NzCheckboxModule,
    NzNotificationModule,
    NzInputNumberModule,
    NzIconModule,
    SharedUiModule,
  ],
})
export class DoiTacFeatureModule {}
