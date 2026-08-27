import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTreeViewModule } from 'ng-zorro-antd/tree-view';
import { CauTrucChungCuTreeComponent } from './cau-truc-chung-cu-tree.component';

@NgModule({
  declarations: [CauTrucChungCuTreeComponent],
  imports: [
    CommonModule,
    FormsModule,
    NzInputModule,
    NzButtonModule,
    NzIconModule,
    NzTreeViewModule
  ],
  exports: [CauTrucChungCuTreeComponent]
})
export class CauTrucChungCuTreeModule {}
