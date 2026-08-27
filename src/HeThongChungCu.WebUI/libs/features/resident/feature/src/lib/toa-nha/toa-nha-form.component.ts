import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ChungCuService } from '@features/resident/data-access';

@Component({
  selector: 'app-toa-nha-form',
  templateUrl: './toa-nha-form.component.html',
  styleUrls: ['./toa-nha-form.component.scss']
})
export class ToaNhaFormComponent implements OnInit {
  form: FormGroup;
  @Input() id?: number;
  @Input() inModal = false;
  @Output() saved = new EventEmitter<any>();
  @Output() done = new EventEmitter<void>();

  statusOptions: any[] = [];

  constructor(
    private fb: FormBuilder,
    private svc: ChungCuService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      maToaNha: [''],
      tenToaNha: ['', Validators.required],
      soTang: [0, Validators.required],
      soTangHam: [0],
      diaChi: [''],
      moTa: [''],
      trangThaiToaNhaId: [1, Validators.required],
      tenTrangThaiToaNha: ['']
    });
  }

  ngOnInit(): void {
    this.svc.getTrangThaiToaNhaForSelector().subscribe(r => {
      if (r && r.isOk && Array.isArray(r.result)) {
        this.statusOptions = r.result;
      }
    });

    if (!this.inModal) {
      this.route.params.subscribe((p: any) => {
        if (p['id']) {
          this.id = +p['id'];
          this.svc.getToaNhaById(this.id).subscribe((r: any) => {
            if (r.isOk) this.form.patchValue(r.result);
          });
        }
      });
    }

    if (!this.id) {
      const cur = this.form.get('maToaNha')?.value;
      if (!cur) this.goiYMaToaNha();
    }
  }

  goiYMaToaNha(): void {
    const cur = this.form.get('maToaNha')?.value;
    if (cur) return;
    this.svc.goiYMaToaNha().subscribe((r: any) => {
      if (r && r.isOk && r.result) {
        const value = typeof r.result === 'string' ? r.result : (r.result.result || r.result);
        if (!this.form.get('maToaNha')?.value) this.form.patchValue({ maToaNha: value });
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.inModal && changes['id'] && this.id) {
      this.svc.getToaNhaById(this.id).subscribe((r: any) => {
        if (r.isOk) this.form.patchValue(r.result);
      });
    }
  }
  save(): void {
    const payload = { ...(this.form.value as any), id: this.id };
    const finish = (success = false) => {
      if (success) {
        if (this.inModal) this.saved.emit();
        else this.router.navigate(['../'], { relativeTo: this.route });
      }
      this.done.emit();
    };

    if (this.id) {
      this.svc.updateToaNha(payload).subscribe(() => finish(true), () => finish(false));
      return;
    }

    this.svc.createToaNha(payload).subscribe((res: any) => {
      const desired = payload.trangThaiToaNhaId;
      const actual = res?.result?.trangThaiToaNhaId;
      if (typeof desired !== 'undefined' && actual !== desired) {
        const id = res?.result?.id;
        const update = (rec: any) => this.svc.updateToaNha({ ...rec, trangThaiToaNhaId: desired }).subscribe(() => finish(true), () => finish(false));
        if (id > 0) { update(res.result); return; }
        const kw = payload.maToaNha || payload.tenToaNha;
        if (!kw) { finish(false); return; }
        this.svc.getToaNhaList({ keyword: kw, pageNumber: 1, pageSize: 5 }).subscribe((lr: any) => {
          const found = (lr?.result?.items || []).find((it: any) => it.maToaNha === payload.maToaNha || it.tenToaNha === payload.tenToaNha);
          if (found) update(found); else finish(false);
        }, () => finish(false));
      } else finish(true);
    }, () => finish(false));
  }
}
