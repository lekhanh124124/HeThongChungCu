import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { SharedUiModule } from '@shared/ui';
import { HttpClientModule } from '@angular/common/http';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { ReactiveFormsModule } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { FormsModule } from '@angular/forms';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ChangePasswordModalService } from './change-password/change-password-modal.service';

const routes: Routes = [
  { path: '', component: ProfileComponent }
];

@NgModule({
  declarations: [ProfileComponent, ChangePasswordComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedUiModule,
    HttpClientModule,
    NzModalModule,
    NzDescriptionsModule,
    NzButtonModule,
    NzCardModule,
    NzAvatarModule,
    NzTagModule,
    NzGridModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzInputNumberModule
    ,
    NzSelectModule,
    NzRadioModule,
    NzSpinModule,
    FormsModule
  ]
  ,
  providers: [ChangePasswordModalService]
})
export class ProfileFeatureModule {}
