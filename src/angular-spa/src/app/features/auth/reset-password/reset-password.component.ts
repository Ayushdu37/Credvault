import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { AppValidators } from '../../../core/utils/form-validators';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, LucideAngularModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['../mfa/mfa.component.css'] // Reusing MFA layout CSS for consistency
})
export class ResetPasswordComponent implements OnInit {
  resetForm: FormGroup;
  loading = false;
  error: string | null = null;
  success = false;
  token: string | null = null;
  email: string | null = null;

  constructor(
    private fb: FormBuilder, 
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.resetForm = this.fb.group({
      password: ['', [Validators.required, AppValidators.passwordComplexity()]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: AppValidators.matchFields('password', 'confirmPassword') });
  }

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token');
    this.email = this.route.snapshot.queryParamMap.get('email');

    // If no token or email, they shouldn't be here in a real scenario
    // We could show an error or redirect, but for UI parity we'll keep it simple
  }

  get passwordControl() { return this.resetForm.get('password'); }
  get confirmPasswordControl() { return this.resetForm.get('confirmPassword'); }

  onSubmit() {
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;

    // TODO: Wire to backend call [HttpPost("reset-password")] (second step: actual reset)
    setTimeout(() => {
      this.loading = false;
      this.success = true;
    }, 1000);
  }
}
