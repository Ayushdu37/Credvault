import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { Store } from '@ngrx/store';
import { AuthActions } from '../../../store/auth/auth.actions';
import { selectAuthError, selectAuthLoading } from '../../../store/auth/auth.selectors';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule, ReactiveFormsModule],
  templateUrl: './verify-email.component.html',
  styleUrls: ['../mfa/mfa.component.css'] // Reusing MFA layout CSS for consistency
})
export class VerifyEmailComponent implements OnInit {
  status: 'form' | 'verifying' | 'success' | 'error' = 'form';
  errorMessage: string | null = null;
  email: string | null = null;
  
  verifyForm: FormGroup;
  resendSuccess = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private store: Store,
    private fb: FormBuilder
  ) {
    this.verifyForm = this.fb.group({
      code: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
    });
  }

  get codeControl() {
    return this.verifyForm.get('code');
  }

  ngOnInit() {
    // If we have a token in URL, verify automatically.
    // Otherwise, show the form so the user can type it.
    const urlToken = this.route.snapshot.queryParamMap.get('token');
    this.email = this.route.snapshot.queryParamMap.get('email');

    if (!this.email) {
      this.status = 'error';
      this.errorMessage = 'Invalid link. Missing email.';
      return;
    }

    if (urlToken) {
      this.verifyForm.patchValue({ code: urlToken });
      this.verifyEmail();
    }
    
    // Listen for store errors
    this.store.select(selectAuthError).subscribe(error => {
      if (error && this.status === 'verifying') {
        this.status = 'error';
        this.errorMessage = error;
      }
    });
    
    // Watch loading to reset status
    this.store.select(selectAuthLoading).subscribe(loading => {
      if (!loading && this.status === 'verifying' && !this.errorMessage) {
        this.status = 'success';
      }
    });
  }

  verifyEmail() {
    if (this.verifyForm.invalid || !this.email) return;

    this.status = 'verifying';
    this.errorMessage = null;

    const code = this.verifyForm.value.code;
    this.store.dispatch(AuthActions.verifyEmail({ email: this.email, otpCode: code }));
  }

  resendCode() {
    if (!this.email) return;
    this.store.dispatch(AuthActions.sendOTP({ email: this.email, purpose: 'EmailVerification' }));
    
    this.resendSuccess = true;
    setTimeout(() => this.resendSuccess = false, 3000);
  }
}
