import { Component, OnInit } from '@angular/core';
import { ColorService } from './color.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-color-list',
  templateUrl: './color-list.component.html',
  styleUrls: ['./color-list.component.css'],
  standalone: true,
  imports: [CommonModule],
})
export class ColorListComponent implements OnInit {
  colors: any[] = [];

  constructor(private colorService: ColorService) { }

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors(): void {
    this.colorService.getAllColors().subscribe((data) => {
      this.colors = data;
    });
  }

  deleteColor(id: number): void {
    if (confirm('Are you sure you want to delete this color?')) {
      this.colorService.deleteColor(id).subscribe(() => {
        this.loadColors();
      });
    }
  }
}
