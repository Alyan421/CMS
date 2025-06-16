import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ImageService } from './image.service';
import { CommonModule } from '@angular/common';
import { ColorService } from '../Colors/color.service';
import { ClothService } from '../Cloths/cloth.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-image-upload',
  templateUrl: './image-upload.component.html',
  styleUrls: ['./image-upload.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class ImageUploadComponent implements OnInit {
  uploadForm: FormGroup;
  successMessage: string = '';
  errorMessage: string = '';
  imagePreview: string | null = null;
  isAddingNewClothColor: boolean = false;
  colors: any[] = [];
  cloths: any[] = [];
  clothMap: Map<number, string> = new Map();
  isUploading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private imageService: ImageService,
    private colorService: ColorService,
    private clothService: ClothService
  ) {
    this.uploadForm = this.fb.group({
      file: [null, Validators.required],
      colorId: ['', Validators.required],
      // New cloth fields
      clothName: [''],
      clothPrice: [''],
      clothDescription: [''],
      // New color fields
      colorName: [''],
      availableStock: ['']
    });
  }

  ngOnInit(): void {
    this.loadColorsAndCloths();
  }

  loadColorsAndCloths(): void {
    forkJoin({
      colors: this.colorService.getAllColors(),
      cloths: this.clothService.getAllCloths()
    }).subscribe({
      next: (result) => {
        this.colors = result.colors;
        this.cloths = result.cloths;

        // Create a map of cloth IDs to names for easy lookup
        this.cloths.forEach(cloth => {
          this.clothMap.set(cloth.id, cloth.name);
        });
      },
      error: (err) => {
        console.error('Error loading data:', err);
        this.errorMessage = 'Failed to load colors and cloths data.';
      }
    });
  }

  getClothName(clothId: number): string {
    return this.clothMap.get(clothId) || 'Unknown';
  }

  toggleNewClothColor(): void {
    this.isAddingNewClothColor = !this.isAddingNewClothColor;

    if (this.isAddingNewClothColor) {
      // Clear the colorId when switching to new cloth/color mode
      this.uploadForm.get('colorId')?.setValue('');

      // Add validators for new cloth and color fields
      this.uploadForm.get('clothName')?.setValidators([Validators.required]);
      this.uploadForm.get('clothPrice')?.setValidators([Validators.required, Validators.min(0)]);
      this.uploadForm.get('clothDescription')?.setValidators([Validators.required]);
      this.uploadForm.get('colorName')?.setValidators([Validators.required]);
      this.uploadForm.get('availableStock')?.setValidators([Validators.required, Validators.min(0)]);

      // Remove validator from colorId
      this.uploadForm.get('colorId')?.clearValidators();
    } else {
      // Add validators back to colorId
      this.uploadForm.get('colorId')?.setValidators([Validators.required]);

      // Remove validators from new cloth and color fields
      this.uploadForm.get('clothName')?.clearValidators();
      this.uploadForm.get('clothPrice')?.clearValidators();
      this.uploadForm.get('clothDescription')?.clearValidators();
      this.uploadForm.get('colorName')?.clearValidators();
      this.uploadForm.get('availableStock')?.clearValidators();
    }

    // Update validity
    this.uploadForm.get('colorId')?.updateValueAndValidity();
    this.uploadForm.get('clothName')?.updateValueAndValidity();
    this.uploadForm.get('clothPrice')?.updateValueAndValidity();
    this.uploadForm.get('clothDescription')?.updateValueAndValidity();
    this.uploadForm.get('colorName')?.updateValueAndValidity();
    this.uploadForm.get('availableStock')?.updateValueAndValidity();
  }

  onFileChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.uploadForm.patchValue({ file });

      // Create preview
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  isFormValid(): boolean {
    if (this.isUploading) return false;

    if (!this.uploadForm.get('file')?.value) return false;

    if (this.isAddingNewClothColor) {
      // Fix for the type error - use more careful null checking
      const clothNameValid = !!this.uploadForm.get('clothName')?.valid;
      const clothPriceValid = !!this.uploadForm.get('clothPrice')?.valid;
      const clothDescriptionValid = !!this.uploadForm.get('clothDescription')?.valid;
      const colorNameValid = !!this.uploadForm.get('colorName')?.valid;
      const availableStockValid = !!this.uploadForm.get('availableStock')?.valid;

      return clothNameValid && clothPriceValid && clothDescriptionValid &&
        colorNameValid && availableStockValid;
    } else {
      // Use !! to convert to boolean to avoid undefined
      return !!this.uploadForm.get('colorId')?.valid;
    }
  }

  onSubmit(): void {
    if (!this.isFormValid()) {
      this.errorMessage = 'Please complete all required fields before uploading.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isAddingNewClothColor) {
      // First create the cloth
      const clothData = {
        name: this.uploadForm.get('clothName')?.value,
        price: parseFloat(this.uploadForm.get('clothPrice')?.value), // Ensure price is a number
        description: this.uploadForm.get('clothDescription')?.value
      };


      this.clothService.createCloth(clothData).subscribe({
        next: (newCloth) => {

          // Check if newCloth and newCloth.id exist
          if (!newCloth || !newCloth.id) {
            this.isUploading = false;
            this.errorMessage = 'Error: Cloth was created but returned invalid data. Missing cloth ID.';
            return;
          }

          // Then create the color with the new cloth ID
          const colorData = {
            colorName: this.uploadForm.get('colorName')?.value,
            availiableStock: parseInt(this.uploadForm.get('availableStock')?.value), // Note the spelling: "availiableStock" not "availableStock"
            clothId: newCloth.id
          };


          this.colorService.createColor(colorData).subscribe({
            next: (newColor) => {

              // Check if newColor and newColor.id exist
              if (!newColor || !newColor.id) {
                this.isUploading = false;
                this.errorMessage = 'Error: Color was created but returned invalid data. Missing color ID.';
                return;
              }

              // Finally upload the image with the new color ID
              this.uploadImageWithColorId(newColor.id);
            },
            error: (err) => {
              this.isUploading = false;
              this.errorMessage = `Failed to create color: ${err.error?.message || err.statusText || JSON.stringify(err) || 'Unknown error'}`;
            }
          });
        },
        error: (err) => {
          this.isUploading = false;
          this.errorMessage = `Failed to create cloth: ${err.error?.message || err.statusText || JSON.stringify(err) || 'Unknown error'}`;
        }
      });
    } else {
      // Use existing color ID
      const colorId = this.uploadForm.get('colorId')?.value;
      if (!colorId) {
        this.isUploading = false;
        this.errorMessage = 'Please select a color before uploading.';
        return;
      }
      this.uploadImageWithColorId(colorId);
    }
  }


  uploadImageWithColorId(colorId: number): void {
    if (!colorId) {
      this.isUploading = false;
      this.errorMessage = 'Error: Missing color ID for image upload.';
      return;
    }

    const formData = new FormData();
    formData.append('ColorId', colorId.toString());
    formData.append('ImageFile', this.uploadForm.get('file')?.value);


    this.imageService.uploadImage(formData).subscribe({
      next: (response) => {
        this.isUploading = false;
        this.successMessage = 'Image uploaded successfully!';
        this.resetForm();
        // Refresh the lists
        this.loadColorsAndCloths();
      },
      error: (err) => {
        this.isUploading = false;
        this.errorMessage = `Failed to upload image: ${err.error?.message || err.statusText || JSON.stringify(err) || 'Unknown error'}`;
      }
    });
  }



  resetForm(): void {
    this.uploadForm.reset();
    this.imagePreview = null;
    this.isAddingNewClothColor = false;

    // Reset validators
    this.uploadForm.get('colorId')?.setValidators([Validators.required]);
    this.uploadForm.get('clothName')?.clearValidators();
    this.uploadForm.get('clothPrice')?.clearValidators();
    this.uploadForm.get('clothDescription')?.clearValidators();
    this.uploadForm.get('colorName')?.clearValidators();
    this.uploadForm.get('availableStock')?.clearValidators();

    // Update validity
    this.uploadForm.get('colorId')?.updateValueAndValidity();
    this.uploadForm.get('clothName')?.updateValueAndValidity();
    this.uploadForm.get('clothPrice')?.updateValueAndValidity();
    this.uploadForm.get('clothDescription')?.updateValueAndValidity();
    this.uploadForm.get('colorName')?.updateValueAndValidity();
    this.uploadForm.get('availableStock')?.updateValueAndValidity();
  }
}

