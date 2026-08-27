import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ChiSoTieuThuService } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-chi-so-tieu-thu-form',
  templateUrl: './chi-so-tieu-thu-form.component.html',
  styleUrls: ['./chi-so-tieu-thu-form.component.scss']
})
export class ChiSoTieuThuFormComponent implements OnInit {
  @Input() itemId?: number;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  loading = false;
  saving = false;
  item: any = {};

  constructor(
    private chiSoService: ChiSoTieuThuService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    if (this.itemId) {
      this.load();
    }
  }

  load(): void {
    this.loading = true;
    this.chiSoService.getById(this.itemId!).subscribe({
      next: (res) => {
        this.item = res.result || {};
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải chi tiết');
      }
    });
  }

  save(): void {
    this.saving = true;
    const payload = {
      id: this.item.id,
      chiSoCu: this.item.chiSoCu,
      chiSoMoi: this.item.chiSoMoi,
      thang: this.item.thang,
      nam: this.item.nam,
      ngayGhiNhan: this.item.ngayGhiNhan || new Date().toISOString(),
      anhDongHoId: this.item.anhDongHoId,
      ghiChu: this.item.ghiChu
    };

    this.chiSoService.update(payload).subscribe({
      next: (res) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã cập nhật chỉ số');
          this.saved.emit();
        }
        this.saving = false;
      },
      error: () => {
        this.saving = false;
        this.notification.error('Lỗi', 'Cập nhật thất bại');
      }
    });
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
