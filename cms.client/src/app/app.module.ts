import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FooterComponent } from './shared/footer.component';
import { HeaderComponent } from './shared/header.component'; // Import HeaderComponent
import { RouterModule } from '@angular/router'; // Import RouterModule for router-outlet
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './Authorization/auth.interceptor';

@NgModule({
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    RouterModule // Keep RouterModule here
  ],
  declarations: [
    HeaderComponent, // Declare HeaderComponent first
    FooterComponent, // Declare FooterComponent next
    AppComponent,    // Declare AppComponent last
  ],
  providers: [provideHttpClient(), { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }],
  bootstrap: [AppComponent]
})
export class AppModule { }
