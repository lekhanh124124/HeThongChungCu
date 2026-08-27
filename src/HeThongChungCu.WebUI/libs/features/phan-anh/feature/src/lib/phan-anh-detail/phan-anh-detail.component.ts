import { Component, OnInit, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { PhanAnhService, PhanAnhDetailResponse } from '@features/phan-anh/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-phan-anh-detail',
  templateUrl: './phan-anh-detail.component.html',
  styleUrls: ['./phan-anh-detail.component.scss']
})
export class PhanAnhDetailComponent implements OnInit, AfterViewChecked {
  @ViewChild('scrollMe') private myScrollContainer!: ElementRef;

  id!: number;
  loading = false;
  data: PhanAnhDetailResponse | null = null;
  
  replyContent = '';
  submittingReply = false;
  
  constructor(
    private phanAnhService: PhanAnhService,
    private notification: NzNotificationService,
    private modal: NzModalService
  ) {}

  ngOnInit(): void {
    if (this.id) {
      this.load();
    }
  }

  ngAfterViewChecked() {        
    this.scrollToBottom();        
  } 

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch(err) { }                 
  }

  load(): void {
    this.loading = true;
    this.phanAnhService.getById(this.id).subscribe({
      next: (res: ApiResponse<PhanAnhDetailResponse>) => {
        if (res.isOk && res.result) {
          this.data = res.result;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải chi tiết phản ánh');
      }
    });
  }

  tiepNhan(): void {
    this.loading = true;
    this.phanAnhService.tiepNhan(this.id).subscribe((res: any) => {
      if (res.isOk) {
        this.notification.success('Thành công', 'Đã tiếp nhận phản ánh');
        this.load();
      } else {
        this.loading = false;
      }
    });
  }

  sendReply(): void {
    if (!this.replyContent.trim()) return;
    
    this.submittingReply = true;
    this.phanAnhService.submitTraLoi(this.id, this.replyContent).subscribe((res: any) => {
      if (res.isOk) {
        this.replyContent = '';
        this.load();
      }
      this.submittingReply = false;
    });
  }

  xacNhanHoanThanh(): void {
    this.modal.confirm({
      nzTitle: 'Xác nhận hoàn thành',
      nzContent: 'Báo cáo kết quả xử lý và chuyển trạng thái sang Chờ đánh giá?',
      nzOnOk: () => {
        // Mock prompt for result description
        const ketQua = prompt('Vui lòng nhập tóm tắt kết quả xử lý:', 'Đã xử lý xong hỏng hóc thực tế.');
        if (ketQua) {
          this.phanAnhService.xacNhanHoanThanh(this.id, ketQua).subscribe((res: any) => {
            if (res.isOk) {
              this.notification.success('Thành công', 'Đã báo cáo hoàn thành');
              this.load();
            }
          });
        }
      }
    });
  }

  huy(): void {
    this.modal.confirm({
      nzTitle: 'Hủy phản ánh',
      nzContent: 'Bạn có chắc chắn muốn hủy phản ánh này? Vui lòng nhập lý do.',
      nzOkDanger: true,
      nzOnOk: () => {
        const lyDo = prompt('Lý do hủy:');
        if (lyDo) {
          this.phanAnhService.huy(this.id, lyDo).subscribe((res: any) => {
            if (res.isOk) {
              this.notification.success('Thành công', 'Đã hủy phản ánh');
              this.load();
            }
          });
        }
      }
    });
  }
}
