import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../User/user.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], // Ensure ReactiveFormsModule is imported
})
export class RegisterComponent {
  registerForm: FormGroup;
  registrationSuccess: boolean = false;
  errorMessage: string = '';

  // Constructor for initializing FormBuilder and injecting UserService
  constructor(private fb: FormBuilder, private userService: UserService) {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]], // Username validation
      email: ['', [Validators.required, Validators.email]], // Email validation
      password: ['', [Validators.required, Validators.minLength(6)]], // Password validation
    });
  }

  // Method to handle form submission
  register(): void {
    if (this.registerForm.valid) {
      // Call the UserService's registerUser method
      this.userService.registerUser(this.registerForm.value).subscribe({
        next: () => {
          this.registrationSuccess = true; // Display success message
          this.errorMessage = '';
          this.registerForm.reset(); // Reset the form after successful registration
        },
        error: (err) => {
          this.registrationSuccess = false;
          this.errorMessage =
            err.error?.message || 'Registration failed. Please try again.'; // Handle error
        },
      });
    } else {
      this.errorMessage = 'Please fill out the form correctly.'; // Validation error message
    }
  }
}
