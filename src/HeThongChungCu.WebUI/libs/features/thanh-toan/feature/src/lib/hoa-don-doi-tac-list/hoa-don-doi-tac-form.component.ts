import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ThanhToanService } from '@features/thanh-toan/data-access';
import { DoiTacApiService } from '@features/doi-tac/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-hoa-don-doi-tac-form',
  templateUrl: './hoa-don-doi-tac-form.component.html',
})
export class HoaDonDoiTacFormComponent implements OnInit {
  @Input() item?: any;
  @Input() doiTacOptions: any[] = [];

  form: FormGroup;
  loading = false;
  loadingContracts = false;
  hopDongOptions: any[] = [];

  formatterVnd = (value: number) =>
    value ? `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '';
  parserVnd = (value: string) => value.replace(/\$\s?|(,*)/g, '');

  constructor(
    private fb: FormBuilder,
    private thanhToanService: ThanhToanService,
    private doiTacApiService: DoiTacApiService,
    public modalRef: NzModalRef,
    private notification: NzNotificationService,
  ) {
    this.form = this.fb.group({
      id: [null],
      doiTacId: [null, [Validators.required]],
      hopDongDoiTacId: [null, [Validators.required]],
      thang: [new Date().getMonth() + 1, [Validators.required]],
      nam: [new Date().getFullYear(), [Validators.required]],
      soTien: [0, [Validators.required, Validators.min(1)]],
      ghiChu: [''],
      fileHoaDonId: [null],
    });
  }

  ngOnInit(): void {
    if (this.item) {
      this.form.patchValue(this.item);
      this.onDoiTacChange(this.item.doiTacId);
    }
  }

  onDoiTacChange(doiTacId: number): void {
    if (!doiTacId) {
      this.hopDongOptions = [];
      return;
    }
    this.loadingContracts = true;
    this.doiTacApiService.getById(doiTacId).subscribe({
      next: (res: ApiResponse<any>) => {
        this.hopDongOptions = res.result?.hopDongs || [];
        this.loadingContracts = false;
      },
      error: () => (this.loadingContracts = false),
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
      ? this.thanhToanService.updateHoaDonDoiTac(val)
      : this.thanhToanService.createHoaDonDoiTac(val);

    obs.subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã lưu hóa đơn đối tác');
          this.modalRef.close(true);
        }
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
