import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  TemplateRef,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChungCuService } from '@features/resident/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { Subject, forkJoin } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { QuanHeCuTruFormComponent } from './quan-he-cu-tru-form.component';
import { HoSoCuDanComponent } from './ho-so-cu-dan.component';

@Component({
  selector: 'app-quan-he-cu-tru',
  templateUrl: './quan-he-cu-tru.component.html',
  styleUrls: ['./quan-he-cu-tru.component.scss'],
})
export class QuanHeCuTruComponent implements OnInit, OnDestroy {
  items: any[] = [];
  loading = false;

  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;

  // sắp xếp
  sortCol: string | null = null;
  isAsc = true;

  // bộ lọc nâng cao
  maToaNha = '';
  maTang = '';
  maCanHo = '';

  advancedVisible = false;

  keyword = '';
  loaiQuanHeCuTruId: number | null = null;
  isKetThuc: boolean | null = null;

  ngayBatDauFrom = '';
  ngayBatDauTo = '';
  ngayKetThucFrom = '';
  ngayKetThucTo = '';

  loaiQuanHeOptions: any[] = [];
  trangThaiCuTruOptions: any[] = [];

  // selection / bulk actions
  setOfCheckedId = new Set<number | string>();
  checked = false;
  indeterminate = false;
  listOfCurrentPageData: readonly any[] = [];

  // add / edit resident
  // form handled in child component
  @ViewChild('formComp') formComp?: QuanHeCuTruFormComponent;
  @ViewChild('identifyTpl') identifyTpl?: TemplateRef<any>;

  selectedProfile: any | null = null;
  identifyModalRef: any = null;
  identifyEmail = '';
  identifyLoading = false;
  identifyDirectLoading = false;
  identifyUserId: number | null = null;

  private destroy$ = new Subject<void>();

  filterScope: 'root' | 'building' | 'floor' | 'apartment' = 'root';
  toaNhaId?: number;
  toaNhaName = '';
  tangId?: number;
  tangName = '';
  canHoId?: number;

  constructor(
    private route: ActivatedRoute,
    private chungCu: ChungCuService,
    private notification: NzNotificationService,
    private modal: NzModalService,
  ) {}

