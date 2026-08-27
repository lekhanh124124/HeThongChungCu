import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { TriThucApiService } from '../../../../data-access/src/lib/tri-thuc.service';

@Component({
  selector: 'app-tri-thuc-form',
  templateUrl: './tri-thuc-form.component.html',
  styleUrls: ['./tri-thuc-form.component.scss'],
})
export class TriThucFormComponent implements OnInit, OnChanges {
  form = this.fb.group({
    tieuDe: ['', Validators.required],
    noiDung: ['', Validators.required],
    danhMuc: ['', Validators.required],
    thuTuHienThi: [0],
  });

  loading = false;
  danhMucList: string[] = [];
  @Input() id?: number | null;
  @Input() inModal = false;
  @Output() saved = new EventEmitter<void>();

  constructor(
    private fb: FormBuilder,
    private api: TriThucApiService
  ) {}

  ngOnInit(): void {
    this.loadDanhMuc();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.inModal && changes['id'] && this.id) {
      this.load(this.id);
    }
  }

  loadDanhMuc(): void {
    this.api.getDanhMuc().subscribe({
      next: (res: any) => {
        this.danhMucList = Array.isArray(res?.result) ? res.result : [];
      },
      error: () => { this.danhMucList = []; },
    });
  }

  load(id: number): void {
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

  submit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(c => c.markAsDirty());
      return;
    }
    const val = this.form.value;
    this.loading = true;
    const action = this.id ? this.api.update({ ...val, id: this.id }) : this.api.create(val);
    action.subscribe({
      next: () => {
        this.loading = false;
        this.saved.emit();
      },
      error: () => (this.loading = false),
    });
  }
}
