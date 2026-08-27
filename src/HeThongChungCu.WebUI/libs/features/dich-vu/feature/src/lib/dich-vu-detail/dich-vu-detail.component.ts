import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { DichVuService } from '@features/dich-vu/data-access';
import { BangGiaListComponent } from '../bang-gia-list/bang-gia-list.component';

@Component({
  selector: 'app-dich-vu-detail',
  templateUrl: './dich-vu-detail.component.html',
  styleUrls: ['./dich-vu-detail.component.scss']
})
export class DichVuDetailComponent implements OnInit, OnChanges, AfterViewInit {
  @Input() inModal = false;
  @Input() itemId?: number | string | null;
  @Output() closed = new EventEmitter<void>();
  @Output() changed = new EventEmitter<void>();

  id: number | null = null;
  loading = false;
  model: any = null;
  activeTabIndex = 0;
  @ViewChild(BangGiaListComponent) bangGiaList?: BangGiaListComponent;

  constructor(private dichVuService: DichVuService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    if (!this.inModal && this.itemId) {
      this.id = Number(this.itemId);
      this.load();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.itemId) {
      const val = changes.itemId.currentValue;
      this.id = val != null && val !== '' ? Number(val) : null;
      if (this.id) this.load();
    }
  }

  load(): void {
    if (!this.id) return;
    this.loading = true;
    this.dichVuService.getDichVuById(Number(this.id)).subscribe({
      next: (res: any) => { this.model = res?.result ?? res?.data ?? res; this.loading = false; },
      error: () => (this.loading = false)
    });
  }

  onClose(): void { this.closed.emit(); }

  notifyChanged(): void { this.changed.emit(); this.load(); }

  onBangGiaPageChange(page: number): void {
    if (!this.bangGiaList) return;
    this.bangGiaList.pageNumber = page;
    this.bangGiaList.load();
  }

  onBangGiaPageSizeChange(size: number): void {
    if (!this.bangGiaList) return;
    this.bangGiaList.pageSize = size;
    this.bangGiaList.pageNumber = 1;
    this.bangGiaList.load();
  }

  ngAfterViewInit(): void {
    this.cdr.detectChanges();
  }
}
