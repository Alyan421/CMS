import { Component, OnInit } from '@angular/core';
import { ColorService } from './color.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-color-list',
  templateUrl: './color-list.component.html',
  styleUrls: ['./color-list.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class ColorListComponent implements OnInit {
  colors: any[] = [];
  selectedColor: any = null;
  colorForm: FormGroup;
  filteredColors: any[] = [];
  searchTerm: string = '';

  constructor(private colorService: ColorService, private fb: FormBuilder) {
    // Initialize the form with the correct order of properties
    this.colorForm = this.fb.group({
      id: [null],
      clothId: [''],
      colorName: [''],
      availiableStock: ['']
    });
  }

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors(): void {
    this.colorService.getAllColors().subscribe((data) => {
      this.colors = data;
      this.filteredColors = data; // Initialize filtered list
    });
  }

  getColorById(id: number): void {
    this.colorService.getColorById(id).subscribe(data => {
      this.selectedColor = data;
      // Make sure the form is updated with the correct order
      this.colorForm.patchValue({
        id: data.id,
        clothId: data.clothId,
        colorName: data.colorName,
        availiableStock: data.availiableStock
      });
    });
  }

  addColor(): void {
    // Create a properly ordered object for API submission
    const colorData = {
      clothId: this.colorForm.value.clothId,
      colorName: this.colorForm.value.colorName,
      availiableStock: this.colorForm.value.availiableStock
    };

    this.colorService.createColor(colorData).subscribe(() => {
      this.loadColors();
      this.colorForm.reset();
    });
  }

  updateColor(): void {
    const id = this.colorForm.value.id;
    // Create a properly ordered object for API submission
    const colorData = {
      id: id,
      clothId: this.colorForm.value.clothId,
      colorName: this.colorForm.value.colorName,
      availiableStock: this.colorForm.value.availiableStock
    };

    this.colorService.updateColor(colorData).subscribe(() => {
      this.loadColors();
      this.colorForm.reset();
    });
  }

  editColor(color: any): void {
    this.selectedColor = color;
    // Make sure the form is updated with the correct order
    this.colorForm.patchValue({
      id: color.id,
      clothId: color.clothId,
      colorName: color.colorName,
      availiableStock: color.availiableStock
    });
  }

  deleteColor(id: number): void {
    if (confirm('Are you sure you want to delete this color?')) {
      this.colorService.deleteColor(id).subscribe(() => {
        this.loadColors();
      });
    }
  }

  searchColors(event: any): void {
    this.searchTerm = event.target.value.toLowerCase();
    if (this.searchTerm) {
      this.filteredColors = this.colors.filter(color =>
        color.colorName.toLowerCase().includes(this.searchTerm)
      );
    } else {
      this.filteredColors = this.colors;
    }
  }
}
