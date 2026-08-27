import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ChungCuService } from '@features/resident/data-access';
import { AuthApiService } from '@features/auth/data-access';
import { ProfileApiService } from '@features/profile/data-access';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, forkJoin } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-can-ho-residents',
  templateUrl: './can-ho-residents.component.html',
  styleUrls: ['./can-ho-residents.component.scss']
})
export class CanHoResidentsComponent implements OnInit, OnDestroy {
  canHoId!: number;
  items: any[] = [];
  filtered: any[] = [];
  listOfCurrentPageData: readonly any[] = [];
  setOfCheckedId = new Set<number | string>();
  checked = false;
  indeterminate = false;
  loading = false;
  keyword = '';
  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;

  // Thêm cư dân bằng SĐT
  searchState: 'idle' | 'found' | 'notFound' = 'idle';
  searchLoading = false;
  foundUser: any | null = null;
  currentUserId?: number;
  searchError = '';

  loaiQuanHeOptions: any[] = [];
  selectedLoaiQuanHeCuTruId: number | null = null;
  ngayBatDau: string = '';
  setResidenceLoading = false;

  registerForm: FormGroup;
  registerLoading = false;
  gioiTinhOptions: any[] = [];

  isAddingResident = false;

  // edit relation
  editingResident: any | null = null;
  editModalVisible = false;
  editForm: FormGroup;
  editLoading = false;

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private chungCu: ChungCuService,
    private modal: NzModalService,
    private authApi: AuthApiService,
    private profileApi: ProfileApiService,
    private fb: FormBuilder,
    private notification: NzNotificationService
  ) {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      phoneNumber: ['', [Validators.required]],
      idCard: ['', [Validators.required]],
      dob: ['', [Validators.required]],
      gioiTinhId: [null, [Validators.required]],
      diaChi: ['', [Validators.required]]
    });
    this.editForm = this.fb.group({
      quanHeCuTruId: [null, [Validators.required]],
      loaiQuanHeCuTruId: [null, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.canHoId = Number(id);
        this.load();
      }
    });

    this.loadLoaiQuanHeCuTruOptions();
    this.loadGioiTinhOptions();
  }

  load(): void {
    this.loading = true;
    const q: any = {
      canHoId: this.canHoId,
      keyword: this.keyword || undefined,
      loaiQuanHeCuTruId: this.selectedLoaiQuanHeCuTruId || undefined,
      ngayBatDauFrom: this.ngayBatDau || undefined,
      pageNumber: this.pageNumber || 1,
      pageSize: this.pageSize || 10
    };

    this.chungCu.getResidentsByCanHo(q).pipe(takeUntil(this.destroy$)).subscribe(res => {
      this.loading = false;
      if (res && res.isOk) {
        const payload = res.result;
        if (payload) {
          if (Array.isArray(payload)) {
            this.items = payload;
            this.totalItems = payload.length;
          } else if (payload.items && Array.isArray(payload.items)) {
            this.items = payload.items;
            this.totalItems = payload.pagingInfo?.totalItems || payload.items.length;
          } else {
            this.items = [];
            this.totalItems = 0;
          }
        } else {
          this.items = [];
          this.totalItems = 0;
        }
        this.applyFilter();
      } else {
        this.items = [];
        this.filtered = [];
        this.totalItems = 0;
      }
    }, _ => { this.loading = false; this.items = []; this.filtered = []; this.totalItems = 0; });
  }

  applyFilter(): void {
    const k = (this.keyword || '').trim().toLowerCase();
    if (!k) {
      this.filtered = this.items.slice();
    } else {
      this.filtered = this.items.filter(i => (i.hoTen || '').toLowerCase().includes(k) || (i.phoneNumber || '').toLowerCase().includes(k) || (i.email || '').toLowerCase().includes(k));
    }
  }

  refresh(): void {
    this.setOfCheckedId.clear();
    this.pageNumber = 1;
    this.keyword = '';
    this.load();
  }

  onCurrentPageDataChange(list: readonly any[]): void {
    this.listOfCurrentPageData = list;
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    if (checked) {
      this.listOfCurrentPageData.forEach(item => this.setOfCheckedId.add(item.quanHeCuTruId || item.userId));
    } else {
      this.listOfCurrentPageData.forEach(item => this.setOfCheckedId.delete(item.quanHeCuTruId || item.userId));
    }
    this.refreshCheckedStatus();
  }

  onItemChecked(id: number | string, checked: boolean): void {
    if (checked) this.setOfCheckedId.add(id); else this.setOfCheckedId.delete(id);
    this.refreshCheckedStatus();
  }

  editResident(item: any): void {
    this.editingResident = item;
    this.editForm.reset();
    this.editForm.patchValue({
      quanHeCuTruId: item.quanHeCuTruId || item.userId,
      loaiQuanHeCuTruId: item.loaiQuanHeCuTruId || null
    });
    this.editModalVisible = true;
  }

  submitEdit(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }
    const val = this.editForm.value;
    const payload: any = {
      quanHeCuTruId: val.quanHeCuTruId,
      loaiQuanHeCuTruId: val.loaiQuanHeCuTruId
    };

    this.editLoading = true;
    this.chungCu.updateQuanHeCuTru(payload).pipe(takeUntil(this.destroy$)).subscribe(res => {
      this.editLoading = false;
      if (res && res.isOk) {
        this.notification.success('Thành công', 'Cập nhật thành công');
        this.editModalVisible = false;
        this.editingResident = null;
        this.load();
      } else {
        this.notification.error('Lỗi', 'Cập nhật thất bại');
      }
    }, _ => { this.editLoading = false; this.notification.error('Lỗi', 'Cập nhật thất bại'); });
  }

  closeEdit(): void {
    this.editModalVisible = false;
    this.editingResident = null;
  }

  refreshCheckedStatus(): void {
    this.checked = this.listOfCurrentPageData.length > 0 && this.listOfCurrentPageData.every(item => this.setOfCheckedId.has(item.quanHeCuTruId || item.userId));
    this.indeterminate = this.listOfCurrentPageData.some(item => this.setOfCheckedId.has(item.quanHeCuTruId || item.userId)) && !this.checked;
  }

  onPageChange(page: number): void { this.pageNumber = page; this.load(); }
  onPageSizeChange(size: number): void { this.pageSize = size; this.pageNumber = 1; this.load(); }

  // ====== Thêm cư dân bằng SĐT ======

  private loadLoaiQuanHeCuTruOptions(): void {
    this.chungCu.getLoaiQuanHeCuTruForSelector()
      .pipe(takeUntil(this.destroy$))
      .subscribe(r => {
        if (r && r.isOk && Array.isArray(r.result)) this.loaiQuanHeOptions = r.result;
      });
  }

  private loadGioiTinhOptions(): void {
    this.profileApi.getGioiTinhForSelector()
      .pipe(takeUntil(this.destroy$))
      .subscribe(r => {
        if (r && r.isOk && Array.isArray(r.result)) this.gioiTinhOptions = r.result;
      });
  }

  onSearchUserByPhone(): void {
    const phone = (this.registerForm.get('phoneNumber')?.value || '').trim();
    if (!phone) {
      this.searchError = 'Vui lòng nhập số điện thoại.';
      return;
    }
    this.searchError = '';
    this.searchLoading = true;
    this.foundUser = null;
    this.currentUserId = undefined;
    this.searchState = 'idle';

    this.chungCu.searchUserForQuanHeCuTru(phone)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.searchLoading = false;
          if (res && res.isOk && res.result) {
            this.searchError = '';
            this.foundUser = res.result;
            this.currentUserId = res.result.userId;
            this.selectedLoaiQuanHeCuTruId = null;
            this.ngayBatDau = new Date().toISOString().substring(0, 10);
            this.searchState = 'found';
            this.registerForm.patchValue({
              username: res.result.username || '',
              email: res.result.email || '',
              firstName: res.result.firstName || '',
              lastName: res.result.lastName || '',
              phoneNumber: res.result.phoneNumber || phone,
              idCard: res.result.idCard || '',
              dob: res.result.dob ? new Date(res.result.dob).toISOString().substring(0, 10) : '',
              gioiTinhId: res.result.gioiTinhId ?? null
            });
          } else {
            if (res && Array.isArray(res.errors) && res.errors.length) {
              this.searchError = res.errors.map((e: any) => e.description || e.message || '').filter(Boolean).join('; ');
            } else {
              this.searchError = '';
            }
            this.searchState = 'notFound';
            if (!this.gioiTinhOptions.length) this.loadGioiTinhOptions();
          }
        },
        error: (err: any) => {
          this.searchLoading = false;
          this.searchState = 'notFound';
          if (err && err.error && Array.isArray(err.error.errors) && err.error.errors.length) {
            this.searchError = err.error.errors.map((e: any) => e.description || e.message || '').filter(Boolean).join('; ');
          } else if (err && err.error && err.error.message) {
            this.searchError = err.error.message;
          }
          if (!this.gioiTinhOptions.length) this.loadGioiTinhOptions();
        }
      });
  }

  cancelRegister(): void {
    this.isAddingResident = false;
    this.searchState = 'idle';
    this.searchError = '';
    this.foundUser = null;
    this.currentUserId = undefined;
    this.selectedLoaiQuanHeCuTruId = null;
    this.registerForm.reset();
  }

  submitRegister(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const val = this.registerForm.value;
    const dobIso = val.dob ? new Date(val.dob).toISOString() : new Date().toISOString();

    this.registerLoading = true;
    this.authApi.registerResident({
      username: val.username,
      email: val.email,
      password: val.password,
      firstName: val.firstName,
      lastName: val.lastName,
      phoneNumber: val.phoneNumber,
      idCard: val.idCard,
      dob: dobIso,
      gioiTinhId: val.gioiTinhId,
      diaChi: val.diaChi
    }).pipe(takeUntil(this.destroy$)).subscribe(res => {
      this.registerLoading = false;
      if (res && res.isOk && res.result) {
        this.foundUser = {
          id: res.result.userId,
          username: res.result.username,
          email: res.result.email,
          fullName: res.result.fullName,
          phoneNumber: val.phoneNumber
        };
        this.currentUserId = res.result.userId;
        this.searchState = 'found';
        this.modal.success({ nzTitle: 'Đăng ký cư dân thành công' });
      }
    }, _ => {
      this.registerLoading = false;
    });
  }

  setResidence(): void {
    if (!this.canHoId || !this.currentUserId || !this.selectedLoaiQuanHeCuTruId) {
      this.searchError = 'Vui lòng chọn loại quan hệ trước khi thiết lập cư trú.';
      return;
    }
    this.searchError = '';
    const payload: any = {
      canHoId: this.canHoId,
      userId: this.currentUserId,
      loaiQuanHeCuTruId: this.selectedLoaiQuanHeCuTruId
    };
    if (this.ngayBatDau) {
      payload.ngayBatDau = new Date(this.ngayBatDau).toISOString();
    }

    this.setResidenceLoading = true;
    this.chungCu.createQuanHeCuTru(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe(res => {
        this.setResidenceLoading = false;
        if (res && res.isOk) {
          this.notification.success('Thành công', 'Thiết lập cư trú thành công');
          this.isAddingResident = false;
          this.searchState = 'idle';
          this.foundUser = null;
          this.currentUserId = undefined;
          this.selectedLoaiQuanHeCuTruId = null;
          this.registerForm.reset();
          this.load();
        }
      }, _ => {
        this.setResidenceLoading = false;
      });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }

  deleteOne(id?: number | string): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Kết thúc cư trú',
      nzContent: 'Bạn có chắc chắn muốn kết thúc cư trú của cư dân này tại căn hộ?',
      nzOkText: 'Đồng ý',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => this.chungCu.ketThucQuanHeCuTru(id).subscribe(() => this.load())
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
      nzContent: `Bạn có chắc chắn muốn kết thúc cư trú của ${ids.length} cư dân đã chọn tại căn hộ này?`,
      nzOkText: 'Đồng ý',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        forkJoin(ids.map(id => this.chungCu.ketThucQuanHeCuTru(id))).subscribe(() => {
          this.setOfCheckedId.clear();
          this.load();
        });
      }
    });
  }
}
