import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KhungGioListComponent } from './khung-gio-list.component';

describe('KhungGioListComponent', () => {
  let component: KhungGioListComponent;
  let fixture: ComponentFixture<KhungGioListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ KhungGioListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(KhungGioListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
