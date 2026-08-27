import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ChungCuService } from '@features/resident/data-access';

@Component({
  selector: 'app-can-ho-form',
  templateUrl: './can-ho-form.component.html',
  styleUrls: ['./can-ho-form.component.scss']
})
export class CanHoFormComponent implements OnInit, OnChanges {
  form: FormGroup;
  @Input() id?: number;
  @Input() inModal = false;
  @Input() initialTangId?: number;
  @Output() saved = new EventEmitter<void>();
  @Output() done = new EventEmitter<void>();

  constructor(private fb: FormBuilder, private svc: ChungCuService, private route: ActivatedRoute, private router: Router) {
    this.form = this.fb.group({
      tenCanHo: ['', Validators.required],
      maCanHo: [''],
      dienTich: [0],
      tangId: [null],
      soPhongNgu: [null],
      soPhongTam: [null],
      loaiCanHoId: [null],
      tinhTrangCanHoId: [null]
    });
  }

  ngOnInit(): void {
    if (!this.inModal) {
      this.route.params.subscribe((p: any) => {
        if (p['id']) {
          this.id = +p['id'];
          this.svc.getCanHoById(this.id).subscribe((r: any) => {
            if (r && r.isOk) this.form.patchValue(r.result);
          });
        }
      });
    }
    if (this.initialTangId) this.form.patchValue({ tangId: this.initialTangId });
    this.form.get('tangId')?.disable();

    this.form.get('tangId')?.valueChanges.subscribe((v: any) => {
      if (this.id) return;
      if (!v) return;
      const cur = this.form.get('maCanHo')?.value;
      if (cur) return;
      this.svc.goiYMaCanHo(v).subscribe((r: any) => {
        if (r && r.isOk && r.result) {
          const value = typeof r.result === 'string' ? r.result : (r.result.result || r.result);
          if (!this.form.get('maCanHo')?.value) this.form.patchValue({ maCanHo: value });
        }
      });
    });

    if (!this.id && this.initialTangId) {
      const cur = this.form.get('maCanHo')?.value;
      if (!cur) this.svc.goiYMaCanHo(this.initialTangId).subscribe((r: any) => {
        if (r && r.isOk && r.result) {
          const value = typeof r.result === 'string' ? r.result : (r.result.result || r.result);
          if (!this.form.get('maCanHo')?.value) this.form.patchValue({ maCanHo: value });
        }
      });
    }
    this.loadTinhTrangOptions();
    this.loadLoaiCanHoOptions();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.inModal && changes['id'] && this.id) {
      this.svc.getCanHoById(this.id).subscribe((r: any) => {
        if (r && r.isOk) {
          const val: any = { ...r.result, tangId: r.result.tangId ?? r.result.tang };
          this.form.patchValue(val);
        }
      });
    }
    if (this.inModal && !this.id) {
      if (changes['initialTangId'] && this.initialTangId) this.form.patchValue({ tangId: this.initialTangId });
    }
  }

  goiYMaCanHo(): void {
    const fv: any = this.form.getRawValue();
    const tangId = fv.tangId;
    if (!tangId) return;
    const cur = this.form.get('maCanHo')?.value;
    if (cur) return;
    this.svc.goiYMaCanHo(tangId).subscribe((r: any) => {
      if (r && r.isOk && r.result) {
        const value = typeof r.result === 'string' ? r.result : (r.result.result || r.result);
        if (!this.form.get('maCanHo')?.value) this.form.patchValue({ maCanHo: value });
      }
    });
  }

  save(): void {
    const fv: any = this.form.getRawValue();
    const payload: any = { id: this.id, ...fv };

    const finish = (success = false) => {
      if (success) {
        if (this.inModal) this.saved.emit();
        else this.router.navigate(['../'], { relativeTo: this.route });
      }
      this.done.emit();
    };

    if (this.id) {
      this.svc.updateCanHo(payload).subscribe(() => finish(true), () => finish(false));
      return;
    }

    this.svc.createCanHo(payload).subscribe((res: any) => {
      const desired = payload.tinhTrangCanHoId;
      const actual = res?.result?.tinhTrangCanHoId;
      if (typeof desired !== 'undefined' && actual !== desired) {
        const id = res?.result?.id;
        const update = (rec: any) => this.svc.updateCanHo({ ...rec, tinhTrangCanHoId: desired }).subscribe(() => finish(true), () => finish(false));
        if (id > 0) { update(res.result); return; }
        const kw = payload.maCanHo || payload.tenCanHo;
        if (!kw) { finish(false); return; }
        this.svc.getCanHoList({ keyword: kw, pageNumber: 1, pageSize: 5 }).subscribe((lr: any) => {
          const found = (lr?.result?.items || []).find((it: any) => it.maCanHo === payload.maCanHo || it.tenCanHo === payload.tenCanHo);
          if (found) update(found); else finish(false);
        }, () => finish(false));
      } else finish(true);
    }, () => finish(false));
  }

  tinhTrangOptions: any[] = [];
  loaiCanHoOptions: any[] = [];
  private loadTinhTrangOptions(): void {
    this.svc.getTinhTrangCanHoForSelector().subscribe(r => {
      if (r && r.isOk && Array.isArray(r.result)) this.tinhTrangOptions = r.result;
    });
  }

  private loadLoaiCanHoOptions(): void {
    this.svc.getLoaiCanHoForSelector().subscribe(r => {
      if (r && r.isOk && Array.isArray(r.result)) this.loaiCanHoOptions = r.result;
    });
  }
}
