import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, Subject, Observable } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { SignalrService } from '@core/signalr';
import { ThongBaoService } from '@core/notification';
import {
  ChungCuService,
  YeuCauThiCongService,
  YeuCauThiCongListItem,
  YeuCauThiCongDetail,
  NhanSuThiCong,
  ApiResponse,
} from '@features/resident/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { environment } from '@env/environment';

@Component({
  selector: 'app-yeu-cau-thi-cong-list',
  templateUrl: './yeu-cau-thi-cong-list.component.html',
  styleUrls: ['./yeu-cau-thi-cong-list.component.scss'],
})
export class YeuCauThiCongListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private _notifSub?: Subscription;

  items: YeuCauThiCongListItem[] = [];
  loading = false;
  pageSize = 10;
  pageNumber = 1;
  total = 0;

  keyword = '';
  sortCol: string | null = 'CreatedAt';
  isAsc = false;

  // Filters
  canHoId?: number;
  trangThaiId?: number;
  trangThaiThiCongId?: number;
  ngayTaoTu?: Date;
  ngayTaoDen?: Date;
  batDauTu?: Date;
  batDauDen?: Date;
  ketThucTu?: Date;
  ketThucDen?: Date;

  // Options
  trangThaiOptions: any[] = [];
  trangThaiThiCongOptions: any[] = [];
  advancedVisible = false;

  // Detail & Actions
  detailVisible = false;
  detailLoading = false;
  selectedRequest: YeuCauThiCongDetail | null = null;

  // Action Modals state
  actionLoading = false;

  reasonModalVisible = false;
  reasonType: 'Return' | 'Cancel' | 'DeletePersonnel' | 'Refund' = 'Return';
  reasonTitle = '';
  reasonLabel = '';
  reasonValue = '';
  reasonTargetId: number | null = null;
  targetPersonnelId?: number;

  // Deposit
  depositVisible = false;
  depositAmount = 0;

  // Collection
  collectVisible = false;
  collectNote = '';

  // Acceptance/Refund
  refundAmount = 0;

  // Add Personnel
  nhanSuFormVisible = false;
  nhanSuModel: NhanSuThiCong = {
    hoTen: '',
    soCCCD: '',
    soDienThoai: '',
    vaiTro: '',
    ghiChu: '',
  };

  highlightId?: number;

  constructor(
    private service: YeuCauThiCongService,
    private chungCu: ChungCuService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    private route: ActivatedRoute,
    private signalr: SignalrService,
    private thongBao: ThongBaoService,
  ) {}

  ngOnInit(): void {
    this.route.queryParams
      .pipe(takeUntil(this.destroy$))
      .subscribe((params: any) => {
        const tabRaw = params.tab;
        const tab = tabRaw != null ? Number(tabRaw) : null;
        const handleHighlight = !isNaN(tab as any) && tab === 3;

        this.highlightId =
          handleHighlight && params.id != null
            ? params.id === ''
              ? undefined
              : +params.id
            : undefined;

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
    this.chungCu
      .getTrangThaiYeuCauForSelector()
      .subscribe((res) => (this.trangThaiOptions = res.isOk ? res.result : []));

    this.chungCu.getTrangThaiThiCongForSelector().subscribe({
      next: (res: any) => {
        const raw = res?.isOk
          ? res.result
          : (res?.result ?? res?.data ?? res ?? []);
        this.trangThaiThiCongOptions = this.normalizeSelectorOptions(raw);
      },
      error: () => {
        this.trangThaiThiCongOptions = [];
      },
    });
  }

  private normalizeSelectorOptions(raw: any): any[] {
    if (!raw) return [];
    if (!Array.isArray(raw)) return [];
    return raw.map((it: any) => {
      if (it == null) return { value: it, label: String(it) };
      if (
        typeof it === 'string' ||
        typeof it === 'number' ||
        typeof it === 'boolean'
      )
        return { value: it, label: String(it) };
      const value =
        it.value ??
        it.id ??
        it.key ??
        it.code ??
        it.value ??
        Object.values(it)[0];
      const label =
        it.label ??
        it.text ??
        it.ten ??
        it.name ??
        it.title ??
        String(value ?? '');
      return { value, label };
    });
  }

  private setupSignalR(): void {
    this._notifSub = this.signalr.notifications().subscribe((n: any) => {
      if (!n) return;
      const kind =
        n.loaiThongBaoId ?? (n.metadata && n.metadata.loaiThongBaoId) ?? null;
      if (kind === 11) {
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
      trangThaiId: this.trangThaiId,
      trangThaiThiCongId: this.trangThaiThiCongId,
      ngayTaoTu: this.ngayTaoTu?.toISOString(),
      ngayTaoDen: this.ngayTaoDen?.toISOString(),
      batDauTu: this.batDauTu?.toISOString(),
      batDauDen: this.batDauDen?.toISOString(),
      ketThucTu: this.ketThucTu?.toISOString(),
      ketThucDen: this.ketThucDen?.toISOString(),
      keyword: this.keyword,
    };

    this.loading = true;
    this.service.getList(query).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.isOk && res.result) {
          this.items = res.result.items;
          this.total = res.result.pagingInfo.totalItems;
          try {
            this.applyHighlight();
          } catch (e) {}
        }
      },
      error: () => (this.loading = false),
    });
  }

  private reloadDetail(): void {
    if (!this.selectedRequest) return;
    this.service.getById(this.selectedRequest.id).subscribe({
      next: (res) => {
        if (res.isOk) this.selectedRequest = res.result;
      },
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
        next: (res) => {
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
    this.trangThaiId = undefined;
    this.trangThaiThiCongId = undefined;
    this.ngayTaoTu = undefined;
    this.ngayTaoDen = undefined;
    this.batDauTu = undefined;
    this.batDauDen = undefined;
    this.ketThucTu = undefined;
    this.ketThucDen = undefined;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  toggleAdvanced(): void {
    this.advancedVisible = !this.advancedVisible;
  }

  applyAdvanced(): void {
    this.pageNumber = 1;
    this.load();
    this.advancedVisible = false;
  }

  closeAdvanced(): void {
    this.advancedVisible = false;
  }

  openDetail(it: YeuCauThiCongListItem): void {
    this.detailVisible = true;
    this.detailLoading = true;
    this.selectedRequest = null;
    this.service.getById(it.id).subscribe({
      next: (res) => {
        this.detailLoading = false;
        if (res.isOk) this.selectedRequest = res.result;
      },
      error: () => (this.detailLoading = false),
    });
  }

  // --- ACTIONS ---

  openReturn(): void {
    this.openReasonModal('Return');
  }

  openCancel(): void {
    this.openReasonModal('Cancel');
  }

  openSetDeposit(): void {
    this.depositAmount = this.selectedRequest?.tienDatCoc || 0;
    this.depositVisible = true;
  }

  submitDeposit(): void {
    if (this.depositAmount <= 0) {
      this.notification.warning(
        'Thông báo',
        'Vui lòng nhập số tiền cọc hợp lệ',
      );
      return;
    }
    this.actionLoading = true;
    this.service
      .setTienCoc(this.selectedRequest!.id, this.depositAmount)
      .subscribe({
        next: (res) => {
          this.actionLoading = false;
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã thiết lập tiền cọc');
            this.depositVisible = false;
            this.reloadDetail();
            this.load();
          }
        },
        error: () => (this.actionLoading = false),
      });
  }

  approve(): void {
    if (!this.selectedRequest) return;
    // Validation based on prerequisites in flow doc
    if (
      !this.selectedRequest.tienDatCoc ||
      this.selectedRequest.tienDatCoc <= 0
    ) {
      this.notification.warning(
        'Thông báo',
        'Vui lòng thiết lập tiền cọc trước khi duyệt',
      );
      return;
    }
    if (
      !this.selectedRequest.nhanSuThiCongs ||
      this.selectedRequest.nhanSuThiCongs.length === 0
    ) {
      this.notification.warning(
        'Thông báo',
        'Danh sách nhân sự không được để trống',
      );
      return;
    }
    if (
      !this.selectedRequest.danhSachTep ||
      this.selectedRequest.danhSachTep.length === 0
    ) {
      this.notification.warning(
        'Thông báo',
        'Hồ sơ kỹ thuật không được để trống',
      );
      return;
    }

    this.modal.confirm({
      nzTitle: 'Phê duyệt yêu cầu',
      nzContent:
        'Bạn có chắc chắn muốn phê duyệt chính thức yêu cầu thi công này? Cư dân sẽ nhận được thông báo nộp tiền cọc.',
      nzOnOk: () => {
        this.service.approve(this.selectedRequest!.id).subscribe((res) => {
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã phê duyệt yêu cầu');
            this.reloadDetail();
            this.load();
          }
        });
      },
    });
  }

  openCollect(): void {
    this.collectNote = '';
    this.collectVisible = true;
  }

  submitCollect(): void {
    this.actionLoading = true;
    this.service.thuCoc(this.selectedRequest!.id, this.collectNote).subscribe({
      next: (res) => {
        this.actionLoading = false;
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã xác nhận thu tiền cọc');
          this.collectVisible = false;
          this.reloadDetail();
          this.load();
        }
      },
      error: () => (this.actionLoading = false),
    });
  }

  nghiemThu(): void {
    this.modal.confirm({
      nzTitle: 'Nghiệm thu hoàn tất',
      nzContent:
        'Xác nhận công tác thi công tại hiện trường đã hoàn tất và đạt yêu cầu kỹ thuật?',
      nzOnOk: () => {
        this.service.nghiemThu(this.selectedRequest!.id).subscribe((res) => {
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã nghiệm thu hoàn tất');
            this.reloadDetail();
            this.load();
          }
        });
      },
    });
  }

  openRefund(): void {
    this.refundAmount = 0;
    this.openReasonModal('Refund');
  }

  complete(): void {
    this.modal.confirm({
      nzTitle: 'Đóng yêu cầu',
      nzContent: 'Bạn có chắc chắn muốn đóng hồ sơ yêu cầu thi công này?',
      nzOnOk: () => {
        this.service.complete(this.selectedRequest!.id).subscribe((res) => {
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã đóng yêu cầu thi công');
            this.reloadDetail();
            this.load();
          }
        });
      },
    });
  }

  // --- GRANULAR MANAGEMENT ---

  beforeUpload = (file: any): boolean => {
    if (!this.selectedRequest) return false;

    this.actionLoading = true;
    this.chungCu.uploadMedia([file as File], 'yeu-cau-thi-cong').subscribe({
      next: (res) => {
        if (res.isOk && res.result && res.result.length > 0) {
          const tepIds = res.result.map((f: any) => f.fileId);
          this.service
            .addTep({ id: this.selectedRequest!.id, tepIds })
            .subscribe((res2) => {
              this.actionLoading = false;
              if (res2.isOk) {
                this.notification.success('Thành công', 'Đã bổ sung hồ sơ');
                this.reloadDetail();
                this.load();
              }
            });
        } else {
          this.actionLoading = false;
        }
      },
      error: () => (this.actionLoading = false),
    });
    return false;
  };

  deleteTep(f: any): void {
    this.modal.confirm({
      nzTitle: 'Xác nhận xóa tệp',
      nzContent: `Bạn có chắc chắn muốn xóa tệp "${f.fileName}"?`,
      nzOnOk: () => {
        this.actionLoading = true;
        this.service
          .deleteTep({ id: this.selectedRequest!.id, tepId: f.id })
          .subscribe((res) => {
            this.actionLoading = false;
            if (res.isOk) {
              this.notification.success('Thành công', 'Đã xóa tệp');
              this.reloadDetail();
              this.load();
            }
          });
      },
    });
  }

  openAddNhanSu(): void {
    this.nhanSuModel = {
      hoTen: '',
      soCCCD: '',
      soDienThoai: '',
      vaiTro: '',
      ghiChu: '',
    };
    this.nhanSuFormVisible = true;
  }

  submitNhanSu(): void {
    if (!this.nhanSuModel.hoTen || !this.nhanSuModel.soCCCD) {
      this.notification.warning(
        'Thông báo',
        'Vui lòng nhập đầy đủ thông tin nhân sự',
      );
      return;
    }
    this.actionLoading = true;
    this.service
      .addNhanSu({ id: this.selectedRequest!.id, ...this.nhanSuModel })
      .subscribe({
        next: (res) => {
          this.actionLoading = false;
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã thêm nhân sự');
            this.nhanSuFormVisible = false;
            this.reloadDetail();
            this.load();
          }
        },
        error: () => (this.actionLoading = false),
      });
  }

  deleteNhanSu(ns: NhanSuThiCong): void {
    this.reasonTargetId = this.selectedRequest!.id;
    this.targetPersonnelId = ns.id;
    this.openReasonModal('DeletePersonnel');
  }

  // File management would typically involve a separate upload component,
  // here we assume we get file IDs from a shared upload service or similar.
  // For brevity, we focus on the lifecycle actions.

  // --- REASON MODAL ---

  openReasonModal(
    type: 'Return' | 'Cancel' | 'DeletePersonnel' | 'Refund',
  ): void {
    this.reasonType = type;
    this.reasonValue = '';
    switch (type) {
      case 'Return':
        this.reasonTitle = 'Yêu cầu bổ sung';
        this.reasonLabel = 'Nội dung cần bổ sung';
        break;
      case 'Cancel':
        this.reasonTitle = 'Hủy yêu cầu';
        this.reasonLabel = 'Lý do hủy';
        break;
      case 'DeletePersonnel':
        this.reasonTitle = 'Xóa nhân sự';
        this.reasonLabel = 'Lý do xóa';
        break;
      case 'Refund':
        this.reasonTitle = 'Quyết toán cọc';
        this.reasonLabel = 'Lý do khấu trừ (nếu có)';
        break;
    }
    this.reasonModalVisible = true;
  }

  submitReason(): void {
    if (!this.reasonValue.trim() && this.reasonType !== 'Refund') {
      this.notification.warning('Thông báo', 'Vui lòng nhập nội dung');
      return;
    }

    const id = this.selectedRequest?.id;
    if (!id) return;

    this.actionLoading = true;
    let obs: Observable<ApiResponse<any>>;

    switch (this.reasonType) {
      case 'Return':
        obs = this.service.traLai(id, this.reasonValue);
        break;
      case 'Cancel':
        obs = this.service.cancel(id, this.reasonValue);
        break;
      case 'DeletePersonnel':
        obs = this.service.deleteNhanSu({
          id,
          nhanSuId: this.targetPersonnelId!,
          lyDo: this.reasonValue,
        });
        break;
      case 'Refund':
        obs = this.service.hoanCoc(id, this.refundAmount, this.reasonValue);
        break;
      default:
        return;
    }

    obs.subscribe({
      next: (res: any) => {
        this.actionLoading = false;
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã cập nhật yêu cầu');
          this.reasonModalVisible = false;
          this.reloadDetail();
          this.load();
        }
      },
      error: () => (this.actionLoading = false),
    });
  }

  // --- UTILS ---

  getStatusColor(id: number): string {
    switch (id) {
      case 1:
        return 'gold'; // Pending (Chờ duyệt)
      case 2:
        return 'blue'; // Approved (Đã duyệt)
      case 3:
        return 'blue'; // Approved (Backup if ID is 3)
      case 7:
        return 'green'; // Completed (Đã đóng)
      case 8:
        return 'red'; // Cancelled (Hủy)
      case 9:
        return 'orange'; // Returned (Trả lại/Yêu cầu bổ sung)
      default:
        return 'default';
    }
  }

  getThiCongStatusColor(id: number): string {
    switch (id) {
      case 1:
        return 'default'; // ChuaThiCong
      case 2:
        return 'warning'; // ChoThuCoc
      case 3:
        return 'processing'; // DaCapPhep
      case 4:
        return 'processing'; // DangThiCong
      case 5:
        return 'success'; // DaHoanTat
      default:
        return 'default';
    }
  }

  canShowAction(action: string): boolean {
    if (!this.selectedRequest) return false;
    const s = this.selectedRequest.trangThaiYeuCauId;
    const tc = this.selectedRequest.trangThaiThiCongId;

    switch (action) {
      case 'Return':
        return s === 1; // Pending
      case 'SetDeposit':
        return s === 1; // Pending
      case 'Approve':
        return s === 1 || s === 9; // Pending or Returned
      case 'Collect':
        return (s === 2 || s === 3) && tc === 2; // Approved & Waiting for deposit
      case 'Acceptance':
        return (s === 2 || s === 3) && (tc === 3 || tc === 4); // Approved & Licensed/In Progress
      case 'Refund':
        return (
          (s === 2 || s === 3) && tc === 5 && !this.selectedRequest.isDaHoanCoc
        ); // Approved & Finished & Not refunded
      case 'Complete':
        return (
          (s === 2 || s === 3) && tc === 5 && this.selectedRequest.isDaHoanCoc
        ); // Approved & Finished & Refunded
      case 'Cancel':
        if (s === 7 || s === 8) return false;
        if (
          this.selectedRequest.isDaThuCoc &&
          !this.selectedRequest.isDaHoanCoc
        )
          return false;
        return true;
    }
    return false;
  }

  formatterVND = (value: number): string =>
    value != null ? `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '';
  parserVND = (value: string): string => value.replace(/\$\s?|(,*)/g, '');

  getFileUrl(url: string): string {
    if (!url) return '#';
    if (url.startsWith('http')) return url;
    const base = environment.apiBaseUrl
      ? environment.apiBaseUrl.replace(/\/$/, '')
      : '';
    return `${base}${url.startsWith('/') ? '' : '/'}${url}`;
  }
}
