import { Component, OnInit, OnChanges, SimpleChanges, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DichVuService } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-bang-gia-form',
  templateUrl: './bang-gia-form.component.html',
  styleUrls: ['./bang-gia-form.component.scss'],
})
export class BangGiaFormComponent implements OnInit, OnChanges {
  model: any = {
    dichVuId: null,
    tenBangGia: '',
    ngayApDung: null,
    ngayKetThuc: null,
    loaiDinhGiaId: null,
    loaiDinhGiaCode: null,
    donGia: null,
    donGiaCoDinh: null,
    isDinhKy: false,
    giaLuyTiens: [] as any[],
    giaKhungGios: [] as any[],
    giaLoaiCanHos: [] as any[],
  };

  loading = false;
  id: number | null = null;
  step = 1;

  loaiDinhGiaOptions: any[] = [];
  khungGioOptions: any[] = [];
  loaiCanHoOptions: any[] = [];

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

    // load pricing types for selector
    try {
      this.dichVuService.getLoaiDinhGiaForSelector({}).subscribe({
        next: (res: any) => {
          const items = res?.result ?? res?.data ?? res;
          this.loaiDinhGiaOptions = Array.isArray(items)
            ? items
            : (items?.items ?? []);
        },
        error: () => {
          this.loaiDinhGiaOptions = [];
        },
      });
    } catch (e) {
      this.loaiDinhGiaOptions = [];
    }

    // load apartment types for selector (used in TheoDienTich)
    try {
      this.dichVuService.getLoaiCanHoForSelector({}).subscribe({
        next: (res: any) => {
          const items = res?.result ?? res?.data ?? res;
          this.loaiCanHoOptions = Array.isArray(items)
            ? items
            : (items?.items ?? []);
        },
        error: () => {
          this.loaiCanHoOptions = [];
        },
      });
    } catch (e) {
      this.loaiCanHoOptions = [];
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.inModal && changes.itemId) {
      const val = changes.itemId.currentValue;
      this.id = val != null && val !== '' ? Number(val) : null;
      if (this.id) this.load();
      else this.resetForCreate();
    }

