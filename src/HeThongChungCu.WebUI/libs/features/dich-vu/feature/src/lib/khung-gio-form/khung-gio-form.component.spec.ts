import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KhungGioFormComponent } from './khung-gio-form.component';

describe('KhungGioFormComponent', () => {
  let component: KhungGioFormComponent;
  let fixture: ComponentFixture<KhungGioFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ KhungGioFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(KhungGioFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
