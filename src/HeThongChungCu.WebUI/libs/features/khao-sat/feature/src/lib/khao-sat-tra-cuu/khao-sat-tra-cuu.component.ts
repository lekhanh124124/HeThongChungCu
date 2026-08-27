import { Component, OnInit } from '@angular/core';
import { ChungCuService, CanHo } from '@features/resident/data-access';
import { NzModalRef } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-khao-sat-tra-cuu',
  templateUrl: './khao-sat-tra-cuu.component.html',
  styleUrls: ['./khao-sat-tra-cuu.component.scss']
})
export class KhaoSatTraCuuComponent implements OnInit {
  canHoOptions: Array<{ id: number; label: string }> = [];
  selectedCanHoId: number | null = null;
  loading = false;

  constructor(
    private chungCuService: ChungCuService,
    private modalRef: NzModalRef
  ) {}

  ngOnInit(): void {
    this.loading = true;
    this.chungCuService.getCanHoList({ pageSize: 9999, sortCol: 'maCanHo', isAsc: true }).subscribe({
      next: (res: any) => {
        const items: CanHo[] = res.result?.items || [];
        this.canHoOptions = items.map(ch => ({
          id: ch.id!,
          label: `[${ch.maCanHo}] ${ch.tenCanHo || ''}${ch.tenTang ? ' - ' + ch.tenTang : ''}`.trim()
        }));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  traCuu(): void {
    if (this.selectedCanHoId) {
      this.modalRef.close(this.selectedCanHoId);
    }
  }

  cancel(): void {
    this.modalRef.close();
  }
}
