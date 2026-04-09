import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, LucideAngularModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['../mfa/mfa.component.css'] // Reusing MFA layout CSS for consistency
})
export class ForgotPasswordComponent {
  forgotForm: FormGroup;
  loading = false;
  error: string | null = null;
  submitted = false;

  constructor(private fb: FormBuilder, private router: Router) {
    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  get emailControl() {
    return this.forgotForm.get('email');
  }

  onSubmit() {
    if (this.forgotForm.invalid) {
      this.forgotForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;

    // TODO: Wire to backend call [HttpPost("reset-password")] (first step: request token)
    setTimeout(() => {
      this.loading = false;
      this.submitted = true;
    }, 1000);
  }
}
