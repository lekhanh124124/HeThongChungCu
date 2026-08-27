import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-yeu-cau-cu-dan',
  templateUrl: './yeu-cau-cu-dan.component.html',
  styleUrls: ['./yeu-cau-cu-dan.component.scss']
})
export class YeuCauCuDanComponent implements OnInit {
  selectedIndex = 0;
  constructor(private route: ActivatedRoute) {}
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const tab = params['tab'] ?? params['loaiThongBaoId'] ?? null;
      const t = Number(tab);
      if (!isNaN(t)) this.selectedIndex = t;
    });
  }
}
