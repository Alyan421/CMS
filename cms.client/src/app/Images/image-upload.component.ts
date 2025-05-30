import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ImageService } from './image.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-image-upload',
  templateUrl: './image-upload.component.html',
  styleUrls: ['./image-upload.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class ImageUploadComponent {
  uploadForm: FormGroup;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private imageService: ImageService) {
    // Update the form group to include colorId and isPrimary fields
    this.uploadForm = this.fb.group({
      file: [null, Validators.required],
      colorId: ['', Validators.required],
      isPrimary: [false], // Default value for checkbox
    });
  }

  onFileChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.uploadForm.patchValue({ file }); // Update file in the form
    }
  }

  onSubmit(): void {
    if (this.uploadForm.valid) {
      const formData = new FormData();
      if (this.uploadForm.get('isPrimary')?.value)
        formData.append('IsPrimary', this.uploadForm.get('isPrimary')?.value.toString()); // Append isPrimary
      formData.append('ColorId', this.uploadForm.get('colorId')?.value); // Append colorId
      formData.append('ImageFile', this.uploadForm.get('file')?.value); // Append file

      this.imageService.uploadImage(formData).subscribe({
        next: () => {
          this.successMessage = 'Image uploaded successfully!';
          this.errorMessage = '';
          this.uploadForm.reset(); // Reset the form after successful submission
        },
        error: (err) => {
          this.errorMessage = 'Failed to upload image. Please try again.';
          this.successMessage = '';
          console.error(err);
        },
      });
    } else {
      this.errorMessage = 'Please complete all required fields before uploading.';
    }
  }
}
