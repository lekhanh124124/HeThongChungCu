import { Component, Input, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ChungCuService } from '@features/resident/data-access';
import { ProfileApiService } from '@features/profile/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService, NzModalRef } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-ho-so-cu-dan',
  templateUrl: './ho-so-cu-dan.component.html',
  styleUrls: ['./ho-so-cu-dan.component.scss']
})
export class HoSoCuDanComponent implements OnInit {
  @Input() profile: any;
  editMode = false;
  editForm!: FormGroup;
  saving = false;
  private currentModal?: NzModalRef;

  @ViewChild('editTpl', { static: true }) editTpl?: TemplateRef<any>;
  loaiQuanHeOptions: any[] = [];
  loaiGiayToOptions: any[] = [];
  gioiTinhOptions: any[] = [];

  documentCards: Array<{ meta: { loaiGiayToId: number; soGiayTo: string; ngayPhatHanh: string }; files: File[]; existingFiles?: any[]; taiLieuCuTruId?: number | string }> = [];
  currentAttachCardIndex: number | null = null;

  constructor(
    private fb: FormBuilder,
    private chungCu: ChungCuService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    private profileApi: ProfileApiService
  ) {}

  ngOnInit(): void {
    this.editForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      gioiTinhId: [null, []],
      loaiQuanHeCuTruId: [null, [Validators.required]],
      phoneNumber: ['', []],
      idCard: ['', []],
      dob: ['', [Validators.required]],
      diaChi: ['', []]
    });
    this.patchFormFromProfile();
    this.loadLoaiQuanHeOptions();
    this.loadGioiTinhOptions();
    this.loadLoaiGiayToOptions();
  }

  private patchFormFromProfile(): void {
    if (!this.profile || !this.editForm) return;
    try {
      this.editForm.patchValue({
        firstName: this.profile.firstName || this.profile.fullName || '',
        lastName: this.profile.lastName || '',
        gioiTinhId: this.profile.gioiTinhId || this.profile.yeuCauGioiTinhId || null,
        phoneNumber: this.profile.phoneNumber || '',
        idCard: this.profile.idCard || '',
        dob: this.profile.dob ? (new Date(this.profile.dob).toISOString().substring(0, 10)) : '',
        diaChi: this.profile.diaChi || ''
      });
      try {
        const rel = this.profile.loaiQuanHeCuTruId || this.profile.loaiQuanHeId || this.profile.targetQuanHeCuTruId || this.profile.loaiQuanHeId;
        if (rel) this.editForm.patchValue({ loaiQuanHeCuTruId: rel });
      } catch (e) {}
      this.documentCards = [];
      if (Array.isArray(this.profile.taiLieuCuTrus)) {
        this.profile.taiLieuCuTrus.forEach((d: any) => {
          this.documentCards.push({
            meta: {
              loaiGiayToId: d.loaiGiayToId ?? d.loaiGiayTo ?? 0,
              soGiayTo: d.soGiayTo || '',
              ngayPhatHanh: d.ngayPhatHanh ? (new Date(d.ngayPhatHanh).toISOString().substring(0, 10)) : ''
            },
            files: [],
            existingFiles: Array.isArray(d.files) ? d.files : [],
            taiLieuCuTruId: d.id ?? d.taiLieuCuTruId
          });
        });
      }
    } catch (e) {}
  }

  openEdit(): void {
    this.patchFormFromProfile();
    this.currentModal = this.modal.create({
      nzTitle: 'Chỉnh sửa hồ sơ cư dân',
      nzContent: this.editTpl,
      nzFooter: null,
      nzWidth: 720,
      nzBodyStyle: { 'max-height': '70vh', 'overflow': 'auto' }
    });
  }

  cancelEdit(): void {
    this.currentModal?.close();
  }

  saveEdit(): void {
    if (!this.profile) return;
    if (this.editForm.invalid) { this.editForm.markAllAsTouched(); return; }
    const val = this.editForm.value;
    const basePayload: any = {
      quanHeCuTruId: this.profile.quanHeCuTruId || this.profile.userId || this.profile.id,
      firstName: val.firstName,
      lastName: val.lastName,
      gioiTinhId: val.gioiTinhId,
      loaiQuanHeCuTruId: val.loaiQuanHeCuTruId,
      dob: val.dob ? new Date(val.dob).toISOString() : undefined,
      idCard: val.idCard,
      phoneNumber: val.phoneNumber,
      diaChi: val.diaChi
    };

    const existing = Array.isArray(this.profile.taiLieuCuTrus) ? this.profile.taiLieuCuTrus.map((d: any) => ({
      taiLieuCuTruId: d.id ?? d.taiLieuCuTruId ?? undefined,
      loaiGiayToId: d.loaiGiayToId ?? d.loaiGiayTo ?? 0,
      soGiayTo: d.soGiayTo || '',
      ngayPhatHanh: d.ngayPhatHanh ? new Date(d.ngayPhatHanh).toISOString() : undefined,
      fileIds: Array.isArray(d.files) ? d.files.map((f: any) => f.id || f.fileId).filter((x: any) => x != null) : []
    })) : [];

    const newFilesList: File[] = [];
    this.documentCards.forEach(c => c.files.forEach(f => newFilesList.push(f)));

    this.saving = true;

    const sendUpdate = (newDocsFileIdsMap: number[][]) => {
      const docs = this.documentCards.map((c, idx) => {
        const existingFileIds = Array.isArray(c.existingFiles) ? c.existingFiles.map((f: any) => f.id || f.fileId).filter((x: any) => x != null) : [];
        const newIds = newDocsFileIdsMap[idx] || [];
        const fileIds = existingFileIds.concat(newIds);
        const doc: any = {
          loaiGiayToId: c.meta.loaiGiayToId || 0,
          soGiayTo: c.meta.soGiayTo || '',
          ngayPhatHanh: c.meta.ngayPhatHanh ? new Date(c.meta.ngayPhatHanh).toISOString() : undefined,
          fileIds: fileIds
        };
        if (c.taiLieuCuTruId) doc.taiLieuCuTruId = c.taiLieuCuTruId;
        return doc;
      }).filter(d => d.fileIds && d.fileIds.length > 0 || d.taiLieuCuTruId);

      const payload: any = { ...basePayload, taiLieuCuTrus: docs };

      try {
        const origIds = Array.isArray(this.profile?.taiLieuCuTrus) ? this.profile.taiLieuCuTrus.map((d: any) => d.id ?? d.taiLieuCuTruId).filter((x: any) => x != null) : [];
        const currentIds = this.documentCards.map(c => c.taiLieuCuTruId).filter((x: any) => x != null);
        const removed = origIds.filter((id: any) => !currentIds.includes(id));
        if (removed.length) payload.deletedTaiLieuCuTruIds = removed;
      } catch (e) {}

      this.chungCu.updateHoSoCuDan(payload).subscribe({
        next: (res: any) => {
          this.saving = false;
          if (res && res.isOk) {
            this.notification.success('Thành công', 'Cập nhật hồ sơ thành công');
            this.profile = res.result || this.profile;
            this.currentModal?.close();
          } else {
            this.notification.error('Lỗi', 'Cập nhật hồ sơ thất bại');
          }
        },
        error: () => { this.saving = false; this.notification.error('Lỗi', 'Cập nhật hồ sơ thất bại'); }
      });
    };

    if (newFilesList.length === 0) {
      sendUpdate([]);
      return;
    }

    this.chungCu.uploadMedia(newFilesList, 'tai-lieu-cu-tru').subscribe({
      next: (res: any) => {
        if (res && res.isOk && Array.isArray(res.result)) {
          const uploaded = res.result.map((x: any) => x && (x.fileId ?? x.id)).filter((id: any) => id != null);
          const map: number[][] = [];
          let cursor = 0;
          this.documentCards.forEach(c => {
            const count = c.files.length;
            map.push(uploaded.slice(cursor, cursor + count));
            cursor += count;
          });
          sendUpdate(map);
        } else {
          this.saving = false;
          this.notification.error('Lỗi', 'Tải lên tài liệu thất bại');
        }
      },
      error: () => { this.saving = false; this.notification.error('Lỗi', 'Tải lên tài liệu thất bại'); }
    });
  }

  loadLoaiQuanHeOptions(): void {
    this.chungCu.getLoaiQuanHeCuTruForSelector().subscribe(r => { if (r && r.isOk && Array.isArray(r.result)) this.loaiQuanHeOptions = r.result; });
  }

  loadLoaiGiayToOptions(): void {
    this.chungCu.getLoaiGiayToForSelector().subscribe(r => { if (r && r.isOk && Array.isArray(r.result)) this.loaiGiayToOptions = r.result; });
  }

  loadGioiTinhOptions(): void {
    this.profileApi.getGioiTinhForSelector().subscribe(r => { if (r && r.isOk && Array.isArray(r.result)) this.gioiTinhOptions = r.result; });
  }

  addDocumentCard(): void {
    this.documentCards.push({ meta: { loaiGiayToId: 0, soGiayTo: this.editForm.get('idCard')?.value || '', ngayPhatHanh: '' }, files: [] });
  }

  removeDocumentCard(idx: number): void {
    if (idx >= 0 && idx < this.documentCards.length) this.documentCards.splice(idx, 1);
  }

  onCardFileInput(evt: Event, idx?: number): void {
    const input = evt.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;
    const files = Array.from(input.files);
    const targetIdx = (typeof idx === 'number') ? idx : this.currentAttachCardIndex;
    if (typeof targetIdx !== 'number' || !this.documentCards[targetIdx]) return;
    this.documentCards[targetIdx].files = this.documentCards[targetIdx].files.concat(files);
    this.currentAttachCardIndex = null;
  }

  removeExistingFile(cardIndex: number, existingIndex: number): void {
    const card = this.documentCards[cardIndex];
    if (!card || !Array.isArray(card.existingFiles)) return;
    if (existingIndex >= 0 && existingIndex < card.existingFiles.length) {
      card.existingFiles.splice(existingIndex, 1);
    }
  }

  startAddFileToCard(index: number, inputEl: HTMLInputElement): void {
    this.currentAttachCardIndex = index;
    inputEl.value = '';
    inputEl.click();
  }

  removeDocumentFile(cardIndex: number, fileIndex: number): void {
    if (!this.documentCards[cardIndex]) return;
    const card = this.documentCards[cardIndex];
    if (fileIndex >= 0 && fileIndex < (card.files || []).length) {
      card.files.splice(fileIndex, 1);
    }
  }
}
