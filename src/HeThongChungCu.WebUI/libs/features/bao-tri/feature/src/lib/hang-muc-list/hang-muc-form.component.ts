import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-hang-muc-form',
  templateUrl: './hang-muc-form.component.html',
})
export class HangMucFormComponent implements OnInit {
  @Input() item?: any;
  form: FormGroup;
  loading = false;

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
      maHangMuc: ['', [Validators.required]],
      tenHangMuc: ['', [Validators.required]],
      moTa: [''],
      thoiGianUocTinhPhut: [30, [Validators.required]],
      chiPhiUocTinh: [0, [Validators.required]],
      checklistTieuChuan: this.fb.array([]),
    });
  }

  get checklistArray(): FormArray {
    return this.form.get('checklistTieuChuan') as FormArray;
  }

  ngOnInit(): void {
    if (this.item) {
      const { checklistTieuChuan, ...other } = this.item;
      this.form.patchValue(other);
      if (checklistTieuChuan && Array.isArray(checklistTieuChuan)) {
        checklistTieuChuan.forEach((item) => this.addChecklistItem(item));
      }
    } else {
      // Default items
      this.addChecklistItem('Kiểm tra ngoại quan');
      this.addChecklistItem('Vệ sinh thiết bị');
    }
  }

  addChecklistItem(value: string = ''): void {
    this.checklistArray.push(this.fb.control(value, [Validators.required]));
  }

  removeChecklistItem(index: number): void {
    this.checklistArray.removeAt(index);
  }

  submit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach((c) => {
        if (c instanceof FormArray) {
          c.controls.forEach((ctrl) => {
            ctrl.markAsDirty();
            ctrl.updateValueAndValidity({ onlySelf: true });
          });
        } else {
          c.markAsDirty();
          c.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    this.loading = true;
    const val = this.form.getRawValue();
    const obs = val.id
      ? this.baoTriService.updateHangMuc(val)
      : this.baoTriService.createHangMuc(val);

    obs.subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã lưu hạng mục bảo trì');
          this.modalRef.close(true);
        }
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
