import { Component, OnInit } from '@angular/core';
import { ProfileApiService } from '../../../../data-access/src/lib/profile-api.service';
import { AuthService } from '@core/auth/data-access';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  profile: any = null;
  genders: any[] = [];
  loading = false;
  editMode = false;
  profileForm!: FormGroup;
  isEditVisible = false;
  saving = false;
  avatarPreview: string | null = null;
  avatarUrl = '';
  selectedFile: File | null = null;
  editModel: any = {
    email: '',
    phoneNumber: '',
    dob: null,
    idCard: '',
    firstName: '',
    lastName: '',
    diaChi: '',
    gioiTinhId: null
  };

  constructor(
    private api: ProfileApiService,
    private fb: FormBuilder,
    private sanitizer: DomSanitizer,
    private auth: AuthService
  ) {}


  ngOnInit(): void {
    this.load();
    this.loadGenders();
  }

  loadGenders(): void {
    this.api.getGioiTinhForSelector().subscribe({
      next: (r: any) => {
        if (r && r.isOk) {
          this.genders = r.result || [];
        } else if (Array.isArray(r)) {
          this.genders = r || [];
        }
      },
      error: () => {}
    });
  }

  load(): void {
    this.loading = true;
    this.api.getProfile().subscribe({
      next: (r: any) => {
        this.loading = false;
        if (r && r.isOk) {
          this.profile = r.result;
          this.initForm();
          this.auth.setAvatar(this.profile?.anhDaiDienUrl || null);
        }
      },
      error: () => (this.loading = false)
    });
  }

  private initForm(): void {
    this.profileForm = this.fb.group({
      email: [this.profile?.email || '', [Validators.required, Validators.email]],
      firstName: [this.profile?.firstName || '', Validators.required],
      lastName: [this.profile?.lastName || '', Validators.required],
      phoneNumber: [this.profile?.phoneNumber || ''],
      idCard: [this.profile?.idCard || ''],
      dob: [this.profile?.dob || null],
      diaChi: [this.profile?.diaChi || '']
    });
  }

  startEdit(): void {
    this.editMode = true;
    if (!this.profileForm) this.initForm();
    else this.profileForm.patchValue({
      email: this.profile?.email,
      firstName: this.profile?.firstName,
      lastName: this.profile?.lastName,
      phoneNumber: this.profile?.phoneNumber,
      idCard: this.profile?.idCard,
      dob: this.profile?.dob,
      diaChi: this.profile?.diaChi,
      gioiTinhId: this.profile?.gioiTinhId,
    });
    this.editModel = {
      email: this.profile?.email || '',
      phoneNumber: this.profile?.phoneNumber || '',
      dob: this.profile?.dob || null,
      idCard: this.profile?.idCard || '',
      firstName: this.profile?.firstName || '',
      lastName: this.profile?.lastName || '',
      diaChi: this.profile?.diaChi || '',
      gioiTinhId: this.profile?.gioiTinhId ?? null
    };
    this.avatarUrl = this.profile?.anhDaiDienUrl || '';
    this.avatarPreview = null;
    this.isEditVisible = true;
  }

  cancelEdit(): void {
    this.editMode = false;
    this.isEditVisible = false;
  }

  save(): void {
    const payload: any = {
      Email: this.editModel.email,
      firstName: this.editModel.firstName,
      lastName: this.editModel.lastName,
      phoneNumber: this.editModel.phoneNumber,
      idCard: this.editModel.idCard,
      dob: this.editModel.dob,
      diaChi: this.editModel.diaChi,
      gioiTinhId: this.editModel.gioiTinhId ?? this.profile?.gioiTinhId ?? 0,
    };
    this.saving = true;
    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('avatar', this.selectedFile as File, this.selectedFile!.name);
      this.api.changeAvatar(formData).subscribe({
        next: (ar: any) => {
            if (ar && ar.isOk) {
            const newUrl = ar.result?.anhDaiDienUrl || ar.result?.url || null;
            if (newUrl) {
              this.avatarUrl = newUrl;
              this.profile = { ...this.profile, anhDaiDienUrl: newUrl };
            } else if (this.avatarPreview) {
              this.profile = { ...this.profile, anhDaiDienUrl: this.avatarPreview };
            }
            this.auth.setAvatar(this.profile?.anhDaiDienUrl || null);
          }
          this.selectedFile = null;
          this.api.updateProfile(payload).subscribe({
            next: (r: any) => {
              this.saving = false;
              if (r && r.isOk) {
                this.profile = r.result || { ...this.profile, ...payload };
                this.auth.setAvatar(this.profile?.anhDaiDienUrl || null);
                this.isEditVisible = false;
                this.editMode = false;
              }
            },
            error: () => (this.saving = false)
          });
        },
        error: () => {
          this.selectedFile = null;
          this.api.updateProfile(payload).subscribe({
            next: (r: any) => {
              this.saving = false;
              if (r && r.isOk) {
                this.profile = r.result || { ...this.profile, ...payload };
                this.auth.setAvatar(this.profile?.anhDaiDienUrl || null);
                this.isEditVisible = false;
                this.editMode = false;
              }
            },
            error: () => (this.saving = false)
          });
        }
      });
    } else {
      this.api.updateProfile(payload).subscribe({
        next: (r: any) => {
          this.saving = false;
          if (r && r.isOk) {
            this.profile = r.result || { ...this.profile, ...payload };
            this.auth.setAvatar(this.profile?.anhDaiDienUrl || null);
            this.isEditVisible = false;
            this.editMode = false;
          }
        },
        error: () => (this.saving = false)
      });
    }
  }

  onFileChange(ev: any): void {
    const file: File = ev.target.files && ev.target.files[0];
    if (!file) return;
    this.selectedFile = file;
    const reader = new FileReader();
    reader.onload = () => {
      this.avatarPreview = String(reader.result);
    };
    reader.readAsDataURL(file);
  }

}
