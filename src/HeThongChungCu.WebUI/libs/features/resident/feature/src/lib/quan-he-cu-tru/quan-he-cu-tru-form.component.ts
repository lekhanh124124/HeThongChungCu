import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ChungCuService } from '@features/resident/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ProfileApiService } from '@features/profile/data-access';
import { Subject } from 'rxjs';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-quan-he-cu-tru-form',
  templateUrl: './quan-he-cu-tru-form.component.html',
  styleUrls: ['./quan-he-cu-tru-form.component.scss']
})
export class QuanHeCuTruFormComponent implements OnInit, OnDestroy {
  @Input() canHoId?: number;
  @Input() loaiQuanHeOptions: any[] = [];
  @Output() saved = new EventEmitter<void>();

  registerForm!: FormGroup;
  registerLoading = false;
  searchLoading = false;
  foundUser: any | null = null;
  currentUserId?: number;
  searchError = '';
  gioiTinhOptions: any[] = [];


  documentCards: Array<{ meta: { loaiGiayToId: number; soGiayTo: string; ngayPhatHanh: string }; files: File[]; displayFiles: NzUploadFile[] }> = [];
  documentFilesCount = 0;
  currentAttachCardIndex: number | null = null;
  private seenCentralUploadUids = new Set<string>();

  loaiGiayToOptions: Array<{ id: number; name: string }> = [];

  isAddingResident = false;
  selectedLoaiQuanHeCuTruId: number | null = null;
  ngayBatDau: string = '';
  setResidenceLoading = false;

