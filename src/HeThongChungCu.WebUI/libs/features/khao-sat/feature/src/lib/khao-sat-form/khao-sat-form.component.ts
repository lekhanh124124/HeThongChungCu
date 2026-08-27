import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { KhaoSatService } from '@features/khao-sat/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalRef } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-khao-sat-form',
  templateUrl: './khao-sat-form.component.html',
  styleUrls: ['./khao-sat-form.component.scss']
})
export class KhaoSatFormComponent implements OnInit {
  form: FormGroup;
  loading = false;
  isEdit = false;
  item: any;

  constructor(
    private fb: FormBuilder,
    private khaoSatService: KhaoSatService,
    private notification: NzNotificationService,
    private modalRef: NzModalRef
  ) {
    this.form = this.fb.group({
      id: [null],
      tieuDe: ['', [Validators.required, Validators.maxLength(250)]],
      moTa: ['', [Validators.required]],
      loaiKhaoSatId: [1, [Validators.required]],
      coCheTinhDiemId: [1, [Validators.required]],
      ngayBatDau: [null, [Validators.required]],
      ngayKetThuc: [null, [Validators.required]],
      tyleThamGiaToiThieu: [50, [Validators.required, Validators.min(0), Validators.max(100)]],
      tyLeDongYToiThieu: [50, [Validators.required, Validators.min(0), Validators.max(100)]],
      isAnDanh: [false],
      cauHois: this.fb.array([])
    });
  }

  ngOnInit(): void {
    if (this.item) {
      this.isEdit = true;
      this.patchForm(this.item);
    } else {
      this.addQuestion(); // Add one default question
    }
  }

  get cauHois(): FormArray {
    return this.form.get('cauHois') as FormArray;
  }

  asFormGroup(control: any): FormGroup {
    return control as FormGroup;
  }

  patchForm(data: any): void {
    this.form.patchValue(data);
    if (data.cauHois) {
      data.cauHois.forEach((q: any) => {
        const qForm = this.addQuestion();
        qForm.patchValue(q);
        if (q.luaChons) {
          q.luaChons.forEach((l: any) => {
            const lForm = this.addOption(qForm);
            lForm.patchValue(l);
          });
        }
      });
    }
  }

  addQuestion(): FormGroup {
    const q = this.fb.group({
      id: [null],
      noiDungCauHoi: ['', [Validators.required]],
      isBatBuoc: [true],
      isMultiSelect: [false],
      luaChons: this.fb.array([])
    });
    this.cauHois.push(q);
    this.addOption(q); // Add one default option
    this.addOption(q); // Add another default option
    return q;
  }

  removeQuestion(index: number): void {
    this.cauHois.removeAt(index);
  }

  getOptions(question: FormGroup): FormArray {
    return question.get('luaChons') as FormArray;
  }

  addOption(question: FormGroup): FormGroup {
    const options = this.getOptions(question);
    const o = this.fb.group({
      id: [null],
      noiDungLuaChon: ['', [Validators.required]],
      isUngVienBQT: [false],
      tieuSuUngVien: [null],
      ungVienId: [null]
    });
    options.push(o);
    return o;
  }

  removeOption(question: FormGroup, index: number): void {
    const options = this.getOptions(question);
    options.removeAt(index);
  }

  submit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.notification.warning('Thông báo', 'Vui lòng kiểm tra lại thông tin form');
      return;
    }

    this.loading = true;
    const payload = this.form.value;
    const obs = this.isEdit ? this.khaoSatService.update(payload) : this.khaoSatService.create(payload);

    obs.subscribe({
      next: (res) => {
        if (res.isOk) {
          this.notification.success('Thành công', this.isEdit ? 'Đã cập nhật khảo sát' : 'Đã tạo khảo sát mới');
          this.modalRef.close(true);
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Có lỗi xảy ra trong quá trình xử lý');
      }
    });
  }

  cancel(): void {
    this.modalRef.close();
  }
}
