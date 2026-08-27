import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-giao-dich-detail',
  templateUrl: './giao-dich-detail.component.html',
  styleUrls: ['./giao-dich-detail.component.scss']
})
export class GiaoDichDetailComponent {
  @Input() item: any;
  @Input() loading = false;

  getLoaiLabel(loaiId: number): string {
    return loaiId === 1 ? 'Phiếu thu' : 'Phiếu chi';
  }

  getLoaiColor(loaiId: number): string {
    return loaiId === 1 ? 'green' : 'red';
  }
}