    if (changes.dichVuId) {
      const val = changes.dichVuId.currentValue;
      const nextId = val != null && val !== '' ? Number(val) : null;
      this.model.dichVuId = nextId;
      this.loadKhungGioOptions();
    }
  }

  resetForCreate(): void {
    const contextDichVuId =
      this.dichVuId != null ? Number(this.dichVuId) : null;
    this.model = {
      dichVuId: contextDichVuId,
      tenBangGia: '',
      ngayApDung: null,
      ngayKetThuc: null,
      loaiDinhGiaId: null,
      loaiDinhGiaCode: null,
      donGia: null,
      donGiaCoDinh: null,
      isDinhKy: false,
      giaLuyTiens: [],
      giaKhungGios: [],
      giaLoaiCanHos: [],
    };
    this.step = 1;
    this.id = null;
    this.loadKhungGioOptions();
  }

  prefillForRenew(item: any): void {
    if (!item) return;
    // copy relevant fields and suggest new start date
    const nextStart = item.ngayKetThuc
      ? new Date(item.ngayKetThuc)
      : new Date();
    if (nextStart) nextStart.setDate(nextStart.getDate() + 1);
    this.model = {
      dichVuId: item.dichVuId ?? item.dichVu?.id ?? null,
      tenBangGia: item.tenBangGia ? item.tenBangGia + ' - Gia hạn' : 'Gia hạn',
      ngayApDung: nextStart ? nextStart.toISOString() : null,
      ngayKetThuc: null,
      loaiDinhGiaId: item.loaiDinhGiaId ?? null,
      loaiDinhGiaCode: item.loaiDinhGiaCode ?? item.loaiDinhGia?.code ?? null,
      donGia: item.donGia ?? item.donGiaCoDinh ?? null,
      donGiaCoDinh: item.donGiaCoDinh ?? item.donGia ?? null,
      isDinhKy: item.isDinhKy ?? false,
      giaLuyTiens: Array.isArray(item.giaLuyTiens)
        ? JSON.parse(JSON.stringify(item.giaLuyTiens))
        : [],
      giaKhungGios: Array.isArray(item.giaKhungGios)
        ? JSON.parse(JSON.stringify(item.giaKhungGios))
        : [],
      giaLoaiCanHos: Array.isArray(item.giaLoaiCanHos)
        ? JSON.parse(JSON.stringify(item.giaLoaiCanHos))
        : [],
    };
    this.id = null;
    this.step = 1;
    this.loadKhungGioOptions();
  }

  load(): void {
    if (!this.id) return;
    this.loading = true;
    this.dichVuService.getBangGiaById(Number(this.id)).subscribe({
      next: (res: any) => {
        this.model = res?.result ?? res?.data ?? res;
        this.loading = false;
        this.loadKhungGioOptions();
      },
      error: () => (this.loading = false),
    });
  }

  private loadKhungGioOptions(): void {
    const dichVuId = this.model?.dichVuId ?? this.dichVuId;
    if (!dichVuId) {
      this.khungGioOptions = [];
      return;
    }

    try {
      this.dichVuService
        .getKhungGioList({
          dichVuId: Number(dichVuId),
          pageNumber: 1,
          pageSize: 1000,
          isActive: true,
        })
        .subscribe({
          next: (res: any) => {
            const raw = res?.result ?? res?.data ?? res;
            this.khungGioOptions = Array.isArray(raw)
              ? raw
              : (raw?.items ?? []);
          },
          error: () => {
            this.khungGioOptions = [];
          },
        });
    } catch (e) {
      this.khungGioOptions = [];
    }
  }

  save(): void {
    if (this.readOnly) return;
    if (
      !this.model ||
      !this.model.dichVuId ||
      !this.model.tenBangGia ||
      !this.model.ngayApDung ||
      !this.model.loaiDinhGiaId
    ) {
      this.notification.warning(
        'Thiếu thông tin',
        'Vui lòng điền đầy đủ các trường bắt buộc.',
      );
      return;
    }

    this.loading = true;
    const finalizeSave = () => {
      let obs: any;
      const payload: any = JSON.parse(JSON.stringify(this.model));
      if (
        (payload.donGiaCoDinh == null || payload.donGiaCoDinh === '') &&
        payload.donGia != null
      ) {
        payload.donGiaCoDinh = payload.donGia;
      }
      payload.isDinhKy = !!payload.isDinhKy;

      if (this.id) {
        payload.id = Number(this.id);
        obs = this.dichVuService.updateBangGia(payload);
      } else {
        obs = this.dichVuService.createBangGia(payload);
      }

      obs.subscribe({
        next: (res: any) => {
          this.loading = false;
          if (this.inModal) this.saved.emit(res?.result ?? res);
          else this.router.navigate(['../']);
        },
        error: () => (this.loading = false),
      });
    };

    finalizeSave();
  }

  cancel(): void {
    if (this.inModal) this.cancelled.emit();
    else this.router.navigate(['../']);
  }

  // wizard navigation
  nextStep(): void {
    if (this.step < 3) this.step++;
  }
  prevStep(): void {
    if (this.step > 1) this.step--;
  }

  // helpers for dynamic lists
  addTier(): void {
    this.model.giaLuyTiens = this.model.giaLuyTiens || [];
    this.model.giaLuyTiens.push({ tuMuc: null, denMuc: null, donGia: null });
  }
  removeTier(i: number): void {
    if (this.model.giaLuyTiens) this.model.giaLuyTiens.splice(i, 1);
  }

  addKhungGio(): void {
    this.model.giaKhungGios = this.model.giaKhungGios || [];
    this.model.giaKhungGios.push({ khungGioId: null, donGia: null });
  }
  removeKhungGio(i: number): void {
    if (this.model.giaKhungGios) this.model.giaKhungGios.splice(i, 1);
  }

  addLoaiCanHo(): void {
    this.model.giaLoaiCanHos = this.model.giaLoaiCanHos || [];
    this.model.giaLoaiCanHos.push({ loaiCanHoId: null, donGia: null });
  }
  removeLoaiCanHo(i: number): void {
    if (this.model.giaLoaiCanHos) this.model.giaLoaiCanHos.splice(i, 1);
  }

  onLoaiDinhGiaChanged(value: any): void {
    const sel = (this.loaiDinhGiaOptions || []).find(
      (x: any) =>
        x.id == value || x.value == value || x.code == value || x.key == value,
    );
    this.model.loaiDinhGiaCode = sel?.code ?? sel?.value ?? sel?.key ?? null;

    // Ensure selector data is ready for step 3 when needed
    if (this.model.loaiDinhGiaCode === 'TheoKhungGio') {
      this.loadKhungGioOptions();
      if (!this.model.giaKhungGios || this.model.giaKhungGios.length === 0)
        this.addKhungGio();
    }
    if (this.model.loaiDinhGiaCode === 'TheoDienTich') {
      if (!this.model.giaLoaiCanHos || this.model.giaLoaiCanHos.length === 0)
        this.addLoaiCanHo();
    }
  }

  onSelectLoaiDinhGia(value: any): void {
    if (this.readOnly) return;
    this.model.loaiDinhGiaId = value;
    this.onLoaiDinhGiaChanged(value);
  }

  getStrategyIcon(code: string): string {
    switch (code) {
      case 'CoDinh':
        return 'dollar';
      case 'LuyTien':
        return 'line-chart';
      case 'TheoKhungGio':
        return 'clock-circle';
      case 'TheoDienTich':
        return 'home';
      default:
        return 'appstore';
    }
  }

  getStrategyDesc(code: string): string {
    switch (code) {
      case 'CoDinh':
        return 'Một mức giá duy nhất cho mọi đơn vị sử dụng.';
      case 'LuyTien':
        return 'Giá thay đổi theo các bậc thang tiêu thụ.';
      case 'TheoKhungGio':
        return 'Định giá khác nhau theo thời điểm trong ngày.';
      case 'TheoDienTich':
        return 'Áp dụng đơn giá riêng cho từng loại căn hộ.';
      default:
        return 'Mô hình tính toán mặc định.';
    }
  }

  getKhungGioLabel(id: any): string {
    if (id == null) return '-';
    const kg = (this.khungGioOptions || []).find(
      (x: any) => x.id == id || x.value == id,
    );
    if (!kg) return '-';
    return `${kg.tenKhungGio || kg.ten || kg.name || '-'} (${kg.gioBatDau || '-'} - ${kg.gioKetThuc || '-'})`;
  }

  getLoaiCanHoLabel(id: any): string {
    if (id == null) return '-';
    const lc = (this.loaiCanHoOptions || []).find(
      (x: any) => x.id == id || x.value == id,
    );
    if (!lc) return '-';
    return lc.tenLoaiCanHo || lc.ten || lc.name || lc.label || '-';
  }
}
