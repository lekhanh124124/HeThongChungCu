import { Component, OnInit, OnChanges, SimpleChanges, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DichVuService } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-dich-vu-form',
  templateUrl: './dich-vu-form.component.html',
  styleUrls: ['./dich-vu-form.component.scss']
})
export class DichVuFormComponent implements OnInit, OnChanges {
  model: any = {
    maDichVu: '',
    tenDichVu: '',
    loaiDichVuId: null,
    donViTinh: '',
    moTa: '',
    hopDongDoiTacId: null,
    trangThaiDichVuId: null
  };
  loading = false;
  id: number | null = null;

  loaiDichVuOptions: any[] = [];
  hopDongOptions: any[] = [];

  @Input() inModal = false;
  @Input() itemId?: number | string | null;
  @Output() saved = new EventEmitter<any>();
  @Output() cancelled = new EventEmitter<void>();

  constructor(
    public route: ActivatedRoute,
    public router: Router,
    public dichVuService: DichVuService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    if (!this.inModal) {
      const idStr = this.route.snapshot.paramMap.get('id');
      this.id = idStr ? Number(idStr) : null;
      if (this.id) this.load();
    }
    // load selectors
    try {
      this.dichVuService.getLoaiDichVuForSelector({}).subscribe({
        next: (res: any) => { this.loaiDichVuOptions = Array.isArray(res) ? res : (res?.result?.items ?? res?.result ?? res?.data ?? []); },
        error: () => { this.loaiDichVuOptions = []; }
      });
    } catch (e) { this.loaiDichVuOptions = []; }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.inModal && changes.itemId) {
      const val = changes.itemId.currentValue;
      this.id = val != null && val !== '' ? Number(val) : null;
      if (this.id) this.load();
      else this.model = { maDichVu: '', tenDichVu: '', loaiDichVuId: null, donViTinh: '', moTa: '', iconId: null, hopDongDoiTacId: null, isBatBuoc: false, soLuongToiDa: null };
    }
  }

  load() {
    if (!this.id) return;
    this.loading = true;
    this.dichVuService.getDichVuById(Number(this.id)).subscribe({
      next: (res: any) => {
        this.model = res?.result ?? res?.data ?? res;
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  save() {
    if (!this.model.maDichVu || !this.model.tenDichVu || !this.model.loaiDichVuId || !this.model.donViTinh) {
      this.notification.warning('Thiếu thông tin', 'Vui lòng điền đầy đủ các trường bắt buộc.');
      return;
    }

    this.loading = true;

    const finalizeSave = () => {
      let obs: any;
      if (this.id) {
        this.model.id = Number(this.id);
        obs = this.dichVuService.updateDichVu(this.model);
      } else {
        obs = this.dichVuService.createDichVu(this.model);
      }

      obs.subscribe({
        next: (res: any) => {
          this.loading = false;
          if (this.inModal) {
            this.saved.emit(res?.result ?? res);
          } else {
            this.router.navigate(['../'], { relativeTo: this.route });
          }
        },
        error: () => (this.loading = false)
      });
    };

    finalizeSave();
  }

  cancel() {
    if (this.inModal) {
      this.cancelled.emit();
    } else {
      this.router.navigate(['../'], { relativeTo: this.route });
    }
  }
}
