import { Component, OnInit, Input, Output, EventEmitter, Optional } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ChungCuService } from '@features/resident/data-access';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-phuong-tien-form',
  templateUrl: './phuong-tien-form.component.html',
  styleUrls: ['./phuong-tien-form.component.scss']
})
export class PhuongTienFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  id: number | null = null;
  @Input() inModal = false;
  @Input() phuongTienId?: number;
  @Input() canHoId?: number;
  @Output() saved = new EventEmitter<any>();
  @Output() canceled = new EventEmitter<void>();
  loaiPhuongTienOptions: any[] = [];
  uploadFiles: File[] = [];
  displayFiles: any[] = [];
  existingImages: any[] = [];
  saving = false;

  constructor(
    private fb: FormBuilder,
    private svc: ChungCuService,
    private route: ActivatedRoute,
    private router: Router,
    private notification: NzNotificationService,
    @Optional() private modalRef?: NzModalRef
  ) {
    this.form = this.fb.group({
      canHoId: [null, Validators.required],
      tenPhuongTien: ['', Validators.required],
      loaiPhuongTienId: [null, Validators.required],
      bienSo: [''],
      mauXe: ['']
    });
  }

  ngOnInit(): void {
    this.loadCatalogs();
    if (this.phuongTienId) {
      this.isEdit = true;
      this.id = this.phuongTienId as number;
      this.load(this.id);
    } else {
      this.route.params.subscribe(p => {
        if (p && p.id) {
          this.isEdit = true;
          this.id = +p.id;
          this.load(this.id);
        }
      });
    }
    if (this.canHoId != null) {
      this.form.patchValue({ canHoId: +this.canHoId });
      this.form.get('canHoId')?.disable();
    } else {
      this.route.queryParams.subscribe((q: any) => {
        if (q && q['canHoId']) {
          this.form.patchValue({ canHoId: +q['canHoId'] });
          this.form.get('canHoId')?.disable();
        }
      });
    }
  }

  private loadCatalogs(): void {
    this.svc.getLoaiPhuongTienForSelector().subscribe(r => {
      if (r && r.isOk && Array.isArray(r.result)) this.loaiPhuongTienOptions = r.result;
    });
  }

  load(id: number): void {
    this.svc.getPhuongTienById(id).subscribe(r => {
      if (r && r.isOk && r.result) {
        this.form.patchValue({
          canHoId: r.result.canHoId,
          tenPhuongTien: r.result.tenPhuongTien,
          loaiPhuongTienId: r.result.loaiPhuongTienId,
          bienSo: r.result.bienSo,
          mauXe: r.result.mauXe
        });
        if (Array.isArray(r.result.hinhAnhPhuongTiens) && r.result.hinhAnhPhuongTiens.length) {
          const tmp = r.result.hinhAnhPhuongTiens.slice();
          tmp.sort((a: any, b: any) => (b.fileId || 0) - (a.fileId || 0));
          this.existingImages = tmp;
          if (this.isEdit) {
            this.existingImages = this.existingImages.slice(0, 1);
          }
          this.displayFiles = this.existingImages.map((img: any, idx: number) => ({
            uid: 'exist-' + (img.fileId ?? idx),
            name: img.fileName || ('Ảnh ' + (idx + 1)),
            size: null,
            originFileObj: null,
            thumbUrl: img.fileUrl,
            fileId: img.fileId,
            isExisting: true,
            isLocal: false
          }));
        }
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
      const payload = this.form.getRawValue();
      if (!payload.canHoId) {
        if (this.canHoId != null) payload.canHoId = +this.canHoId;
        else if (this.route && this.route.snapshot && this.route.snapshot.queryParams && this.route.snapshot.queryParams['canHoId']) {
          payload.canHoId = +this.route.snapshot.queryParams['canHoId'];
        }
      }
      const proceedCreateOrUpdate = (finalPayload: any) => {
        const handleResponse = (res: any) => {
          if (res && res.isOk && res.result) {
            if (Array.isArray(res.result.hinhAnhPhuongTiens) && res.result.hinhAnhPhuongTiens.length) {
              const tmp = res.result.hinhAnhPhuongTiens.slice();
              tmp.sort((a: any, b: any) => (b.fileId || 0) - (a.fileId || 0));
              this.existingImages = tmp;
              if (this.isEdit) this.existingImages = this.existingImages.slice(0, 1);
              this.displayFiles = this.existingImages.map((img: any, idx: number) => ({
                uid: 'exist-' + (img.fileId ?? idx),
                name: img.fileName || ('Ảnh ' + (idx + 1)),
                size: null,
                originFileObj: null,
                thumbUrl: img.fileUrl,
                fileId: img.fileId,
                isExisting: true,
                isLocal: false
              }));
            } else {
              this.existingImages = [];
              this.displayFiles = [];
            }
          }
          try { this.saved.emit(res && res.result ? res.result : undefined); } catch (e) {}
          if (this.inModal) {
            this.modalRef?.close();
          } else this.router.navigate(['../'], { relativeTo: this.route });
        };

        if (this.isEdit && this.id) {
          finalPayload.phuongTienId = this.id;
          this.svc.updatePhuongTien(finalPayload).subscribe({ next: handleResponse, error: () => handleResponse(null) });
        } else {
          this.svc.createPhuongTien(finalPayload).subscribe({ next: handleResponse, error: () => handleResponse(null) });
        }
      };

      if (this.uploadFiles && this.uploadFiles.length > 0) {
        this.svc.uploadMedia(this.uploadFiles, 'tai-lieu-phuong-tien').subscribe({
          next: res => {
            if (res && res.isOk && Array.isArray(res.result)) {
              const ids = res.result.map((x: any) => x && x.fileId).filter((id: any) => id !== null && id !== undefined);
              const existingIds = Array.isArray(this.existingImages) ? this.existingImages.map((x: any) => x.fileId).filter((id: any) => id != null) : [];
              const combined = Array.from(new Set([...(existingIds || []), ...(ids || [])]));
              payload.hinhAnhIds = this.isEdit ? combined.slice(0, 1) : combined;
              payload.hinhAnhPhuongTiens = (this.existingImages || []).slice();
            }
            proceedCreateOrUpdate(payload);
          },
          error: (err) => {
            const msg = err && err.error && err.error.errors && err.error.errors.length ? (err.error.errors[0].description || 'Tải ảnh thất bại') : 'Tải ảnh thất bại';
            if (this.inModal) {
              this.notification?.error?.('Lỗi', msg);
            } else {
              try { console.error('uploadMedia error', err); } catch (e) {}
            }
            this.saving = false;
          }
        });
        return;
      }
    // Ensure we always send current image ids to the backend (empty array when user removed all images)
    const existingIds = Array.isArray(this.existingImages) ? this.existingImages.map((x: any) => x.fileId).filter((id: any) => id != null) : [];
    payload.hinhAnhPhuongTiens = (this.existingImages || []).slice();
    const combined = Array.from(new Set((payload.hinhAnhIds || []).concat(existingIds)));
    payload.hinhAnhIds = this.isEdit ? (combined.slice(0, 1)) : combined;
    // helper: verify backend applied image ids, retry once if mismatch
    const verifyAndClose = (sentIds: number[] | undefined, checkId?: number) => {
      const idToCheck = checkId || this.id;
      if (!idToCheck) {
        if (this.inModal) { this.saved.emit(); this.modalRef?.close(); }
        else this.router.navigate(['../'], { relativeTo: this.route });
        return;
      }

      this.svc.getPhuongTienById(idToCheck).subscribe({ next: (r) => {
        const returnedFull = (r && r.isOk && r.result && Array.isArray(r.result.hinhAnhPhuongTiens)) ? r.result.hinhAnhPhuongTiens : [];
        const returned = returnedFull.map((x:any)=>x.fileId).filter((id:any)=>id!=null);
        const sent = Array.isArray(sentIds) ? sentIds.slice().filter((id:any)=>id!=null) : [];
        const sortNum = (a:number,b:number)=>a-b;
        returned.sort(sortNum); sent.sort(sortNum);
        try { console.debug('verifyAndClose - sentIds', sent, 'returnedIds', returned); } catch (e) {}
        const equal = returned.length === sent.length && returned.every((v: number, i: number) => v === sent[i]);
        if (equal) {
          // apply returned images to UI
          this.existingImages = returnedFull.slice();
          if (this.isEdit) this.existingImages = this.existingImages.slice(0, 1);
          this.displayFiles = this.existingImages.map((img: any, idx: number) => ({
            uid: 'exist-' + (img.fileId ?? idx),
            name: img.fileName || ('Ảnh ' + (idx + 1)),
            size: null,
            originFileObj: null,
            thumbUrl: img.fileUrl,
            fileId: img.fileId,
            isExisting: true,
            isLocal: false
          }));
          if (this.inModal) { this.saved.emit(); this.modalRef?.close(); }
          else this.router.navigate(['../'], { relativeTo: this.route });
          return;
        }

        // retry once with explicit payload containing required fields + hinhAnhIds
        const retryPayload: any = {
          phuongTienId: idToCheck,
          tenPhuongTien: this.form.get('tenPhuongTien')?.value,
          loaiPhuongTienId: this.form.get('loaiPhuongTienId')?.value,
          bienSo: this.form.get('bienSo')?.value,
          mauXe: this.form.get('mauXe')?.value,
          hinhAnhIds: sent
        };
        this.svc.updatePhuongTien(retryPayload).subscribe({ next: () => {
          this.svc.getPhuongTienById(idToCheck).subscribe({ next: (r2) => {
            const afterFull = (r2 && r2.isOk && r2.result && Array.isArray(r2.result.hinhAnhPhuongTiens)) ? r2.result.hinhAnhPhuongTiens : [];
            const after = afterFull.map((x:any)=>x.fileId).filter((id:any)=>id!=null);
            after.sort(sortNum);
            try { console.debug('retry check - sent', sent, 'after', after); } catch (e) {}
            if (after.length === sent.length && after.every((v: number, i: number) => v === sent[i])) {
              // update UI
              this.existingImages = afterFull.slice();
              if (this.isEdit) this.existingImages = this.existingImages.slice(0,1);
              this.displayFiles = this.existingImages.map((img: any, idx: number) => ({
                uid: 'exist-' + (img.fileId ?? idx),
                name: img.fileName || ('Ảnh ' + (idx + 1)),
                size: null,
                originFileObj: null,
                thumbUrl: img.fileUrl,
                fileId: img.fileId,
                isExisting: true,
                isLocal: false
              }));
              if (this.inModal) { this.saved.emit(); this.modalRef?.close(); }
              else this.router.navigate(['../'], { relativeTo: this.route });
            } else {
              this.notification.warning('Cảnh báo', 'Server không cập nhật được ảnh. Vui lòng kiểm tra API.');
              // apply server state so user sees actual data
              this.existingImages = afterFull.slice();
              if (this.isEdit) this.existingImages = this.existingImages.slice(0,1);
              this.displayFiles = this.existingImages.map((img: any, idx: number) => ({
                uid: 'exist-' + (img.fileId ?? idx),
                name: img.fileName || ('Ảnh ' + (idx + 1)),
                size: null,
                originFileObj: null,
                thumbUrl: img.fileUrl,
                fileId: img.fileId,
                isExisting: true,
                isLocal: false
              }));
              if (this.inModal) { this.saved.emit(); this.modalRef?.close(); }
              else this.router.navigate(['../'], { relativeTo: this.route });
            }
          }, error: () => {
            if (this.inModal) { this.saved.emit(); this.modalRef?.close(); }
            else this.router.navigate(['../'], { relativeTo: this.route });
          } });
        }, error: () => {
          this.notification.warning('Cảnh báo', 'Không thể áp dụng thay đổi ảnh trên server.');
          if (this.inModal) { this.saved.emit(); this.modalRef?.close(); }
          else this.router.navigate(['../'], { relativeTo: this.route });
        } });
      }, error: () => {
        if (this.inModal) { this.saved.emit(); this.modalRef?.close(); }
        else this.router.navigate(['../'], { relativeTo: this.route });
      } });
    };

    // perform initial update/create then verify
    if (this.isEdit && this.id) {
      payload.phuongTienId = this.id;
      this.svc.updatePhuongTien(payload).subscribe({ next: (res) => {
        const sentIds = payload.hinhAnhIds || [];
        const returnedId = res && res.isOk && res.result && res.result.id ? res.result.id : undefined;
        verifyAndClose(sentIds, returnedId);
      }, error: () => { verifyAndClose(payload.hinhAnhIds || []); } });
    } else {
      this.svc.createPhuongTien(payload).subscribe({ next: (res) => {
        const sentIds = payload.hinhAnhIds || [];
        const returnedId = res && res.isOk && res.result && res.result.id ? res.result.id : undefined;
        verifyAndClose(sentIds, returnedId);
      }, error: () => { verifyAndClose(payload.hinhAnhIds || []); } });
    }
  }

  beforeUpload = (file: any): boolean => {
    const f = file as File;
    const disp: any = { uid: 'n-' + Date.now(), name: f.name, size: f.size, originFileObj: f };
    if (f && f.type && f.type.startsWith('image/')) {
      try {
        disp.thumbUrl = URL.createObjectURL(f);
        disp.isLocal = true;
      } catch (e) {
        disp.thumbUrl = undefined;
      }
    }
    if (this.isEdit) {
      this.uploadFiles.push(f);
      this.displayFiles.push(disp);
    } else {
      this.uploadFiles.push(f);
      this.displayFiles.push(disp);
    }
    return false;
  }

  removeFile(idx: number): void {
    if (idx == null) return;
    const d = this.displayFiles[idx];
    if (!d) return;
    if (d.isExisting && d.fileId != null) {
      this.existingImages = (this.existingImages || []).filter(x => x.fileId !== d.fileId);
    } else {
      const foundIndex = this.uploadFiles.findIndex(f => f.name === d.name && f.size === d.size);
      if (foundIndex !== -1) this.uploadFiles.splice(foundIndex, 1);
    }
    if (d && d.isLocal && d.thumbUrl) {
      try { URL.revokeObjectURL(d.thumbUrl); } catch (e) { }
    }
    this.displayFiles.splice(idx, 1);
  }

  ngOnDestroy(): void {
    for (const d of this.displayFiles) {
      if (d && d.isLocal && d.thumbUrl) {
        try { URL.revokeObjectURL(d.thumbUrl); } catch (e) { }
      }
    }
  }


  onCancel(): void {
    if (this.inModal) {
      if (this.modalRef) this.modalRef.close();
      else this.canceled.emit();
    } else {
      this.router.navigate(['../'], { relativeTo: this.route });
    }
  }
}
