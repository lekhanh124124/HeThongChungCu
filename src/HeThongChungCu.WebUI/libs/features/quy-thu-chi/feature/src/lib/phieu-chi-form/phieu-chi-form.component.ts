import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { QuyThuChiService } from '@features/quy-thu-chi/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalRef } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-phieu-chi-form',
  templateUrl: './phieu-chi-form.component.html',
  styleUrls: ['./phieu-chi-form.component.scss']
})
export class PhieuChiFormComponent implements OnInit {
  form!: FormGroup;
  saving = false;

  constructor(
    private fb: FormBuilder,
    private svc: QuyThuChiService,
    private notification: NzNotificationService,
    private modalRef: NzModalRef
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      ngayGiaoDich: [new Date(), Validators.required],
      phuongThucThanhToanId: [null, Validators.required],
      nguoiGiaoDich: ['', Validators.required],
      chungTuGoc: [''],
      chiTiets: this.fb.array([this.createChiTiet()])
    });
  }

  get chiTiets(): FormArray {
    return this.form.get('chiTiets') as FormArray;
  }

  createChiTiet(): FormGroup {
    return this.fb.group({
      soTien: [0, [Validators.required, Validators.min(1)]],
      nhomThongKe: [''],
      ghiChu: ['']
    });
  }

  addChiTiet(): void {
    this.chiTiets.push(this.createChiTiet());
  }

  removeChiTiet(index: number): void {
    if (this.chiTiets.length > 1) {
      this.chiTiets.removeAt(index);
    }
  }

  formatter = (value: number) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  parser = (value: string) => value.replace(/,/g, '');

  save(): void {
    for (const i in this.form.controls) {
      this.form.controls[i].markAsDirty();
      this.form.controls[i].updateValueAndValidity();
    }
    if (this.form.invalid) return;

    this.saving = true;
    const payload = {
      ...this.form.value,
      ngayGiaoDich: (this.form.value.ngayGiaoDich as Date).toISOString()
    };
    this.svc.taoPhieuChi(payload).subscribe({
      next: (res: ApiResponse<any>) => {
        this.saving = false;
        if (res.isOk) {
          this.notification.success('Thành công', 'Tạo phiếu chi thành công');
          this.modalRef.close(true);
        } else {
          this.notification.error('Lỗi', res.errors?.[0]?.description || 'Tạo phiếu chi thất bại');
        }
      },
      error: () => {
        this.saving = false;
        this.notification.error('Lỗi', 'Tạo phiếu chi thất bại');
      }
    });
  }

  cancel(): void {
    this.modalRef.close();
  }
}
