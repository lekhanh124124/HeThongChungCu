import { Injectable } from '@angular/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { ChangePasswordComponent } from './change-password.component';

@Injectable({ providedIn: 'root' })
export class ChangePasswordModalService {
  constructor(private modal: NzModalService) {}

  open(): void {
    this.modal.create({
      nzContent: ChangePasswordComponent,
      nzFooter: null,
      nzClosable: false,
      nzWidth: 560,
      nzBodyStyle: { padding: '0px', background: 'transparent' },
      nzMaskClosable: true,
      nzWrapClassName: 'no-modal-frame'
    });
  }
}
