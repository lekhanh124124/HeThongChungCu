import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { DoiTacApiService } from '../../../../data-access/src/lib/doi-tac.service';
import { DichVuService } from '@features/dich-vu/data-access';
import { DoiTacCreateComponent } from '../doi-tac-form/doi-tac-form.component';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-doi-tac-list',
  templateUrl: './doi-tac-list.component.html',
  styleUrls: ['./doi-tac-list.component.scss'],
})
export class DoiTacListComponent implements OnInit {
  items: any[] = [];
  loading = false;
  isModalVisible = false;
  modalTitle = '';
  saving = false;
  editingId?: number | null;

  // paging / searching / sorting
  pageSize = 20;
  pageNumber = 1;
  total = 0;

  keyword = '';
  sortCol: string | null = null;
  isAsc = true;

  // selection state (checkboxes)
  listOfCurrentPageData: readonly any[] = [];
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;

  @ViewChild(DoiTacCreateComponent) formComp?: DoiTacCreateComponent;
  // contracts modal state
  contractModalVisible = false;
  contractLoading = false;
  contractList: any[] = [];
  contractPartnerName = '';
  contractPartnerId?: number | null;
  contractPartnerDto: any = null;
  dichVuOptions: any[] = [];
  trangThaiHopDongOptions: any[] = [];

  constructor(
    private api: DoiTacApiService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    private dichVuService: DichVuService
  ) {}

  ngOnInit(): void {
    this.load();
    try {
      this.dichVuService.getLoaiDichVuForSelector({}).subscribe({
          next: (res: any) => {
            this.dichVuOptions = Array.isArray(res) ? res : (res?.result?.items ?? res?.result ?? res?.data ?? []);
          },
        error: () => { this.dichVuOptions = []; }
      });
    } catch (e) { this.dichVuOptions = []; }
    try {
      this.api.getTrangThaiHopDongForSelector({}).subscribe({
        next: (res: any) => { this.trangThaiHopDongOptions = Array.isArray(res) ? res : (res?.result?.items ?? res?.result ?? res?.data ?? []); },
        error: () => { this.trangThaiHopDongOptions = []; }
      });
    } catch (e) { this.trangThaiHopDongOptions = []; }
  }

