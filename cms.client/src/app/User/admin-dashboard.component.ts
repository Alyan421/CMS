import { Component, OnInit } from '@angular/core';
import { UserService } from './user.service';
import { ImageService } from '../Images/image.service';
import { ColorService } from '../Colors/color.service';
import { ClothService } from '../Cloths/cloth.service';
import { forkJoin, catchError, of } from 'rxjs';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../Authorization/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-component.dashboard.css'],
  standalone: true,
  imports: [RouterModule, CommonModule],
})
export class AdminDashboardComponent implements OnInit {
  totalUsers: number = 0;
  totalImages: number = 0;
  totalColors: number = 0;
  totalCloths: number = 0;
  isLoading: boolean = true;
  error: string | null = null;

  constructor(
    private userService: UserService,
    private imageService: ImageService,
    private colorService: ColorService,
    private clothService: ClothService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;
    this.error = null;

    // Load all statistics in parallel with error handling for each
    forkJoin({
      users: this.userService.getAllUsers().pipe(catchError(() => of([]))),
      images: this.imageService.getAllImages().pipe(catchError(() => of([]))),
      colors: this.colorService.getAllColors().pipe(catchError(() => of([]))),
      cloths: this.clothService.getAllCloths().pipe(catchError(() => of([])))
    }).subscribe({
      next: (results) => {
        this.totalUsers = results.users.length;
        this.totalImages = results.images.length;
        this.totalColors = results.colors.length;
        this.totalCloths = results.cloths.length;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading dashboard data:', err);
        this.error = 'Failed to load dashboard data. Please try again later.';
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  refreshData(): void {
    this.loadDashboardData();
  }
}