  editingResident: any | null = null;
  editModalVisible = false;
  editForm!: FormGroup;
  editLoading = false;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private chungCu: ChungCuService,
    private notification: NzNotificationService,
    private profileApi: ProfileApiService
  ) {
    this.registerForm = this.fb.group({
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
    this.loadGioiTinhOptions();
    this.loadLoaiGiayToOptions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private resolveUserId(input: any): number | undefined {
    const candidate = input && typeof input === 'object'
      ? (input.userId ?? input.id)
      : input;
    const n = Number(candidate);
    return Number.isFinite(n) && n > 0 ? n : undefined;
  }

  private loadGioiTinhOptions(): void {
    this.profileApi.getGioiTinhForSelector()
      .pipe(takeUntil(this.destroy$))
      .subscribe(r => {
        if (r && r.isOk && Array.isArray(r.result)) this.gioiTinhOptions = r.result;
      });
  }

  private loadLoaiGiayToOptions(): void {
    this.chungCu.getLoaiGiayToForSelector()
      .pipe(takeUntil(this.destroy$))
      .subscribe(r => {
        if (r && r.isOk && Array.isArray(r.result)) {
          this.loaiGiayToOptions = r.result.map((x: any) => ({ id: x.id ?? x.value ?? x.key ?? 0, name: x.name ?? x.ten ?? x.label ?? x.text ?? '' }));
        }
      });
  }

  openAdd(): void { this.isAddingResident = true; }
  closeAdd(): void { this.cancelRegister(); }

  onSearchUserByIdCard(): void {
    const idCard = (this.registerForm.get('idCard')?.value || '').trim();
    if (!idCard) { this.searchError = 'Vui lòng nhập số CCCD/CMND.'; return; }
    this.searchError = '';
    this.searchLoading = true;
    this.foundUser = null;
    this.currentUserId = undefined;

    this.chungCu.searchUserForQuanHeCuTru(idCard).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.searchLoading = false;
        if (res && res.isOk && res.result) {
          this.foundUser = res.result;
          const u: any = res.result;
          this.currentUserId = this.resolveUserId(u);
          try {
            this.registerForm.patchValue({
              firstName: u.firstName || u.fullName || '',
              lastName: u.lastName || '',
              phoneNumber: u.phoneNumber || '',
              idCard: u.idCard || '',
              dob: u.dob ? (new Date(u.dob).toISOString().substring(0, 10)) : '',
              gioiTinhId: u.gioiTinhId ?? null,
              diaChi: u.diaChi || ''
            });
          } catch (e) { }
          this.selectedLoaiQuanHeCuTruId = null;
          this.ngayBatDau = new Date().toISOString().substring(0, 10);
        } else {
          this.foundUser = null;
          this.currentUserId = undefined;
        }
      },
      error: () => { this.searchLoading = false; }
    });
  }

  cancelRegister(): void {
    this.isAddingResident = false;
    this.searchError = '';
    this.foundUser = null;
    this.currentUserId = undefined;
    this.selectedLoaiQuanHeCuTruId = null;
    this.registerForm.reset();
    this.documentCards = [];
    this.documentFilesCount = 0;
    this.seenCentralUploadUids.clear();
  }

  addDocumentCard(): void {
    this.documentCards.push({
      meta: { loaiGiayToId: 0, soGiayTo: (this.registerForm.get('idCard')?.value || '').toString(), ngayPhatHanh: '' },
      files: [],
      displayFiles: []
    });
    this.documentFilesCount = this.documentCards.reduce((s, c) => s + c.displayFiles.length, 0);
  }

  removeDocumentCard(index: number): void {
    if (index < 0 || index >= this.documentCards.length) return;
    this.documentCards.splice(index, 1);
    this.documentFilesCount = this.documentCards.reduce((s, c) => s + c.displayFiles.length, 0);
    if (this.currentAttachCardIndex === index) {
      this.currentAttachCardIndex = null;
    } else if (this.currentAttachCardIndex != null && this.currentAttachCardIndex > index) {
      this.currentAttachCardIndex -= 1;
    }
  }

  removeDocumentFile(cardIndex: number, fileIndex: number): void {
    if (cardIndex < 0 || cardIndex >= this.documentCards.length) return;
    const card = this.documentCards[cardIndex];
    if (!card || fileIndex < 0 || fileIndex >= card.displayFiles.length) return;
    card.displayFiles.splice(fileIndex, 1);
    if (fileIndex < card.files.length) card.files.splice(fileIndex, 1);
    this.documentFilesCount = this.documentCards.reduce((s, c) => s + c.displayFiles.length, 0);
  }

  startAddFileToCard(index: number, inputEl: HTMLInputElement): void {
    this.currentAttachCardIndex = index;
    inputEl.value = '';
    inputEl.click();
  }

  onCardFileInput(evt: Event): void {
    const input = evt.target as HTMLInputElement;
    if (!input.files || input.files.length === 0 || this.currentAttachCardIndex == null) return;
    const idx = this.currentAttachCardIndex;
    const fList = Array.from(input.files);
    const displayFiles: NzUploadFile[] = fList.map((f, i) => ({ uid: 'n-' + Date.now() + '-' + i, name: f.name, size: f.size, originFileObj: f } as any));
    if (!this.documentCards[idx]) return;
    this.documentCards[idx].files = this.documentCards[idx].files.concat(fList);
    this.documentCards[idx].displayFiles = this.documentCards[idx].displayFiles.concat(displayFiles);
    this.documentFilesCount = this.documentCards.reduce((s, c) => s + c.displayFiles.length, 0);
    this.currentAttachCardIndex = null;
  }

  submitRegister(): void {
    if (this.registerForm.invalid) { this.registerForm.markAllAsTouched(); return; }
    const val = this.registerForm.value;
    const dobIso = val.dob ? new Date(val.dob).toISOString() : new Date().toISOString();
    const basePayload: any = { firstName: val.firstName, lastName: val.lastName, dob: dobIso, gioiTinhId: val.gioiTinhId, diaChi: val.diaChi, idCard: val.idCard, phoneNumber: val.phoneNumber };
    this.registerLoading = true;

    if (this.documentCards && this.documentCards.length > 0) {
      const allFiles: File[] = [];
      this.documentCards.forEach(c => { c.files.forEach(f => allFiles.push(f)); });
      if (!allFiles.length) { this.registerLoading = false; this.notification.error('Lỗi', 'Không tìm thấy file để tải lên. Vui lòng thử lại.'); return; }
      this.chungCu.uploadMedia(allFiles, 'tai-lieu-cu-tru').pipe(takeUntil(this.destroy$)).subscribe({
        next: res => {
          if (res && res.isOk && Array.isArray(res.result) && res.result.length > 0) {
            const uploaded = res.result.map((x: any) => x && x.fileId).filter((id: any) => id !== null && id !== undefined);
            let cursor = 0;
            const documents = this.documentCards.map(card => {
              const count = card.files.length;
              const ids = uploaded.slice(cursor, cursor + count);
              cursor += count;
              return { loaiGiayToId: card.meta.loaiGiayToId ?? 0, soGiayTo: card.meta.soGiayTo || val.idCard, ngayPhatHanh: card.meta.ngayPhatHanh ? new Date(card.meta.ngayPhatHanh).toISOString() : new Date().toISOString(), fileIds: ids || [] };
            }).filter(d => d.fileIds && d.fileIds.length > 0);
            const payload: any = { ...basePayload, taiLieuCuTrus: documents };
            this.chungCu.taoHoSoCuDan(payload).pipe(takeUntil(this.destroy$)).subscribe({
              next: res2 => {
                this.registerLoading = false;
                if (res2 && res2.isOk) {
                  const newUserId = this.resolveUserId(res2.result);
                  if (!newUserId) { this.notification.error('Lỗi', 'Không nhận được userId hợp lệ sau khi tạo hồ sơ.'); return; }
                  this.currentUserId = newUserId;
                  this.foundUser = { id: newUserId, firstName: val.firstName, lastName: val.lastName, fullName: `${val.lastName || ''} ${val.firstName || ''}`.trim(), phoneNumber: val.phoneNumber, idCard: val.idCard, dob: dobIso, gioiTinhId: val.gioiTinhId, diaChi: val.diaChi };
                  this.notification.success('Thành công', 'Đã tạo hồ sơ cư dân mới');
                } else { this.notification.error('Lỗi', 'Tạo hồ sơ cư dân thất bại'); }
              },
              error: () => { this.registerLoading = false; this.notification.error('Lỗi', 'Tạo hồ sơ cư dân thất bại'); }
            });
          } else { this.registerLoading = false; this.notification.error('Lỗi', 'Tải lên tài liệu thất bại'); }
        },
        error: () => { this.registerLoading = false; this.notification.error('Lỗi', 'Tải lên tài liệu thất bại'); }
      });
    } else {
      const payload: any = { ...basePayload, taiLieuCuTrus: [] };
      this.chungCu.taoHoSoCuDan(payload).pipe(takeUntil(this.destroy$)).subscribe({
        next: res => {
          this.registerLoading = false;
          if (res && res.isOk) {
            const newUserId = this.resolveUserId(res.result);
            if (!newUserId) { this.notification.error('Lỗi', 'Không nhận được userId hợp lệ sau khi tạo hồ sơ.'); return; }
            this.currentUserId = newUserId;
            this.foundUser = { id: newUserId, firstName: val.firstName, lastName: val.lastName, fullName: `${val.lastName || ''} ${val.firstName || ''}`.trim(), phoneNumber: val.phoneNumber, idCard: val.idCard, dob: dobIso, gioiTinhId: val.gioiTinhId, diaChi: val.diaChi };
            this.notification.success('Thành công', 'Đã tạo hồ sơ cư dân mới');
          } else { this.notification.error('Lỗi', 'Tạo hồ sơ cư dân thất bại'); }
        },
        error: () => { this.registerLoading = false; this.notification.error('Lỗi', 'Tạo hồ sơ cư dân thất bại'); }
      });
    }
  }

  onUploadChange(_evt: { file: NzUploadFile; fileList: NzUploadFile[] }): void { }

  beforeUpload = (_file: NzUploadFile, _fileList: NzUploadFile[]): boolean => {
    return false;
  };

  handleRemove = (_file: NzUploadFile): boolean => { return true; };

  setResidence(): void {
    const resolvedUserId = this.resolveUserId(this.currentUserId ?? this.foundUser);
    if (!resolvedUserId || !this.selectedLoaiQuanHeCuTruId) { this.searchError = 'Vui lòng chọn loại quan hệ trước khi thiết lập cư trú.'; return; }
    this.searchError = '';
    const payload: any = { canHoId: this.canHoId, userId: resolvedUserId, loaiQuanHeCuTruId: this.selectedLoaiQuanHeCuTruId };
    if (this.ngayBatDau) payload.ngayBatDau = new Date(this.ngayBatDau).toISOString();
    this.setResidenceLoading = true;
    this.chungCu.createQuanHeCuTru(payload).pipe(takeUntil(this.destroy$)).subscribe(res => {
      this.setResidenceLoading = false;
      if (res && res.isOk) {
        this.notification.success('Thành công', 'Thiết lập cư trú thành công');
        this.isAddingResident = false;
        this.registerForm.reset();
        this.saved.emit();
      } else { this.notification.error('Lỗi', 'Thiết lập cư trú thất bại'); }
    }, _ => { this.setResidenceLoading = false; this.notification.error('Lỗi', 'Thiết lập cư trú thất bại'); });
  }

  editResident(item: any): void {
    this.editingResident = item;
    this.editForm.reset();
    this.editForm.patchValue({ quanHeCuTruId: item.quanHeCuTruId || item.userId, loaiQuanHeCuTruId: item.loaiQuanHeCuTruId || null });
    this.editModalVisible = true;
  }

  submitEdit(): void {
    if (this.editForm.invalid) { this.editForm.markAllAsTouched(); return; }
    const val = this.editForm.value;
    const payload: any = { quanHeCuTruId: val.quanHeCuTruId, loaiQuanHeCuTruId: val.loaiQuanHeCuTruId };
    this.editLoading = true;
    this.chungCu.updateQuanHeCuTru(payload).pipe(takeUntil(this.destroy$)).subscribe(res => {
      this.editLoading = false;
      if (res && res.isOk) { this.notification.success('Thành công', 'Cập nhật thành công'); this.editModalVisible = false; this.editingResident = null; this.saved.emit(); } else this.notification.error('Lỗi', 'Cập nhật thất bại');
    }, _ => { this.editLoading = false; this.notification.error('Lỗi', 'Cập nhật thất bại'); });
  }

  closeEdit(): void { this.editModalVisible = false; this.editingResident = null; }
}
