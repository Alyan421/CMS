import { Component, OnInit } from '@angular/core';
import { ImageService } from './image.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // Import FormsModule
import { ClothService } from '../Cloths/cloth.service';
import { ColorService } from '../Colors/color.service';

@Component({
  selector: 'app-image-list',
  templateUrl: './image-list.component.html',
  styleUrls: ['./image-list.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule], // Add FormsModule here
})
export class ImageListComponent implements OnInit {
  images: any[] = [];
  searchQuery: string = '';
  currentPage: number = 1;
  itemsPerPage: number = 10;
  cloths: any[] = []; // List of cloths
  colors: any[] = []; // List of colors
  selectedClothName: string = '';
  selectedColorName: string = '';
  clothImages: any[] = []; // Images of the selected cloth

  constructor(
    private imageService: ImageService,
    private clothService: ClothService,
    private colorService: ColorService
  ) { }

  ngOnInit(): void {
    this.loadImages();
    this.loadCloths();
    this.loadColors();
  }

  loadCloths(): void {
    this.clothService.getAllCloths().subscribe((data) => {
      this.cloths = data;
    });
  }

  loadColors(): void {
    this.colorService.getAllColors().subscribe((data) => {
      this.colors = data.filter(
        (color, index, self) =>
          index === self.findIndex((c) => c.name === color.name)
      );
    });
  }



  filterByClothName(): void {
    this.imageService.filterByClothName(this.selectedClothName).subscribe((data) => {
      this.images = data;
    });
  }

  filterByColorName(colorName: string): void {
    this.imageService.filterByColorName(colorName).subscribe((data) => {
      this.images = data;
      this.currentPage = 1; // Reset to the first page
    });
  }

  loadImages(): void {
    this.imageService.getAllImages().subscribe((data) => {
      this.images = data;
    });
  }

  deleteImage(id: number): void {
    this.imageService.deleteImage(id).subscribe(() => {
      this.loadImages(); // Reload the images after deletion
    });
  }

  get paginatedImages(): any[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.images.slice(start, start + this.itemsPerPage);
  }

  nextPage(): void {
    if (this.currentPage * this.itemsPerPage < this.images.length) {
      this.currentPage++;
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }
}
