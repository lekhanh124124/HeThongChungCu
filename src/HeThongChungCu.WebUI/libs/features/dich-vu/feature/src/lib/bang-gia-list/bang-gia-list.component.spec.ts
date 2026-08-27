import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BangGiaListComponent } from './bang-gia-list.component';

describe('BangGiaListComponent', () => {
  let component: BangGiaListComponent;
  let fixture: ComponentFixture<BangGiaListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BangGiaListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BangGiaListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
