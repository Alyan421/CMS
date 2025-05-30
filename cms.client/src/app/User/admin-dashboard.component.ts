import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css'],
})
export class AdminDashboardComponent implements OnInit {
  totalUsers: number = 0;
  totalImages: number = 0;
  totalColors: number = 0;
  totalCloths: number = 0;

  constructor() { }

  ngOnInit(): void {
    // Fetch statistics from backend (implement API calls here)
    this.totalUsers = 100; // Example data
    this.totalImages = 200;
    this.totalColors = 50;
    this.totalCloths = 30;
  }
}
