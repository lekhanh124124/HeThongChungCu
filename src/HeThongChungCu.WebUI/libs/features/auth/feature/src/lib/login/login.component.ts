import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '@core/auth/data-access';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  form: FormGroup;
  showPassword = false;
  loading = false;
  submitted = false;
  waiting = false;
  pendingTooltip = 'Đang xác thực...';
  errorMessages: string[] = [];
  warningMessages: string[] = [];
  usernameErrors: string[] = [];
  passwordErrors: string[] = [];
  showForgotModal = false;
  forgotStep = 1;
  forgotLoading = false;
  forgotFormStep1: FormGroup;
  forgotFormStep2: FormGroup;
  forgotErrors: string[] = [];
  forgotMessages: string[] = [];
  forgotUsername = '';
  showNewPassword = false;
  showConfirmNewPassword = false;
  private errorClearTimer: any = null;


  constructor(private fb: FormBuilder, private router: Router, private auth: AuthService) {
    this.form = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', Validators.required]
    });

    this.forgotFormStep1 = this.fb.group({
      username: ['', [Validators.required]]
    });

    this.forgotFormStep2 = this.fb.group({
      resetCode: ['', [Validators.required]],
      newPassword: ['', [Validators.required]],
      confirmNewPassword: ['', [Validators.required]]
    });
  }

  private extractErrors(body: any): string[] {
    const out: string[] = [];
    if (!body) return out;
    if (body.errors) {
      if (Array.isArray(body.errors)) {
        body.errors.forEach((e: any) => {
          const desc: string = (e?.description || e?.code || '').toString();
          if (desc) out.push(desc);
        });
      } else if (typeof body.errors === 'object') {
        Object.keys(body.errors).forEach((k) => {
          const v = body.errors[k];
          if (Array.isArray(v)) v.forEach((m: any) => out.push(String(m)));
          else if (v) out.push(String(v));
        });
      }
    }
    if (body.result && typeof body.result === 'string') out.push(body.result);
    return out;
  }

  openForgotPassword() {
    this.showForgotModal = true;
    this.forgotStep = 1;
    this.forgotErrors = [];
    this.forgotMessages = [];
    const usernameCtrl = this.form.get('username');
    const username = (usernameCtrl?.value || '').toString().trim();
    this.forgotFormStep1.patchValue({ username });
  }

  closeForgotPassword() {
    this.showForgotModal = false;
    this.forgotStep = 1;
    this.forgotFormStep1.reset();
    this.forgotFormStep2.reset();
    this.forgotErrors = [];
    this.forgotMessages = [];
    this.forgotUsername = '';
  }

  submitForgotStep1() {
    this.forgotErrors = [];
    this.forgotMessages = [];
    this.forgotFormStep1.markAllAsTouched();
    const usernameCtrl = this.forgotFormStep1.get('username');
    const username = (usernameCtrl?.value || '').toString().trim();
    if (usernameCtrl && username !== usernameCtrl.value) {
      usernameCtrl.setValue(username, { emitEvent: false });
    }
    this.forgotLoading = true;
    this.auth.forgotPassword(username).subscribe({
      next: (res: any) => {
        this.forgotLoading = false;
        if (!res) return;
        if (res.errors && res.errors.length) {
          this.forgotErrors.push(...this.extractErrors(res));
          return;
        }
        if (res.result && typeof res.result === 'string') this.forgotMessages.push(res.result);
        this.forgotUsername = username;
        this.forgotStep = 2;
      },
      error: (err: any) => {
        this.forgotLoading = false;
        const body = err?.error;
        if (body) this.forgotErrors.push(...this.extractErrors(body));
      }
    });
  }

  submitForgotStep2() {
    this.forgotErrors = [];
    this.forgotMessages = [];
    const resetCodeCtrl = this.forgotFormStep2.get('resetCode');
    const newPasswordCtrl = this.forgotFormStep2.get('newPassword');
    const confirmNewPasswordCtrl = this.forgotFormStep2.get('confirmNewPassword');
    const resetCode = (resetCodeCtrl?.value || '').toString().trim();
    const newPassword = (newPasswordCtrl?.value || '').toString().trim();
    const confirmNewPassword = (confirmNewPasswordCtrl?.value || '').toString().trim();
    if (resetCodeCtrl && resetCode !== resetCodeCtrl.value) resetCodeCtrl.setValue(resetCode, { emitEvent: false });
    if (newPasswordCtrl && newPassword !== newPasswordCtrl.value) newPasswordCtrl.setValue(newPassword, { emitEvent: false });
    if (confirmNewPasswordCtrl && confirmNewPassword !== confirmNewPasswordCtrl.value) confirmNewPasswordCtrl.setValue(confirmNewPassword, { emitEvent: false });
    this.forgotFormStep2.markAllAsTouched();

    
    this.forgotLoading = true;
    this.auth.resetPassword(this.forgotUsername, resetCode, newPassword, confirmNewPassword).subscribe({
      next: (res: any) => {
        this.forgotLoading = false;
        if (!res) return;
        if (res.errors && res.errors.length) {
          this.forgotErrors.push(...this.extractErrors(res));
          return;
        }
        if (res.isOk === false) {
          if (res.result && typeof res.result === 'string') this.forgotErrors.push(res.result);
          return;
        }
        if (res.result && typeof res.result === 'string') this.forgotMessages.push(res.result);
        this.forgotStep = 3;
      },
      error: (err: any) => {
        this.forgotLoading = false;
        const body = err?.error;
        if (body) this.forgotErrors.push(...this.extractErrors(body));
      }
    });
  }

  submit() {
    this.errorMessages = [];
    this.warningMessages = [];
    this.usernameErrors = [];
    this.passwordErrors = [];

    const usernameCtrl = this.form.get('username');
    const pwCtrl = this.form.get('password');
    if (usernameCtrl) {
      const trimmed = (usernameCtrl.value || '').toString().trim();
      if (trimmed !== usernameCtrl.value) usernameCtrl.setValue(trimmed, { emitEvent: false });
      usernameCtrl.updateValueAndValidity();
    }
    if (pwCtrl) {
      const trimmedPw = (pwCtrl.value || '').toString().trim();
      if (trimmedPw !== pwCtrl.value) pwCtrl.setValue(trimmedPw, { emitEvent: false });
      pwCtrl.updateValueAndValidity();
    }

    this.submitted = true;
    this.form.markAllAsTouched();

    this.waiting = true;
    this.usernameErrors = [];
    this.passwordErrors = [];

    this.loading = true;
    const { username, password } = this.form.value;
    this.auth.login(username, password).subscribe({
      next: (res: any) => {
        this.loading = false;
        if (!res) {
          this.errorMessages.push('Lỗi kết nối');
          return;
        }
        this.waiting = false;
        if (res.warningMessages && res.warningMessages.length) this.warningMessages = res.warningMessages;
        if (res.errors && res.errors.length) {
          this.usernameErrors = [];
          this.passwordErrors = [];
          this.errorMessages = [];
          const errs = this.extractErrors(res);
          errs.forEach((desc: string) => {
            const lower = desc.toLowerCase();
            const hasUsername = lower.includes('username') || lower.includes('tài khoản') || lower.includes('tên đăng nhập');
            const hasPassword = lower.includes('password') || lower.includes('mật khẩu');
            if (hasUsername && hasPassword) {
              this.errorMessages.push(desc);
            } else if (hasUsername) {
              this.usernameErrors.push(desc);
            } else if (hasPassword) {
              this.passwordErrors.push(desc);
            } else {
              this.errorMessages.push(desc);
            }
          });
          this.scheduleClearErrors(5000);
          return;
        }
        if (res.isOk === false && res.result && typeof res.result === 'string') {
          this.errorMessages.push(res.result);
          return;
        }
        if (res.isOk || (res.result && (res.result.accessToken || res.result.userId))) {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (err: any) => {
        this.loading = false;
        this.waiting = false;
        const body = err?.error;
        if (body) {
          if (body.warningMessages) this.warningMessages = body.warningMessages;
          const errs = this.extractErrors(body);
          this.usernameErrors = [];
          this.passwordErrors = [];
          this.errorMessages = [];
          errs.forEach((desc: string) => {
            const lower = desc.toLowerCase();
            const hasUsername = lower.includes('username') || lower.includes('tài khoản') || lower.includes('tên đăng nhập');
            const hasPassword = lower.includes('password') || lower.includes('mật khẩu');
            if (hasUsername && hasPassword) {
              this.errorMessages.push(desc);
            } else if (hasUsername) {
              this.usernameErrors.push(desc);
            } else if (hasPassword) {
              this.passwordErrors.push(desc);
            } else {
              this.errorMessages.push(desc);
            }
          });
          this.scheduleClearErrors(3000);
        }
      }
    });
  }

  private scheduleClearErrors(ms: number) {
    if (this.errorClearTimer) {
      clearTimeout(this.errorClearTimer);
      this.errorClearTimer = null;
    }
    this.errorClearTimer = setTimeout(() => {
      this.usernameErrors = [];
      this.passwordErrors = [];
      this.errorMessages = [];
      this.errorClearTimer = null;
    }, ms);
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  toggleNewPassword() {
    this.showNewPassword = !this.showNewPassword;
  }

  toggleConfirmNewPassword() {
    this.showConfirmNewPassword = !this.showConfirmNewPassword;
  }
}