  ngOnInit(): void {
    this.loadLoaiQuanHeCuTruOptions();
    this.loadTrangThaiCuTruOptions();
    this.chungCu.refresh$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.loadResidents());
    this.route.queryParams.subscribe((params) => {
      const scope = params['filterScope'] as any;
      this.filterScope =
        scope === 'building' || scope === 'floor' || scope === 'apartment'
          ? scope
          : 'root';

      this.toaNhaId = params['toaNhaId']
        ? Number(params['toaNhaId'])
        : undefined;
      this.toaNhaName = params['toaNhaName'] || '';
      this.tangId = params['tangId'] ? Number(params['tangId']) : undefined;
      this.tangName = params['tangName'] || '';
      this.canHoId = params['canHoId'] ? Number(params['canHoId']) : undefined;

      this.pageNumber = 1;
      this.loadResidents();
    });
  }

  private loadTrangThaiCuTruOptions(): void {
    this.chungCu
      .getTrangThaiCuTruForSelector()
      .pipe(takeUntil(this.destroy$))
      .subscribe((r) => {
        if (r && r.isOk && Array.isArray(r.result)) {
          this.trangThaiCuTruOptions = r.result;
        }
      });
  }

  getTrangThaiLabel(val: any): string {
    if (this.trangThaiCuTruOptions && this.trangThaiCuTruOptions.length) {
      const found = this.trangThaiCuTruOptions.find((o) => {
        if (o == null) return false;
        return (
          o.id === val ||
          o.value === val ||
          o.key === val ||
          String(o.id) === String(val) ||
          o === val
        );
      });
      if (found)
        return found.name || found.label || found.ten || String(found.id);
    }
    if (val == null) return '-';
    return String(val);
  }

  getTrangThaiColor(val: any): string {
    if (this.trangThaiCuTruOptions && this.trangThaiCuTruOptions.length) {
      const found = this.trangThaiCuTruOptions.find((o) => {
        if (o == null) return false;
        return (
          o.id === val ||
          o.value === val ||
          o.key === val ||
          String(o.id) === String(val) ||
          o === val
        );
      });
      if (found) {
        if ((found as any).color) return (found as any).color;
        const name = (found.name || found.label || found.ten || '')
          .toString()
          .toLowerCase();
        if (
          name.includes('kết') ||
          name.includes('kết thúc') ||
          name.includes('đã')
        )
          return 'red';
        if (name.includes('đang') || name.includes('đang cư trú'))
          return 'green';
      }
    }

    // Fallback heuristics
    const s = val == null ? '' : String(val).toLowerCase();
    if (
      s === 'true' ||
      s === '1' ||
      s.includes('kết') ||
      s.includes('kết thúc') ||
      s.includes('đã')
    )
      return 'red';
    if (s === 'false' || s === '0' || s.includes('đang')) return 'green';

    return 'default';
  }
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  openAddForm(): void {
    try {
      this.formComp?.openAdd();
    } catch (e) {}
  }

  private loadLoaiQuanHeCuTruOptions(): void {
    this.chungCu.getLoaiQuanHeCuTruForSelector().subscribe((r) => {
      if (r && r.isOk && Array.isArray(r.result)) {
        this.loaiQuanHeOptions = r.result;
      }
    });
  }

  loadResidents(): void {
    this.loading = true;

    const query: any = {
      keyword: this.keyword || undefined,
      maToaNha: this.maToaNha || undefined,
      maTang: this.maTang || undefined,
      maCanHo: this.maCanHo || undefined,
      loaiQuanHeCuTruId: this.loaiQuanHeCuTruId || undefined,
      isKetThuc: this.isKetThuc !== null ? this.isKetThuc : undefined,
      ngayBatDauFrom: this.ngayBatDauFrom || undefined,
      ngayBatDauTo: this.ngayBatDauTo || undefined,
      ngayKetThucFrom: this.ngayKetThucFrom || undefined,
      ngayKetThucTo: this.ngayKetThucTo || undefined,
      sortCol: this.sortCol || undefined,
      isAsc: this.sortCol ? this.isAsc : undefined,
      pageNumber: this.pageNumber || 1,
      pageSize: this.pageSize || 10,
    };

    if (this.filterScope === 'building' && this.toaNhaId) {
      query.toaNhaId = this.toaNhaId;
    } else if (this.filterScope === 'floor' && this.tangId) {
      query.tangId = this.tangId;
    } else if (this.filterScope === 'apartment' && this.canHoId) {
      query.canHoId = this.canHoId;
    }

    this.chungCu.getResidentsByCanHo(query).subscribe({
      next: (res) => {
        this.loading = false;
        if (res && res.isOk) {
          const payload: any = res.result;
          if (payload) {
            if (Array.isArray(payload)) {
              this.items = payload;
              this.totalItems = payload.length;
            } else if (payload.items && Array.isArray(payload.items)) {
              this.items = payload.items;
              this.totalItems =
                payload.pagingInfo?.totalItems || payload.items.length;
            } else {
              this.items = [];
              this.totalItems = 0;
            }
          } else {
            this.items = [];
            this.totalItems = 0;
          }
        } else {
          this.items = [];
          this.totalItems = 0;
          this.notification.error('Lỗi', 'Không lấy được danh sách cư dân');
        }
      },
      error: () => {
        this.loading = false;
        this.items = [];
        this.totalItems = 0;
        this.notification.error('Lỗi', 'Không lấy được danh sách cư dân');
      },
    });
  }

  refresh(): void {
    this.keyword = '';
    this.maToaNha = '';
    this.maTang = '';
    this.maCanHo = '';
    this.loaiQuanHeCuTruId = null;
    this.isKetThuc = null;
    this.ngayBatDauFrom = '';
    this.ngayBatDauTo = '';
    this.ngayKetThucFrom = '';
    this.ngayKetThucTo = '';
    this.sortCol = null;
    this.isAsc = true;
    this.pageNumber = 1;
    this.loadResidents();
  }

  search(): void {
    this.pageNumber = 1;
    this.loadResidents();
  }

  onSortSelect(col: string | null): void {
    if (!col) {
      this.sortCol = null;
      this.isAsc = true;
      this.pageNumber = 1;
      this.loadResidents();
      return;
    }
    if (this.sortCol === col) this.isAsc = !this.isAsc;
    else {
      this.sortCol = col;
      this.isAsc = true;
    }
    this.pageNumber = 1;
    this.loadResidents();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadResidents();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNumber = 1;
    this.loadResidents();
  }

  openAdvanced(): void {
    this.advancedVisible = true;
  }

  closeAdvanced(): void {
    this.advancedVisible = false;
  }

  applyAdvanced(): void {
    this.pageNumber = 1;
    this.loadResidents();
    this.advancedVisible = false;
  }

  changeSort(col: string): void {
    if (this.sortCol === col) {
      this.isAsc = !this.isAsc;
    } else {
      this.sortCol = col;
      this.isAsc = true;
    }
    this.pageNumber = 1;
    this.loadResidents();
  }

  // selection helpers copied from can-ho-residents
  onCurrentPageDataChange(list: readonly any[]): void {
    this.listOfCurrentPageData = list;
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    if (checked) {
      this.listOfCurrentPageData.forEach((item) =>
        this.setOfCheckedId.add(item.quanHeCuTruId || item.userId),
      );
    } else {
      this.listOfCurrentPageData.forEach((item) =>
        this.setOfCheckedId.delete(item.quanHeCuTruId || item.userId),
      );
    }
    this.refreshCheckedStatus();
  }

  onItemChecked(id: number | string, checked: boolean): void {
    if (checked) this.setOfCheckedId.add(id);
    else this.setOfCheckedId.delete(id);
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    this.checked =
      this.listOfCurrentPageData.length > 0 &&
      this.listOfCurrentPageData.every((item) =>
        this.setOfCheckedId.has(item.quanHeCuTruId || item.userId),
      );
    this.indeterminate =
      this.listOfCurrentPageData.some((item) =>
        this.setOfCheckedId.has(item.quanHeCuTruId || item.userId),
      ) && !this.checked;
  }

  // onSearchUserByIdCard(): void {
  //   // moved to form component
  // }

  // cancelRegister(): void {
  //   // moved to form component
  // }

  // addDocumentCard(): void {
  //   // moved to form component
  // }

  // removeDocumentCard(index: number): void {
  //   // moved to form component
  // }

  // removeDocumentFile(cardIndex: number, fileIndex: number): void {
  //   // moved to form component
  // }

  // startAddFileToCard(index: number, inputEl: HTMLInputElement): void {
  //   // moved to form component
  // }

  // onCardFileInput(evt: Event): void {
  //   // moved to form component
  // }

  // submitRegister(): void {
  //   // moved to form component
  // }

  // // upload helpers moved to form component

  // setResidence(): void {
  //   // moved to form component
  // }

  // // định danh cư dân: gửi mã định danh qua email
  // sendMaDinhDanh(): void {
  //   // moved to form component
  // }

  // // định danh cư dân: liên kết tài khoản trực tiếp (dành cho BQL/quầy tiếp tân)
  // lienKetTaiKhoanTrucTiep(): void {
  //   // moved to form component
  // }

  // // edit / end residence
  // editResident(item: any): void {
  //   //
  // }

  // submitEdit(): void {
  //   // moved to form component
  // }

  closeEdit(): void {
    try {
      this.formComp?.closeEdit();
    } catch (e) {}
  }

  deleteOne(id?: number | string): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Kết thúc cư trú',
      nzContent: 'Bạn có chắc chắn muốn kết thúc cư trú của cư dân này?',
      nzOkText: 'Đồng ý',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () =>
        this.chungCu
          .ketThucQuanHeCuTru(id)
          .pipe(takeUntil(this.destroy$))
          .subscribe(() => this.loadResidents()),
    });
  }

  deleteSelectedMultiple(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) {
      this.notification.warning('Thông báo', 'Chưa chọn cư dân nào');
      return;
    }
    this.modal.confirm({
      nzTitle: 'Kết thúc cư trú các cư dân đã chọn',
      nzContent: `Bạn có chắc chắn muốn kết thúc cư trú của ${ids.length} cư dân đã chọn?`,
      nzOkText: 'Đồng ý',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () =>
        forkJoin(ids.map((id) => this.chungCu.ketThucQuanHeCuTru(id)))
          .pipe(takeUntil(this.destroy$))
          .subscribe(() => this.loadResidents()),
    });
  }

  viewHoSo(item: any): void {
    const id = item?.quanHeCuTruId ?? item?.userId;
    if (!id) {
      this.notification.warning('Thông báo', 'Không có id quan hệ cư trú');
      return;
    }
    this.chungCu
      .getCuDanThongTin(Number(id))
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          if (res && res.isOk && res.result) {
            this.selectedProfile = res.result;
            try {
              this.modal.create({
                nzTitle: 'Hồ sơ cư dân',
                nzContent: HoSoCuDanComponent,
                nzComponentParams: { profile: this.selectedProfile },
                nzFooter: null,
                nzWidth: 740,
                nzBodyStyle: { 'max-height': '80vh', overflow: 'auto' },
                nzWrapClassName: 'ho-so-modal',
              });
            } catch (e) {
              this.notification.info(
                'Hồ sơ cư dân',
                JSON.stringify(res.result),
              );
            }
          } else {
            this.notification.error('Lỗi', 'Không lấy được hồ sơ cư dân');
          }
        },
        error: () =>
          this.notification.error('Lỗi', 'Không lấy được hồ sơ cư dân'),
      });
  }

  openIdentify(item: any): void {
    const id = item?.quanHeCuTruId ?? item?.userId;
    if (!id) {
      this.notification.warning('Thông báo', 'Không có id quan hệ cư trú');
      return;
    }
    this.identifyUserId = Number(id);
    this.identifyEmail = '';
    try {
      this.identifyModalRef = this.modal.create({
        nzTitle: 'Định danh cư dân',
        nzContent: this.identifyTpl,
        nzFooter: null,
      });
    } catch (e) {
      this.notification.info('Định danh', 'Không thể mở modal');
    }
  }

  sendMaDinhDanhForList(): void {
    if (!this.identifyUserId) {
      this.notification.warning('Thông báo', 'Không có id cư dân');
      return;
    }
    const email = (this.identifyEmail || '').trim();
    if (!email) {
      this.notification.warning('Thông báo', 'Vui lòng nhập email');
      return;
    }
    this.identifyLoading = true;
    this.chungCu
      .taoMaDinhDanh({ quanHeCuTruId: this.identifyUserId as number, email })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.identifyLoading = false;
          if (res && res.isOk) {
            this.notification.success(
              'Thành công',
              'Đã gửi mã định danh qua email',
            );
            try {
              this.identifyModalRef?.destroy();
            } catch (e) {}
          } else {
            this.notification.error('Lỗi', 'Gửi mã định danh thất bại');
          }
        },
        error: () => {
          this.identifyLoading = false;
          this.notification.error('Lỗi', 'Gửi mã định danh thất bại');
        },
      });
  }

  lienKetTaiKhoanTrucTiepForList(): void {
    if (!this.identifyUserId) {
      this.notification.warning('Thông báo', 'Không có id cư dân');
      return;
    }
    const email = (this.identifyEmail || '').trim();
    if (!email) {
      this.notification.warning('Thông báo', 'Vui lòng nhập email');
      return;
    }
    this.identifyDirectLoading = true;
    this.chungCu
      .lienKetTaiKhoanCuDan({ userId: this.identifyUserId, email })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.identifyDirectLoading = false;
          if (res && res.isOk) {
            this.notification.success('Thành công', 'Đã định danh trực tiếp');
            try {
              this.identifyModalRef?.destroy();
            } catch (e) {}
            this.loadResidents();
          } else {
            this.notification.error('Lỗi', 'Định danh trực tiếp thất bại');
          }
        },
        error: () => {
          this.identifyDirectLoading = false;
          this.notification.error('Lỗi', 'Định danh trực tiếp thất bại');
        },
      });
  }
}
