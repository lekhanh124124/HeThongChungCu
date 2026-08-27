import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DichVuService } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-khung-gio-form',
  templateUrl: './khung-gio-form.component.html',
  styleUrls: ['./khung-gio-form.component.scss'],
})
export class KhungGioFormComponent implements OnInit, OnChanges {
  model: any = {
    dichVuId: null,
    tenKhungGio: '',
    gioBatDau: '',
    gioKetThuc: '',
    ngayTrongTuan: null,
  };

  ngayTrongTuanOptions: any[] = [];

  loading = false;
  id: number | null = null;

  @Input() inModal = false;
  @Input() itemId?: number | string | null;
  @Input() dichVuId?: number | null;
  @Input() readOnly = false;
  @Output() saved = new EventEmitter<any>();
  @Output() cancelled = new EventEmitter<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dichVuService: DichVuService,
    private notification: NzNotificationService,
  ) {}

  ngOnInit(): void {
    if (!this.inModal) {
      const idStr = this.route.snapshot.paramMap.get('id');
      this.id = idStr ? Number(idStr) : null;
      if (this.id) this.load();
    }
    if (this.dichVuId != null && this.model.dichVuId == null) {
      this.model.dichVuId = Number(this.dichVuId);
    }
    this.loadNgayTrongTuan();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.dichVuId) {
      const val = changes.dichVuId.currentValue;
      this.model.dichVuId = val != null && val !== '' ? Number(val) : null;
    }
    if (this.inModal && changes.itemId) {
      const val = changes.itemId.currentValue;
      this.id = val != null && val !== '' ? Number(val) : null;
      if (this.id) this.load();
      else this.resetForCreate(this.model.dichVuId);
    }
  }

  resetForCreate(dichVuId?: number | null): void {
    this.id = null;
    this.model = {
      dichVuId: dichVuId != null ? Number(dichVuId) : null,
      tenKhungGio: '',
      gioBatDau: '',
      gioKetThuc: '',
      ngayTrongTuan: null,
    };
  }

  private loadNgayTrongTuan(): void {
    this.dichVuService.getNgayTrongTuanForSelector().subscribe({
      next: (r: any) => {
        if (!r) return;
        if (r.isOk && Array.isArray(r.result))
          this.ngayTrongTuanOptions = r.result;
        else if (Array.isArray(r.result ?? r))
          this.ngayTrongTuanOptions = r.result ?? r;
      },
      error: () => {},
    });
  }

  load(): void {
    if (!this.id) return;
    this.loading = true;
    this.dichVuService.getKhungGioById(Number(this.id)).subscribe({
      next: (res: any) => {
        this.model = res?.result ?? res?.data ?? res;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  save(): void {
    if (this.readOnly) return;
    if (
      !this.model?.dichVuId ||
      !this.model?.tenKhungGio ||
      !this.model?.gioBatDau ||
      !this.model?.gioKetThuc
    ) {
      this.notification.warning(
        'Thiếu thông tin',
        'Vui lòng điền đầy đủ các trường bắt buộc.',
      );
      return;
    }
    this.loading = true;
    const payload: any = JSON.parse(JSON.stringify(this.model || {}));

    const ensureSeconds = (t: any) => {
      if (t == null) return t;
      const s = String(t).trim();
      if (/^\d{2}:\d{2}:\d{2}$/.test(s)) return s;
      if (/^\d{2}:\d{2}$/.test(s)) return `${s}:00`;
      return s;
    };

    payload.gioBatDau = ensureSeconds(payload.gioBatDau);
    payload.gioKetThuc = ensureSeconds(payload.gioKetThuc);
    if (payload.ngayTrongTuan === '' || payload.ngayTrongTuan === undefined)
      payload.ngayTrongTuan = null;

    if (!this.id && payload.isActive == null) payload.isActive = true;

    const obs = this.id
      ? this.dichVuService.updateKhungGio({ ...payload, id: Number(this.id) })
      : this.dichVuService.createKhungGio(payload);
    obs.subscribe({
      next: (res: any) => {
        this.loading = false;
        const created = res?.result ?? res;

        if (!this.id && created && created.id && created.isActive === false) {
          this.loading = true;
          const dvId = created.dichVuId ?? payload.dichVuId;
          this.dichVuService
            .activateKhungGio({ dichVuId: Number(dvId), ids: [created.id] })
            .subscribe({
              next: () => {
                this.loading = false;
                if (this.inModal) this.saved.emit(created);
                else this.router.navigate(['../'], { relativeTo: this.route });
              },
              error: () => {
                this.loading = false;
                this.notification.error(
                  'Lỗi',
                  'Tạo khung giờ thành công nhưng kích hoạt thất bại.',
                );
                if (this.inModal) this.saved.emit(created);
                else this.router.navigate(['../'], { relativeTo: this.route });
              },
            });
          return;
        }

        if (this.inModal) this.saved.emit(created);
        else this.router.navigate(['../'], { relativeTo: this.route });
      },
      error: () => (this.loading = false),
    });
  }

  cancel(): void {
    if (this.inModal) this.cancelled.emit();
    else this.router.navigate(['../'], { relativeTo: this.route });
  }

  getNgayTrongTuanLabel(value: any): string {
    if (value == null || !this.ngayTrongTuanOptions) return '-';
    const found = (this.ngayTrongTuanOptions || []).find(
      (x: any) =>
        (x.value !== undefined && x.value !== null ? x.value : x.id) == value,
    );
    if (!found) return '-';
    return (
      found.label || found.name || found.ten || found.text || String(value)
    );
  }
}
