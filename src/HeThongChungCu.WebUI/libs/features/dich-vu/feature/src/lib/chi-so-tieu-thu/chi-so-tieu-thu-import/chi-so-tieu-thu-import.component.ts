import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ChiSoTieuThuService } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzUploadFile } from 'ng-zorro-antd/upload';

@Component({
  selector: 'app-chi-so-tieu-thu-import',
  templateUrl: './chi-so-tieu-thu-import.component.html',
  styleUrls: ['./chi-so-tieu-thu-import.component.scss']
})
export class ChiSoTieuThuImportComponent {
  @Input() thang: number | null = null;
  @Input() nam: number | null = null;
  @Output() success = new EventEmitter<void>();

  ngayChot = new Date().toISOString();
  importing = false;
  uploadingImages = false;

  excelFile: File | null = null;
  zipFile: File | null = null;
  excelFileList: NzUploadFile[] = [];
  zipFileList: NzUploadFile[] = [];

  constructor(
    private chiSoService: ChiSoTieuThuService,
    private notification: NzNotificationService
  ) {}

  beforeUploadExcel = (file: NzUploadFile): boolean => {
    this.excelFile = file as any;
    this.excelFileList = [file];
    return false;
  };

  beforeUploadZip = (file: NzUploadFile): boolean => {
    this.zipFile = file as any;
    this.zipFileList = [file];
    return false;
  };

  doImportExcel(): void {
    if (!this.excelFile) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn file Excel');
      return;
    }
    if (!this.thang || !this.nam) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn tháng và năm');
      return;
    }

    this.importing = true;
    this.chiSoService.importExcel(this.excelFile, this.thang, this.nam, this.ngayChot).subscribe({
      next: (res) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã nhập dữ liệu từ Excel');
          this.success.emit();
        }
        this.importing = false;
      },
      error: () => {
        this.importing = false;
        this.notification.error('Lỗi', 'Nhập file Excel thất bại');
      }
    });
  }

  doImportImages(): void {
    if (!this.zipFile) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn file Zip');
      return;
    }

    this.uploadingImages = true;
    this.chiSoService.importImages(this.zipFile).subscribe({
      next: (res) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã tải lên ảnh đồng hồ');
          this.success.emit();
        }
        this.uploadingImages = false;
      },
      error: () => {
        this.uploadingImages = false;
        this.notification.error('Lỗi', 'Tải lên file Zip thất bại');
      }
    });
  }
}
