import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ChungCuService } from '@features/resident/data-access';

@Component({
  selector: 'app-tang-form',
  templateUrl: './tang-form.component.html',
  styleUrls: ['./tang-form.component.scss']
})
export class TangFormComponent implements OnInit, OnChanges {
  form: FormGroup;
  @Input() id?: number;
  @Input() inModal = false;
  @Output() saved = new EventEmitter<void>();
  @Output() done = new EventEmitter<void>();

  loaiTangOptions: any[] = [];


  constructor(private fb: FormBuilder, private svc: ChungCuService, private route: ActivatedRoute, private router: Router) {
    this.form = this.fb.group({
      maTang: [''],
      tenTang: ['', Validators.required],
      loaiTangId: [null],
      toaNhaId: [null, Validators.required]
    });
  }

  ngOnInit(): void {
    if (!this.inModal) {
      this.route.params.subscribe((p: any) => {
        if (p['id']) {
          this.id = +p['id'];
          this.svc.getTangById(this.id).subscribe((r: any) => {
            if (r && r.isOk) this.form.patchValue(r.result);
          });
        }
      });
    }
    this.route.queryParams.subscribe((q: any) => {
      if (q && q['toaNhaId']) {
        this.form.patchValue({ toaNhaId: +q['toaNhaId'] });
        this.form.get('toaNhaId')?.disable();
      }
    });
    this.svc.getLoaiTangForSelector().subscribe(r => {
      if (r && r.isOk && Array.isArray(r.result)) {
        this.loaiTangOptions = r.result;
      }
    });

    this.form.get('loaiTangId')?.valueChanges.subscribe(() => this.goiYMaTang());
    this.form.get('toaNhaId')?.valueChanges.subscribe(() => this.goiYMaTang());
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.inModal && changes['id'] && this.id) {
      this.svc.getTangById(this.id).subscribe((r: any) => {
        if (r && r.isOk) this.form.patchValue(r.result);
      });
    }
  }

  goiYMaTang(): void {
    if (this.id) return;
    const fv: any = this.form.getRawValue();
    const toaNhaId = fv.toaNhaId;
    const loaiTangId = fv.loaiTangId;
    if (!toaNhaId || !loaiTangId) return;
    this.svc.goiYMaTang(toaNhaId, loaiTangId).subscribe((r: any) => {
      if (r && r.isOk && r.result) {
        const value = typeof r.result === 'string' ? r.result : (r.result.result || r.result);
        this.form.patchValue({ maTang: value });
      }
    });
  }

  save(): void {
    const fv: any = this.form.getRawValue();
    const payload: any = {
      id: this.id,
      maTang: fv.maTang,
      tenTang: fv.tenTang,
      loaiTangId: fv.loaiTangId,
      toaNhaId: fv.toaNhaId
    };
    const obs = this.id ? this.svc.updateTang(payload) : this.svc.createTang(payload);
    const finish = (success = false) => {
      if (success) {
        if (this.inModal) this.saved.emit();
        else this.router.navigate(['../'], { relativeTo: this.route });
      }
      this.done.emit();
    };

    obs.subscribe(() => finish(true), () => finish(false));
  }
}
