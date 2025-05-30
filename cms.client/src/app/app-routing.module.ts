import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './Authorization/login.component';
import { RegisterComponent } from './Authorization/register.component';
import { ImageUploadComponent } from './Images/image-upload.component';
import { ClothListComponent } from './Cloths/cloth-list.component';
import { ColorListComponent } from './Colors/color-list.component';
import { ImageListComponent } from './Images/image-list.component';
import { NotFoundComponent } from './shared/not-found.component';
import { AdminDashboardComponent } from './User/admin-dashboard.component';
import { AuthGuard } from './Authorization/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent},
  { path: 'register', component: RegisterComponent },
  { path: 'upload-image', component: ImageUploadComponent, canActivate: [AuthGuard] },
  { path: 'cloths', component: ClothListComponent, canActivate: [AuthGuard] },
  { path: 'colors', component: ColorListComponent, canActivate: [AuthGuard] },
  { path: 'images', component: ImageListComponent, canActivate: [AuthGuard] },
  { path: 'admin-dashboard', component: AdminDashboardComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
