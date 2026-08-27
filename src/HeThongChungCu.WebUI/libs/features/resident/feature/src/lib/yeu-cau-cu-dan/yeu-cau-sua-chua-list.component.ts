import { Component, OnInit, OnDestroy, TemplateRef, ViewChild } from '@angular/core';
import { Subscription, Subject, Observable } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { SignalrService } from '@core/signalr';
import { ThongBaoService } from '@core/notification';
import { ChungCuService, YeuCauSuaChuaService, YeuCauSuaChuaListItem, YeuCauSuaChuaDetail, NhanSuSuaChua, ApiResponse } from '@features/resident/data-access';
import { DoiTacApiService } from '@features/doi-tac/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { DichVuService } from '@features/dich-vu/data-access';

@Component({
  selector: 'app-yeu-cau-sua-chua-list',
  templateUrl: './yeu-cau-sua-chua-list.component.html',
  styleUrls: ['./yeu-cau-sua-chua-list.component.scss']
})
export class YeuCauSuaChuaListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private _notifSub?: Subscription;

  items: YeuCauSuaChuaListItem[] = [];
  loading = false;
  pageSize = 10;
  pageNumber = 1;
  total = 0;

  keyword = '';
  sortCol: string | null = 'CreatedAt';
  isAsc = false;

  // Filters
  toaNhaId?: number;
  tangId?: number;
  canHoId?: number;
  trangThaiYeuCauId?: number;
  trangThaiSuaChuaId?: number;
  loaiSuCoId?: number;
  ngayTaoTu?: Date;
  ngayTaoDen?: Date;

  // Options
  trangThaiOptions: any[] = [];
  trangThaiSuaChuaOptions: any[] = [];
  loaiSuCoOptions: any[] = [];
  advancedVisible = false;

  // Detail & Actions
  detailVisible = false;
  detailLoading = false;
  selectedRequest: YeuCauSuaChuaDetail | null = null;

  // Selection


  // Action Modals/Drawers state
  actionLoading = false;

  reasonModalVisible = false;
  reasonType: 'Reject' | 'Return' | 'Cancel' | 'DeletePersonnel' = 'Reject';
  reasonTitle = '';
  reasonLabel = '';
  reasonValue = '';
  reasonTargetId: number | null = null;
  targetPersonnelId?: number;

  // Dispatching
  dispatchVisible = false;
  dispatchType: 'Internal' | 'Partner' = 'Internal';
  nhanVienOptions: any[] = [];
  partnerOptions: any[] = [];
  contractOptions: any[] = [];

  selectedPartnerId?: number;
  selectedContractId?: number;
  dispatchPersonnel: NhanSuSuaChua[] = [];

  // Quotation
  quoteVisible = false;
  quoteForm = {
    chiPhiDuKien: 0,
    isMienPhi: false,
    ghiChuBaoGia: ''
  };

  // Schedule
  scheduleVisible = false;
  scheduleRange: Date[] = [];
  khungGioOptions: any[] = [];
  selectedKhungGioId: number | null = null;

  // Completion
  completeVisible = false;
  completeForm = {
    ketQuaXuLy: '',
    chiPhiThucTe: 0
  };

  highlightId?: number;

  constructor(
    private service: YeuCauSuaChuaService,
    private chungCu: ChungCuService,
    private doiTac: DoiTacApiService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    private route: ActivatedRoute,
    private signalr: SignalrService,
    private thongBao: ThongBaoService,
    private dichVu: DichVuService,
  ) {}

  ngOnInit(): void {
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe((params: any) => {
      const tabRaw = params.tab;
      const tab = tabRaw != null ? Number(tabRaw) : null;
      const handleHighlight = !isNaN(tab as any) && tab === 2;

      this.highlightId = handleHighlight && params.id != null
        ? (params.id === '' ? undefined : +params.id)
        : undefined;

      this.toaNhaId = params.toaNhaId ? +params.toaNhaId : undefined;
      this.tangId = params.tangId ? +params.tangId : undefined;
      this.canHoId = params.canHoId ? +params.canHoId : undefined;

      this.load();
    });

    this.initSelectors();
    this.setupSignalR();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this._notifSub?.unsubscribe();
  }

  private initSelectors(): void {
    this.chungCu.getTrangThaiYeuCauForSelector().subscribe(res => this.trangThaiOptions = res.isOk ? res.result : []);
    this.service.getTrangThaiSuaChuaForSelector().subscribe(res => this.trangThaiSuaChuaOptions = res.isOk ? res.result : []);
    this.service.getLoaiSuCoForSelector().subscribe(res => this.loaiSuCoOptions = res.isOk ? res.result : []);

    // For dispatching
    this.chungCu.getNhanVienList({ pageSize: 100, pageNumber: 1 }).subscribe(res => this.nhanVienOptions = res.isOk ? res.result.items : []);
    this.doiTac.getList({ pageSize: 100, pageNumber: 1 }).subscribe(res => this.partnerOptions = res.isOk ? res.result.items : []);

    // Load time slots for scheduling
    this.dichVu.getKhungGioList({ pageSize: 1000, isActive: true }).subscribe((res: any) => {
      const raw = res?.result ?? res?.data ?? res;
      this.khungGioOptions = Array.isArray(raw) ? raw : (raw?.items ?? []);
    });
  }

  private setupSignalR(): void {
    this._notifSub = this.signalr.notifications().subscribe((n: any) => {
      if (!n) return;
      const kind = n.loaiThongBaoId ?? (n.metadata && n.metadata.loaiThongBaoId) ?? null;
      if (kind === 10) {
        this.load();
      }
    });
  }

  load(): void {
    const query = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortCol: this.sortCol,
      isAsc: this.isAsc,
      canHoId: this.canHoId,
      trangThaiYeuCauId: this.trangThaiYeuCauId,
      trangThaiSuaChuaId: this.trangThaiSuaChuaId,
      loaiSuCoId: this.loaiSuCoId,
      ngayTaoTu: this.ngayTaoTu?.toISOString(),
      ngayTaoDen: this.ngayTaoDen?.toISOString(),
      keyword: this.keyword
    };

    this.loading = true;
    this.service.getList(query).subscribe({
      next: res => {
        this.loading = false;
        if (res.isOk && res.result) {
          this.items = res.result.items;
          this.total = res.result.pagingInfo.totalItems;
          try { this.applyHighlight(); } catch (e) {}
        }
      },
      error: () => this.loading = false
    });
  }

  private applyHighlight(): void {
    const id = this.highlightId;
    if (!id) return;
    const idx = this.items.findIndex(x => x.id === id);
    if (idx !== -1) {
      const item = this.items[idx];
      this.items = [item, ...this.items.filter((_, i) => i !== idx)];
      try { (this.items[0] as any)._highlight = true; } catch (e) {}
      setTimeout(() => { try { delete (this.items[0] as any)._highlight; this.items = [...this.items]; } catch (e) {} }, 6000);
      return;
    }

    try {
      this.service.getById(id).subscribe({
        next: res => {
          if (res.isOk && res.result) {
            const item = res.result;
            (item as any)._highlight = true;
            this.items = [item as any, ...this.items];
            this.total = (this.total || 0) + 1;
            setTimeout(() => { try { delete (this.items[0] as any)._highlight; this.items = [...this.items]; } catch (e) {} }, 6000);
          }
        },
        error: () => {}
      });
    } catch (e) {}
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.load();
  }

  onRefresh(): void {
    this.keyword = '';
    this.trangThaiYeuCauId = undefined;
    this.trangThaiSuaChuaId = undefined;
    this.loaiSuCoId = undefined;
    this.ngayTaoTu = undefined;
    this.ngayTaoDen = undefined;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  openDetail(it: YeuCauSuaChuaListItem): void {
    this.detailVisible = true;
    this.detailLoading = true;
    this.selectedRequest = null;
    this.service.getById(it.id).subscribe({
      next: res => {
        this.detailLoading = false;
        if (res.isOk) this.selectedRequest = res.result;
      },
      error: () => this.detailLoading = false
    });
  }

  approveItem(it: YeuCauSuaChuaListItem): void {
    this.modal.confirm({
      nzTitle: 'Phê duyệt yêu cầu',
      nzContent: `Bạn có chắc chắn muốn phê duyệt yêu cầu sửa chữa tại căn hộ ${it.tenCanHo}?`,
      nzOnOk: () => {
        this.service.pheDuyet(it.id).subscribe(res => {
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã phê duyệt yêu cầu');
            this.load();
          }
        });
      }
    });
  }

  rejectItem(it: YeuCauSuaChuaListItem): void {
    this.reasonTargetId = it.id;
    this.reasonType = 'Reject';
    this.reasonTitle = 'Từ chối yêu cầu';
    this.reasonLabel = 'Lý do từ chối';
    this.reasonValue = '';
    this.reasonModalVisible = true;
  }

  // --- ACTIONS ---

  approve(): void {
    if (!this.selectedRequest) return;
    this.modal.confirm({
      nzTitle: 'Phê duyệt yêu cầu',
      nzContent: 'Bạn có chắc chắn muốn phê duyệt yêu cầu sửa chữa này?',
      nzOnOk: () => {
        this.service.pheDuyet(this.selectedRequest!.id).subscribe(res => {
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã phê duyệt yêu cầu');
            this.selectedRequest = {
              ...this.selectedRequest,
              ...res.result,
            };
            this.load();
          }
        });
      }
    });
  }

  openReasonModal(type: 'Reject' | 'Return' | 'Cancel' | 'DeletePersonnel', personnelId?: number): void {
    this.reasonType = type;
    this.reasonValue = '';
    this.targetPersonnelId = personnelId;
    switch (type) {
      case 'Reject': this.reasonTitle = 'Từ chối yêu cầu'; this.reasonLabel = 'Lý do từ chối'; break;
      case 'Return': this.reasonTitle = 'Yêu cầu bổ sung'; this.reasonLabel = 'Nội dung cần bổ sung'; break;
      case 'Cancel': this.reasonTitle = 'Hủy yêu cầu'; this.reasonLabel = 'Lý do hủy'; break;
      case 'DeletePersonnel': this.reasonTitle = 'Xóa nhân sự'; this.reasonLabel = 'Lý do xóa'; break;
    }
    this.reasonModalVisible = true;
  }

  submitReason(): void {
    if (!this.reasonValue.trim()) {
      this.notification.warning('Thông báo', 'Vui lòng nhập nội dung');
      return;
    }

    const id = this.reasonTargetId || this.selectedRequest?.id;
    if (!id) return;

    this.actionLoading = true;
    let obs: Observable<ApiResponse<any>>;

    switch (this.reasonType) {
      case 'Reject':
        obs = this.service.tuChoi(id, this.reasonValue);
        break;
      case 'Return':
        obs = this.service.traLai(id, this.reasonValue);
        break;
      case 'Cancel':
        obs = this.service.huy(id, this.reasonValue);
        break;
      case 'DeletePersonnel':
        obs = this.service.xoaNhanSu({ id, nhanSuId: this.targetPersonnelId!, lyDo: this.reasonValue });
        break;
      default: return;
    }

    obs.subscribe({
      next: (res: ApiResponse<any>) => {
        this.actionLoading = false;
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã cập nhật trạng thái');
          this.reasonModalVisible = false;
          this.reasonTargetId = null;
          if (this.selectedRequest && this.selectedRequest.id === id) {
            this.selectedRequest = {
              ...this.selectedRequest,
              ...res.result,
            };
          }
          this.load();
        }
      },
      error: () => this.actionLoading = false
    });
  }

  // --- DISPATCHING ---
  openDispatch(type: 'Internal' | 'Partner'): void {
    this.dispatchType = type;
    this.dispatchPersonnel = [{ vaiTro: '', ghiChu: '' }];
    this.selectedPartnerId = undefined;
    this.selectedContractId = undefined;
    this.dispatchVisible = true;
  }

  onPartnerChange(partnerId: number): void {
    const partner = this.partnerOptions.find(p => p.id === partnerId);
    this.contractOptions = partner?.hopDongs || [];
    this.selectedContractId = undefined;
  }

  addPersonnel(): void {
    this.dispatchPersonnel.push({ vaiTro: '', ghiChu: '' });
  }

  removePersonnel(index: number): void {
    this.dispatchPersonnel.splice(index, 1);
  }

  submitDispatch(): void {
    // Basic validation
    if (this.dispatchType === 'Partner' && !this.selectedContractId) {
      this.notification.warning('Thông báo', 'Vui lòng chọn hợp đồng đối tác');
      return;
    }

    const payload = {
      id: this.selectedRequest!.id,
      hopDongDoiTacId: this.dispatchType === 'Partner' ? this.selectedContractId : null,
      nhanSu: this.dispatchPersonnel.map(p => ({
        ...p,
        nhanVienId: this.dispatchType === 'Internal' ? (p.nhanVienId || undefined) : undefined,
        hoTen: this.dispatchType === 'Partner' ? (p.hoTen || undefined) : undefined,
        soCCCD: this.dispatchType === 'Partner' ? (p.soCCCD || undefined) : undefined,
        soDienThoai: this.dispatchType === 'Partner' ? (p.soDienThoai || undefined) : undefined,
      })) as NhanSuSuaChua[]
    };

    this.service.dieuPhoiNhanSu(payload).subscribe(res => {
      if (res.isOk) {
        this.notification.success('Thành công', 'Đã điều phối nhân sự');
        this.dispatchVisible = false;
        this.selectedRequest = {
          ...this.selectedRequest,
          ...res.result,
        };
        this.load();
      }
    });
  }

  submitSupplement(): void {
     const payload = {
      id: this.selectedRequest!.id,
      nhanSu: this.dispatchPersonnel.map(p => ({
        ...p,
        nhanVienId: this.dispatchType === 'Internal' ? (p.nhanVienId || undefined) : undefined,
        hoTen: this.dispatchType === 'Partner' ? (p.hoTen || undefined) : undefined,
        soCCCD: this.dispatchType === 'Partner' ? (p.soCCCD || undefined) : undefined,
        soDienThoai: this.dispatchType === 'Partner' ? (p.soDienThoai || undefined) : undefined,
      })) as NhanSuSuaChua[]
    };
    this.service.boSungNhanSu(payload).subscribe(res => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã bổ sung nhân sự');
          this.dispatchVisible = false;
        this.selectedRequest = {
          ...this.selectedRequest!,
            ...res.result,
          };
          this.load();
        }
      });
  }

  // --- QUOTATION ---
  openQuote(): void {
    this.quoteForm = {
      chiPhiDuKien: this.selectedRequest?.chiPhiDuKien || 0,
      isMienPhi: this.selectedRequest?.isMienPhi || false,
      ghiChuBaoGia: this.selectedRequest?.ghiChuBaoGia || ''
    };
    this.quoteVisible = true;
  }

  submitQuote(): void {
    this.service.nhapBaoGia({ id: this.selectedRequest!.id, ...this.quoteForm }).subscribe(res => {
      if (res.isOk) {
        this.notification.success('Thành công', 'Đã cập nhật báo giá');
        this.quoteVisible = false;
        this.selectedRequest = {
          ...this.selectedRequest,
          ...res.result,
        };
        this.load();
      }
    });
  }

  // --- SCHEDULING ---
  openSchedule(): void {
    this.scheduleRange = this.selectedRequest?.henTu ? [new Date(this.selectedRequest.henTu), new Date(this.selectedRequest.henDen!)] : [];
    this.selectedKhungGioId = null;
    this.scheduleVisible = true;
  }

  onKhungGioChange(kgId: number): void {
    const kg = this.khungGioOptions.find(o => o.id === kgId);
    if (!kg) return;

    const baseDate = this.scheduleRange[0] || new Date();

    // Parse HH:mm
    const parseTime = (timeStr: string) => {
      const [h, m] = (timeStr || "00:00").split(':').map(Number);
      const d = new Date(baseDate);
      d.setHours(h || 0, m || 0, 0, 0);
      return d;
    };

    this.scheduleRange = [parseTime(kg.gioBatDau), parseTime(kg.gioKetThuc)];
  }

  submitSchedule(): void {
    if (this.scheduleRange.length < 2) return;
    const henTu = this.scheduleRange[0].toISOString();
    const henDen = this.scheduleRange[1].toISOString();
    this.service.henLich({
      id: this.selectedRequest!.id,
      henTu,
      henDen
    }).subscribe(res => {
      if (res.isOk) {
        this.notification.success('Thành công', 'Đã hẹn lịch sửa chữa');
        this.scheduleVisible = false;
        this.selectedRequest = {
          ...this.selectedRequest!,
          henTu: res.result?.henTu || henTu,
          henDen: res.result?.henDen || henDen,
        };
        this.load();
      }
    });
  }

  // --- COMPLETION ---
  openComplete(): void {
    this.completeForm = {
      ketQuaXuLy: '',
      chiPhiThucTe: this.selectedRequest?.chiPhiDuKien || 0
    };
    this.completeVisible = true;
  }

  submitComplete(): void {
    this.service.hoanTatXuLy({ id: this.selectedRequest!.id, ...this.completeForm }).subscribe(res => {
      if (res.isOk) {
        this.notification.success('Thành công', 'Đã hoàn tất xử lý yêu cầu');
        this.completeVisible = false;
        this.selectedRequest = {
          ...this.selectedRequest,
          ...res.result,
        };
        this.load();
      }
    });
  }

  // --- UTILS ---
  getStatusColor(id: number): string {
    switch (id) {
      case 1: return 'gold'; // Pending
      case 2: return 'blue'; // Approved
      case 3: return 'red'; // Rejected
      case 7: return 'green'; // Completed
      case 8: return 'volcano'; // Cancelled
      case 9: return 'orange'; // Returned
      default: return 'default';
    }
  }

  getTrangThaiBgColor(id: number): string {
    switch (id) {
      case 1: return '#f59e0b'; // Pending
      case 2: return '#2563eb'; // Processing
      case 3: return '#ef4444'; // Rejected
      case 7: return '#16a34a'; // Completed
      case 8: return '#ef4444'; // Cancelled
      case 9: return '#f97316'; // Returned
      default: return '#6b7280';
    }
  }

  getSubStatusBgColor(id: number): string {
    switch (id) {
      case 1: return '#0891b2'; // DaDieuPhoi
      case 2: return '#4f46e5'; // DaDuyetBaoGia
      case 3: return '#9333ea'; // DaHenLich
      default: return '#6b7280';
    }
  }

  canShowAction(status: string): boolean {
     if (!this.selectedRequest) return false;
     const s = Number(this.selectedRequest.trangThaiYeuCauId);
     const sub = Number(this.selectedRequest.trangThaiSuaChuaId);
     const hasSchedule = !!this.selectedRequest.henTu;

     switch(status) {
       case 'Approve': return s === 1 || s === 9;
       case 'Dispatch': return s === 2 && !sub;
       case 'Supplement': return s === 2 && !!sub;
       case 'Quote': return s === 2 && sub === 1;
        case 'Schedule': return s === 2 && sub === 3 && !hasSchedule;
        case 'Complete': return s === 2 && !!this.selectedRequest?.henTu;
       case 'Cancel': return s === 1 || s === 2 || s === 9;
     }
     return false;
  }

  exportVoucher(): void {
    this.notification.info('Thông báo', 'Tính năng in phiếu giao việc đang được phát triển');
  }

  formatterVND = (value: number): string => (value != null ? `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '');
  parserVND = (value: string): string => value.replace(/\$\s?|(,*)/g, '');
}