  load(): void {
    const query: any = {
      keyword: this.keyword || undefined,
      sortCol: this.sortCol || undefined,
      isAsc: this.sortCol ? this.isAsc : undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };

    this.loading = true;
    this.api.getList(query).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.items = (res?.result?.items) || [];
        const pi = res?.result?.pagingInfo;
        if (pi) {
          this.pageSize = pi.pageSize || this.pageSize;
          this.pageNumber = pi.pageNumber || this.pageNumber;
          this.total = pi.totalItems || 0;
        } else {
          this.total = this.items.length;
        }
      },
      error: () => {
        this.loading = false;
        this.items = [];
        this.total = 0;
      },
    });
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.load();
  }

  onRefresh(): void {
    this.keyword = '';
    this.sortCol = null;
    this.isAsc = true;
    this.pageNumber = 1;
    this.load();
  }

  onSortSelect(col: string | null): void {
    if (!col) {
      this.sortCol = null;
      this.isAsc = true;
      this.pageNumber = 1;
      this.load();
      return;
    }
    if (this.sortCol === col) this.isAsc = !this.isAsc;
    else {
      this.sortCol = col;
      this.isAsc = true;
    }
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNumber = 1;
    this.load();
  }

  // selection helpers
  updateCheckedSet(id: number | undefined, checked: boolean): void {
    if (id == null) return;
    if (checked) this.setOfCheckedId.add(id);
    else this.setOfCheckedId.delete(id);
  }

  onItemChecked(id: number | undefined, checked: boolean): void {
    if (id == null) return;
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onCurrentPageDataChange(list: readonly any[]): void {
    this.listOfCurrentPageData = list || [];
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    (this.listOfCurrentPageData || []).forEach(item => this.updateCheckedSet(item.id || item.doiTacId, checked));
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    const list = this.listOfCurrentPageData || [];
    if (!list || list.length === 0) {
      this.checked = false;
      this.indeterminate = false;
      return;
    }
    this.checked = list.every(item => this.setOfCheckedId.has(item.id || item.doiTacId));
    this.indeterminate = list.some(item => this.setOfCheckedId.has(item.id || item.doiTacId)) && !this.checked;
  }


  deleteSelectedMultiple(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} đối tác đã chọn?`,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.api.delete(ids).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã xóa đối tác');
            this.setOfCheckedId.clear();
            this.load();
          },
          error: () => this.notification.error('Lỗi', 'Xóa đối tác thất bại')
        });
      }
    });
  }

  deleteOne(id: number | undefined): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận',
      nzContent: 'Bạn có chắc chắn muốn xóa đối tác này?',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.api.delete([id]).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã xóa đối tác');
            this.load();
          },
          error: () => this.notification.error('Lỗi', 'Xóa đối tác thất bại')
        });
      }
    });
  }

  goCreate() {
    this.openCreateModal();
  }

  openCreateModal(): void {
    this.editingId = undefined;
    this.modalTitle = 'Tạo mới đối tác';
    this.isModalVisible = true;
  }

  edit(item: any): void {
    if (!item) return;
    this.editingId = item.id || item.doiTacId || null;
    this.modalTitle = 'Cập nhật đối tác';
    this.isModalVisible = true;
  }

  openContracts(item: any): void {
    if (!item) return;
    const id = item.id || item.doiTacId || null;
    if (!id) return;
    this.contractLoading = true;
    this.contractModalVisible = true;
    this.contractPartnerName = item.tenDoiTac || item.tenCongTy || '';
    this.contractPartnerId = id;
    this.api.getById(id).subscribe({
      next: (res: any) => {
        const dto = res?.result || {};
        this.contractPartnerDto = dto;
        const rawHopDongs = dto.hopDongs || [];
        const meaningfulHopDongs = this.filterMeaningfulHopDongs(rawHopDongs);

        this.contractList = meaningfulHopDongs.map((h: any) => ({
          ...h,
          ngayKyInput: this.formatDateForInput(h.ngayKy),
          ngayHetHanInput: this.formatDateForInput(h.ngayHetHan),
          giaTriHopDong: h.giaTri ?? h.giaTriHopDong ?? null,
          editing: false,
          teps: h.teps || [],
          tepFileIds: h.tepFileIds || [],
          uploadFiles: [],
        }));
        this.contractLoading = false;
      },
      error: () => {
        this.contractLoading = false;
        this.contractList = [];
        this.notification.error('Lỗi', 'Không thể tải danh sách hợp đồng');
      },
    });
  }

  closeContracts(): void {
    this.contractModalVisible = false;
    this.contractList = [];
    this.contractPartnerName = '';
    this.contractLoading = false;
  }

  deleteContract(hopDongId?: number, index?: number): void {
    if (!this.contractPartnerId) {
      this.notification.warning('Thông báo', 'Đối tác không hợp lệ');
      return;
    }

    if (!hopDongId) {
      if (index != null && index >= 0 && index < this.contractList.length) {
        this.contractList.splice(index, 1);
      } else {
        const idx = this.contractList.findIndex(c => !c.id);
        if (idx >= 0) this.contractList.splice(idx, 1);
      }
      return;
    }

    this.modal.confirm({
      nzTitle: 'Xác nhận',
      nzContent: 'Bạn có chắc chắn muốn xóa hợp đồng này?',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
        nzOnOk: () => {
        this.api.revokeContracts(this.contractPartnerId!, [hopDongId]).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã xóa hợp đồng');
            // Re-fetch everything from server to get updated statuses/dates
            if (this.contractPartnerId) {
               this.openContracts({ id: this.contractPartnerId, tenDoiTac: this.contractPartnerName });
            }
            this.load(); // Also refresh main table
          },
          error: () => this.notification.error('Lỗi', 'Xóa hợp đồng thất bại'),
        });
      }
    });
    return;
  }

  addContract(): void {
    const newC: any = {
      id: null,
      soHopDong: '',
      ngayKyInput: null,
      ngayHetHanInput: null,
      giaTriHopDong: null,
      maDichVu: '',
      tenDichVu: '',
      noiDung: '',
      tepFileIds: [],
      teps: [],
      uploadFiles: [],
      editing: true,
    };
    this.contractList.unshift(newC);
  }



  cancelEditContract(index: number): void {
    const c = this.contractList[index];
    if (!c) return;
    if (!c.id) {
      this.contractList.splice(index, 1);
      return;
    }
    if (this.contractPartnerId) this.openContracts({ id: this.contractPartnerId, tenDoiTac: this.contractPartnerName });
  }

  onContractFilesSelected(event: Event, index: number): void {
    const input = event.target as HTMLInputElement;
    const files = input && input.files ? Array.from(input.files) as File[] : [];
    if (!files || !files.length) return;
    const c = this.contractList[index];
    if (!c) return;
    c.uploadFiles = c.uploadFiles || [];
    c.teps = c.teps || [];
    files.forEach(f => {
      c.uploadFiles.push(f);
      c.teps.push({ originFileObj: f, fileName: f.name, size: f.size });
    });
    try { input.value = ''; } catch (e) { }
  }

  removeContractFile(contractIndex: number, fileIndex: number): void {
    const c = this.contractList[contractIndex];
    if (!c) return;
    const f = c.teps && c.teps[fileIndex];
    if (!f) return;
    if (f.fileId != null) {
      c.tepFileIds = (c.tepFileIds || []).filter((id: any) => id !== f.fileId);
    } else {
      if (c.uploadFiles) {
        const idx = c.uploadFiles.findIndex((uf: any) => uf.name === f.fileName && uf.size === f.size);
        if (idx >= 0) c.uploadFiles.splice(idx, 1);
      }
    }
    c.teps.splice(fileIndex, 1);
  }
  private formatDateForInput(val: any): string | null {
    if (!val) return null;
    const d = new Date(val);
    if (isNaN(d.getTime())) return null;
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }


  private parseBooleanValue(v: any): boolean | undefined {
    if (v === true || v === 'true' || v === 1 || v === '1') return true;
    if (v === false || v === 'false' || v === 0 || v === '0') return false;
    return undefined;
  }

  onTrangThaiHopDongChange(c: any, id: any): void {
    if (!c) return;
    c.trangThaiHopDongId = id;
    const sel = (this.trangThaiHopDongOptions || []).find((s: any) => s && (s.id === id || s.value === id || s.code === id));
    c.trangThaiHopDongTen = sel?.ten || sel?.name || sel?.text || sel?.label || sel?.title || c.trangThaiHopDongTen;
  }

  saveContract(index: number): void {
    const c = this.contractList[index];
    if (!c) return;
    if (!this.contractPartnerId) {
      this.notification.warning('Thông báo', 'Đối tác không hợp lệ');
      return;
    }

    const performSave = () => {
      const hopDong: any = {
        id: c.id || null,
        soHopDong: c.soHopDong || null,
        ngayKy: c.ngayKyInput || null,
        ngayHetHan: c.ngayHetHanInput || null,
        giaTri: c.giaTriHopDong ?? c.giaTri ?? null,
        giaTriHopDong: c.giaTriHopDong ?? c.giaTri ?? null,
        loaiDichVuId: c.loaiDichVuId ?? null,
        tenLoaiDichVu: c.tenLoaiDichVu || c.tenDichVu || null,
        maDichVu: c.maDichVu || null,
        tenDichVu: c.tenDichVu || null,
        donViTinh: c.donViTinh || null,
        trangThaiHopDongId: c.trangThaiHopDongId ?? null,
        tenTrangThaiHopDong: c.trangThaiHopDongTen || c.tenTrangThaiHopDong || null,
        trangThaiDichVuId: c.trangThaiDichVuId ?? null,
        trangThaiDichVuTen: c.trangThaiDichVuTen || null,
        noiDung: c.noiDung || null,
        tepFileIds: c.tepFileIds || [],
        teps: (c.teps && Array.isArray(c.teps) && c.teps.length > 0)
          ? c.teps
          : ((c.tepFileIds && Array.isArray(c.tepFileIds)) ? (c.tepFileIds.map((id: any) => ({ fileId: id }))) : []),
      };

      const parsedIsBatBuoc = this.parseBooleanValue(c.isBatBuoc);
      if (parsedIsBatBuoc !== undefined) hopDong.isBatBuoc = parsedIsBatBuoc;

      this.contractLoading = true;
      this.api.createContract(this.contractPartnerId!, hopDong).subscribe({
        next: (res: any) => {
          if (res && res.result) {
            const dto = res.result || {};
            this.contractPartnerDto = dto;
            this.contractList = (dto.hopDongs || []).map((h: any) => ({
              ...h,
              ngayKyInput: this.formatDateForInput(h.ngayKy),
              ngayHetHanInput: this.formatDateForInput(h.ngayHetHan),
              giaTriHopDong: h.giaTri ?? h.giaTriHopDong ?? null,
              editing: false,
              teps: h.teps || [],
              tepFileIds: h.tepFileIds || [],
              uploadFiles: [],
            }));
          } else if (this.contractPartnerId) {
            this.openContracts({ id: this.contractPartnerId, tenDoiTac: this.contractPartnerName });
          }
          this.notification.success('Thành công', 'Tạo hợp đồng thành công');
          this.contractLoading = false;
        },
        error: (err: any) => {
          this.contractLoading = false;
          this.notification.error('Lỗi', 'Tạo hợp đồng thất bại');
        }
      });
    };

    if (c.uploadFiles && Array.isArray(c.uploadFiles) && c.uploadFiles.length > 0) {
      this.contractLoading = true;
      this.api.uploadMedia(c.uploadFiles, 'tai-lieu-doi-tac').subscribe({
        next: (res: any) => {
          if (res && res.isOk && Array.isArray(res.result)) {
            const uploaded = res.result.map((r: any) => ({
              fileId: r.fileId ?? r.id ?? null,
              fileName: r.fileName ?? r.name ?? (r.fileUrl ? r.fileUrl.split('/').pop() : ''),
              fileUrl: r.fileUrl ?? r.url ?? null,
              contentType: r.contentType ?? r.contentType ?? null,
              size: r.size ?? null
            })).filter((u: any) => u.fileId != null);

            const ids = uploaded.map((u: any) => u.fileId).filter((id: any) => id !== null && id !== undefined);
            c.tepFileIds = Array.from(new Set([...(c.tepFileIds || []), ...(ids || [])]));

            c.teps = c.teps || [];
            uploaded.forEach((u: any) => {
              const matchIdx = c.teps.findIndex((t: any) => t && t.originFileObj && t.fileName === u.fileName && (t.size == null || u.size == null || t.size === u.size));
              if (matchIdx >= 0) {
                c.teps[matchIdx] = { fileId: u.fileId, fileName: u.fileName, fileUrl: u.fileUrl, contentType: u.contentType };
              } else {
                if (!c.teps.some((t: any) => t && (t.fileId === u.fileId))) c.teps.push({ fileId: u.fileId, fileName: u.fileName, fileUrl: u.fileUrl, contentType: u.contentType });
              }
            });

            c.uploadFiles = [];
          }
          performSave();
        },
        error: () => {
          this.contractLoading = false;
          this.notification.error('Lỗi', 'Tải tệp thất bại');
        }
      });
      return;
    }

    // No local files -> proceed directly
    performSave();
  }

  cancelModal(): void {
    this.isModalVisible = false;
    this.saving = false;
  }

  onModalOk(): void {
    this.saving = true;
    this.formComp?.submit();
  }

  onSaved(): void {
    this.saving = false;
    this.isModalVisible = false;
    this.load();
    this.notification.success('Thành công', 'Lưu đối tác thành công');
  }

  private filterMeaningfulHopDongs(rawHopDongs: any[]): any[] {
    if (!Array.isArray(rawHopDongs)) return [];

    return rawHopDongs.filter(h => {
      if (!h) return false;

      if (h.id != null && h.id !== 0) return true;

      const stringKeys = [
        'soHopDong',
        'maDichVu',
        'tenDichVu',
        'noiDung',
        'tenLoaiDichVu',
        'tenTrangThaiHopDong'
      ];

      if (stringKeys.some(key => {
        const v = (h as any)[key];
        return typeof v === 'string' && v.trim() !== '';
      })) {
        return true;
      }

      const teps = (h as any).teps;
      if (Array.isArray(teps) && teps.some((t: any) => {
        if (!t) return false;
        if (t.fileId != null && t.fileId !== 0) return true;
        if (typeof t.fileName === 'string' && t.fileName.trim() !== '') return true;
        if (typeof t.fileUrl === 'string' && t.fileUrl.trim() !== '') return true;
        return false;
      })) {
        return true;
      }

      const tepFileIds = (h as any).tepFileIds;
      if (Array.isArray(tepFileIds) && tepFileIds.some((id: any) => id != null && id !== 0)) {
        return true;
      }

      return false;
    });
  }

  formatterVND = (value: number): string => (value != null ? `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '');
  parserVND = (value: string): string => value.replace(/\$\s?|(,*)/g, '');
}
