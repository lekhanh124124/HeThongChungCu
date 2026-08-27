import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-lich-bao-tri-form',
  templateUrl: './lich-bao-tri-form.component.html',
})
export class LichBaoTriFormComponent implements OnInit {
  @Input() item?: any;
  @Input() thietBiOptions: any[] = [];
  @Input() hangMucOptions: any[] = [];

  form: FormGroup;
  loading = false;
  tanSuatBaoTriOptions: any[] = [];

  get hasHistory(): boolean {
    return !!this.item?.ngayBaoTriGanNhat;
  }

  constructor(
    private fb: FormBuilder,
    private baoTriService: BaoTriService,
    public modalRef: NzModalRef,
    private notification: NzNotificationService,
  ) {
    this.form = this.fb.group({
      id: [null],
      thietBiId: [null, [Validators.required]],
      hangMucBaoTriId: [null, [Validators.required]],
      tanSuatBaoTriId: [2, [Validators.required]],
      ngayBatDau: [new Date(), [Validators.required]],
      ngayKetThuc: [null, [Validators.required]],
      isActive: [true],
    });
  }

  ngOnInit(): void {
    this.loadTanSuats();
    if (this.item) {
      this.form.patchValue(this.item);
    }
  }

  loadTanSuats(): void {
    this.baoTriService.getTanSuatBaoTriForSelector().subscribe({
      next: (res: ApiResponse<any>) => {
        this.tanSuatBaoTriOptions = res.result || [];
      },
    });
  }

  submit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach((c) => {
        if (c.invalid) {
          c.markAsDirty();
          c.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.loading = true;
    const val = this.form.value;
    const obs = val.id
      ? this.baoTriService.updateLichBaoTri(val)
      : this.baoTriService.createLichBaoTri(val);

    obs.subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.isOk) {
          this.notification.success(
            'Thành công',
            'Đã lưu lịch bảo trì định kỳ',
          );
          this.modalRef.close(true);
        }
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
