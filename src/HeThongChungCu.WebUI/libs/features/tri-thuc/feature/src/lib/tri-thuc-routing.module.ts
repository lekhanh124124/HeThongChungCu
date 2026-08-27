import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TriThucListComponent } from './tri-thuc-list/tri-thuc-list.component';

const routes: Routes = [
  { path: '', component: TriThucListComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TriThucRoutingModule {}
