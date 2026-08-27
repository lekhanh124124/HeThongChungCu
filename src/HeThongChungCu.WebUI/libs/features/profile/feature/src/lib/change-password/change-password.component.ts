import { Component } from '@angular/core';
import { ProfileApiService } from '../../../../data-access/src/lib/profile-api.service';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent {
  model = { oldPassword: '', newPassword: '', confirmPassword: '' };
  loading = false;
  error: string | null = null;
  showOld = false;
  showNew = false;
  showConfirm = false;

  constructor(private api: ProfileApiService, private modal: NzModalRef, private notification: NzNotificationService) {}

  submit(): void {
    this.error = null;
    const m = this.model;
    if (!m.oldPassword) { this.error = 'Nhập mật khẩu cũ'; return; }
    if (!m.newPassword) { this.error = 'Nhập mật khẩu mới'; return; }
    this.loading = true;
    this.api.changePassword(m).subscribe({
      next: (r: any) => {
        this.loading = false;
        if (r && r.isOk) {
          const msg = (r && r.result) ? String(r.result) : 'Đổi mật khẩu thành công.';
          this.notification.success('Thành công', msg);
          this.modal.close(r);
        } else {
          if (r && Array.isArray(r.errors) && r.errors.length) {
            this.error = r.errors[0].description || r.errors[0].message || 'Đổi mật khẩu không thành công';
          } else {
            this.error = r?.result || 'Đổi mật khẩu không thành công';
          }
        }
      },
      error: (err: any) => {
        this.loading = false;
        const body = err?.error;
        if (body && body.message) {
          this.error = body.message;
          return;
        }
        if (err && err.message) {
          this.error = err.message;
          return;
        }
        this.error = 'Lỗi khi gọi API';
      }
    });
  }

  cancel(): void { this.modal.destroy(); }
}
