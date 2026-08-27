import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-phieu-bao-tri-form',
  templateUrl: './phieu-bao-tri-form.component.html',
})
export class PhieuBaoTriFormComponent implements OnInit {
  form: FormGroup;
  loading = false;

  thietBiOptions: any[] = [];
  hangMucOptions: any[] = [];

  constructor(
    private fb: FormBuilder,
    private baoTriService: BaoTriService,
    public modalRef: NzModalRef,
    private notification: NzNotificationService,
  ) {
    this.form = this.fb.group({
      thietBiId: [null, [Validators.required]],
      hangMucBaoTriId: [null, [Validators.required]],
      ngayDuKien: [new Date(), [Validators.required]],
      maPhieu: [''],
      ghiChuXuLy: [''],
      noiDungChecklistBanDaus: [[]],
      nhanSus: [[]],
    });
  }

  ngOnInit(): void {
    this.loadOptions();
  }

  loadOptions(): void {
    this.baoTriService
      .getThietBiList({
        pageSize: 1000,
        pageNumber: 1,
        sortCol: 'id',
        isAsc: false,
      })
      .subscribe((res) => {
        this.thietBiOptions = res.result?.items || [];
      });
    this.baoTriService
      .getHangMucList({
        pageSize: 1000,
        pageNumber: 1,
        sortCol: 'id',
        isAsc: false,
      })
      .subscribe((res) => {
        this.hangMucOptions = res.result?.items || [];
      });
  }

  onThietBiChange(id: number): void {
    // Logic to auto-select hang-muc if needed
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
    this.baoTriService.createPhieuBaoTri(this.form.value).subscribe({
      next: (res) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã lập phiếu bảo trì mới');
          this.modalRef.close(true);
        }
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
