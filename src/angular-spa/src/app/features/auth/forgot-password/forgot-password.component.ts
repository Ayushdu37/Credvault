import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { AuthService } from '../../../core/services/auth.service';
import { OTPPurpose } from '../../../core/models/enums.model';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, LucideAngularModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['../mfa/mfa.component.css'] // Reusing MFA layout CSS for consistency
})
export class ForgotPasswordComponent {
  forgotForm: FormGroup;
  resetForm: FormGroup;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;
  step: 'email' | 'otp' | 'password' | 'success' = 'email';

  constructor(
    private fb: FormBuilder, 
    private router: Router,
    private authService: AuthService
  ) {
    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });

    this.resetForm = this.fb.group({
      otpCode: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get emailControl() {
    return this.forgotForm.get('email');
  }

  get otpControl() {
    return this.resetForm.get('otpCode');
  }

  get newPasswordControl() {
    return this.resetForm.get('newPassword');
  }

  onSubmit() {
    if (this.forgotForm.invalid) {
      this.forgotForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;

    this.authService.sendOtp({
      email: this.emailControl?.value,
      purpose: 'PasswordReset'
    }).subscribe({
      next: () => {
        this.loading = false;
        this.step = 'otp';
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to send reset code. Please try again.';
      }
    });
  }

  onVerifyOtp() {
    if (this.otpControl?.invalid) {
      this.otpControl.markAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;

    this.authService.verifyOtp({
      email: this.emailControl?.value,
      otpCode: this.otpControl?.value,
      purpose: 'PasswordReset'
    }).subscribe({
      next: () => {
        this.loading = false;
        this.step = 'password';
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Incorrect OTP code. Please check your email or resend.';
      }
    });
  }

  resendOtp() {
    this.onSubmit(); // Re-use the send logic
  }

  onResetSubmit() {
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;

    this.authService.resetPassword({
      email: this.emailControl?.value,
      otpCode: this.otpControl?.value,
      newPassword: this.newPasswordControl?.value
    }).subscribe({
      next: () => {
        this.loading = false;
        this.step = 'success';
        
        // Automatically redirect to login after 3 seconds
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to reset password. The code might be invalid or expired.';
      }
    });
  }
}
