import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-thiet-bi-form',
  templateUrl: './thiet-bi-form.component.html',
})
export class ThietBiFormComponent implements OnInit {
  @Input() item?: any;
  form: FormGroup;
  loading = false;
  trangThaiThietBiOptions: any[] = [];

  formatterVnd = (value: number) =>
    value ? `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '';
  parserVnd = (value: string) => value.replace(/\$\s?|(,*)/g, '');

  constructor(
    private fb: FormBuilder,
    private baoTriService: BaoTriService,
    public modalRef: NzModalRef,
    private notification: NzNotificationService,
  ) {
    this.form = this.fb.group({
      id: [null],
      maThietBi: ['', [Validators.required]],
      tenThietBi: ['', [Validators.required]],
      loaiThietBi: ['', [Validators.required]],
      viTri: ['', [Validators.required]],
      ngayMua: [null],
      ngayHetHanBaoHanh: [null],
      giaTriBanDau: [0],
      trangThaiThietBiId: [1],
      ghiChu: [''],
    });
  }

  ngOnInit(): void {
    this.loadTrangThais();
    if (this.item) {
      this.form.patchValue(this.item);
      this.form.get('maThietBi')?.disable();
    }
  }

  loadTrangThais(): void {
    this.baoTriService.getTrangThaiThietBiForSelector().subscribe((res) => {
      this.trangThaiThietBiOptions = res.result || [];
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
      ? this.baoTriService.updateThietBi(val)
      : this.baoTriService.createThietBi(val);

    obs.subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã lưu thông tin thiết bị');
          this.modalRef.close(true);
        }
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
