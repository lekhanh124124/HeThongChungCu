import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BangGiaFormComponent } from './bang-gia-form.component';

describe('BangGiaFormComponent', () => {
  let component: BangGiaFormComponent;
  let fixture: ComponentFixture<BangGiaFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BangGiaFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BangGiaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
