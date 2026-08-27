import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoiTacListComponent } from './doi-tac-list/doi-tac-list.component';
import { DoiTacCreateComponent } from './doi-tac-form/doi-tac-form.component';

const routes: Routes = [
  { path: '', component: DoiTacListComponent },
  { path: 'create', component: DoiTacCreateComponent },
  { path: ':id/edit', component: DoiTacCreateComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DoiTacRoutingModule {}
