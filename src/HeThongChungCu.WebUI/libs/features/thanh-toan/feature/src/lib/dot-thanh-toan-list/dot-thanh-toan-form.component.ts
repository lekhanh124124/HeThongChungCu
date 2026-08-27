import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ThanhToanService } from '@features/thanh-toan/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-dot-thanh-toan-form',
  templateUrl: './dot-thanh-toan-form.component.html',
  styleUrls: ['./dot-thanh-toan-form.component.scss'],
})
export class DotThanhToanFormComponent implements OnInit {
  @Input() item?: any;
  form: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private thanhToanService: ThanhToanService,
    public modalRef: NzModalRef,
    private notification: NzNotificationService,
  ) {
    this.form = this.fb.group({
      id: [null],
      thang: [new Date().getMonth() + 1, [Validators.required]],
      nam: [new Date().getFullYear(), [Validators.required]],
      tenDot: [''],
      ghiChu: [''],
    });
  }

  ngOnInit(): void {
    if (this.item) {
      this.form.patchValue(this.item);
    }
  }

  submit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    const val = this.form.value;
    const obs = val.id
      ? this.thanhToanService.updateDotThanhToan(val)
      : this.thanhToanService.createDotThanhToan(val);

    obs.subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã lưu đợt thanh toán');
          this.modalRef.close(true);
        }
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
