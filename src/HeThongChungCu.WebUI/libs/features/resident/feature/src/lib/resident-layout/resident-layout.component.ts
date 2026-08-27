import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-resident-layout',
  templateUrl: './resident-layout.component.html',
  styleUrls: ['./resident-layout.component.scss']
})
export class ResidentLayoutComponent implements OnInit, OnDestroy {
  showTree = true;
  private sub?: Subscription;

  constructor(private router: Router) {}

  ngOnInit(): void {
    const update = (url: string) => {
      this.showTree = !url.includes('/resident/nhan-vien');
    };

    update(this.router.url);
    this.sub = this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe((e: any) => {
      update(e.urlAfterRedirects || e.url);
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
