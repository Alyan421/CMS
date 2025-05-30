import { Component, OnInit } from '@angular/core';
import { ClothService } from './cloth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cloth-list',
  templateUrl: './cloth-list.component.html',
  styleUrls: ['./cloth-list.component.css'],
  standalone: true,
  imports: [CommonModule],
})
export class ClothListComponent implements OnInit {
  cloths: any[] = [];

  constructor(private clothService: ClothService) { }

  ngOnInit(): void {
    this.loadCloths();
  }

  loadCloths(): void {
    this.clothService.getAllCloths().subscribe((data) => {
      this.cloths = data;
    });
  }

  deleteCloth(id: number): void {
    if (confirm('Are you sure you want to delete this cloth?')) {
      this.clothService.deleteCloth(id).subscribe(() => {
        this.loadCloths();
      });
    }
  }
}
