import { Component, Input, OnChanges, SimpleChanges, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { ChungCuService } from '@features/resident/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-tao-the-form',
  templateUrl: './tao-the-form.component.html'
})
export class TaoTheFormComponent implements OnInit {
  @Input() phuongTienId!: number;

  form: FormGroup;
  saving = false;

  constructor(
    fb: FormBuilder,
    private svc: ChungCuService,
    private modalRef: NzModalRef,
    private notification: NzNotificationService
  ) {
    this.form = fb.group({
      maThe: ['', Validators.required]
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.phuongTienId && this.phuongTienId) {
      this.suggestMaThe();
    }
  }

  ngOnInit(): void {
    if (this.phuongTienId) this.suggestMaThe();
  }

  private suggestMaThe(): void {
    if (!this.phuongTienId) return;
    this.svc.goiYMaThe(this.phuongTienId).subscribe({
      next: res => {
        if (res && res.isOk && res.result) {
          const suggested = typeof res.result === 'string' ? res.result : (res.result.maThe || res.result);
          if (suggested) this.form.patchValue({ maThe: suggested });
        }
      },
      error: () => {}
    });
  }

  submit(): void {
    if (this.form.invalid || !this.phuongTienId) return;
    this.saving = true;
    const payload = {
      phuongTienId: this.phuongTienId,
      maThe: this.form.value.maThe
    };
    this.svc.taoThePhuongTien(payload).subscribe({
      next: res => {
        this.saving = false;
        if (res && res.isOk) {
          this.notification.success('Thành công', 'Gán thẻ phương tiện thành công');
          this.modalRef.close(true);
        } else {
          this.notification.error('Lỗi', 'Gán thẻ phương tiện thất bại');
        }
      },
      error: () => {
        this.saving = false;
        this.notification.error('Lỗi', 'Gán thẻ phương tiện thất bại');
      }
    });
  }

  cancel(): void {
    this.modalRef.close(false);
  }
}
