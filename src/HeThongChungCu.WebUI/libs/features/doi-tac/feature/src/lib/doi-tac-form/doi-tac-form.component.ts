import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DoiTacApiService } from '../../../../data-access/src/lib/doi-tac.service';

@Component({
  selector: 'app-doi-tac-form',
  templateUrl: './doi-tac-form.component.html',
  styleUrls: ['./doi-tac-form.component.scss'],
})
export class DoiTacCreateComponent implements OnInit, OnChanges {
  form = this.fb.group({
    id: [null],
    tenDoiTac: ['', Validators.required],
    tenCongTy: [''],
    nguoiDaiDien: ['', Validators.required],
    soGiayPhepKD: [''],
    maSoThue: ['', Validators.required],
    diaChi: ['', Validators.required],
    soDienThoai: ['', Validators.required],
    email: [''],
    ghiChu: [''],
  });

  loading = false;
  @Input() doiTacId?: number | null;
  @Input() id?: number | null;
  @Input() inModal = false;
  @Input() modalVisible?: boolean;
  @Input() modalTitle?: string;
  @Output() modalVisibleChange = new EventEmitter<boolean>();
  @Output() saved = new EventEmitter<void>();

  constructor(
    private fb: FormBuilder,
    private api: DoiTacApiService,
    public router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!this.inModal && id) this.load(+id);
  }

  ngOnChanges(changes: SimpleChanges): void {
    const newId = (changes['id'] && this.id) ? this.id : (changes['doiTacId'] && this.doiTacId ? this.doiTacId : undefined);
    if (this.inModal && typeof newId !== 'undefined' && newId !== null) {
      this.load(newId as number);
    }
    if (this.inModal && changes['modalVisible'] && this.modalVisible === false) {
      this.loading = false;
    }
  }

  load(id: number) {
    this.loading = true;
    this.api.getById(id).subscribe({
      next: (res: any) => {
        const dto = res?.result || {};
        this.form.patchValue(dto);
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  submit() {
    if (this.form.invalid) return this.form.markAllAsTouched();
    const val = this.form.value;
    this.loading = true;
    const action = val.id ? this.api.update(val) : this.api.create(val);
    action.subscribe({
      next: () => {
        this.loading = false;
        if (this.inModal) {
          this.saved.emit();
          this.modalVisibleChange.emit(false);
        } else {
          this.router.navigate(['doi-tac']);
        }
      },
      error: () => (this.loading = false),
    });
  }

  cancel() {
    if (this.inModal) this.modalVisibleChange.emit(false);
    else this.router.navigate(['doi-tac']);
  }
}
