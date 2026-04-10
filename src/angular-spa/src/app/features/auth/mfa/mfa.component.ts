import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { Store } from '@ngrx/store';
import { AuthActions } from '../../../store/auth/auth.actions';

import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-mfa',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, LucideAngularModule],
  templateUrl: './mfa.component.html',
  styleUrls: ['./mfa.component.css']
})
export class MfaComponent {
  mfaForm: FormGroup;
  loading = false;
  error: string | null = null;
  resendSuccess = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private store: Store
  ) {
    this.mfaForm = this.fb.group({
      code: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
    });
  }

  get codeControl() {
    return this.mfaForm.get('code');
  }

  onVerify() {
    if (this.mfaForm.invalid) {
      this.mfaForm.markAllAsTouched();
      return;
    }

    // Assuming we're looking up email from auth state or query params.
    // For MFA, we usually have a token or an email saved from the login flow.
    // Let's assume we grabbed email from the URL or state (we'll need to pass it!).
    // For now, I'll dispatch it assuming we have the email stored somewhere.
    // In a prod app, this component would pull `email` from `this.route.snapshot.queryParams` or Store.
    
    // We'll read the email from route params for this mock setup
    const simulatedEmail = window.history.state.email || 'user@credvault.com'; // Fallback
    const code = this.mfaForm.value.code;

    this.store.dispatch(AuthActions.verifyOTP({ 
      email: simulatedEmail, 
      otpCode: code, 
      purpose: 'Login' 
    }));
  }

  resendCode() {
    const simulatedEmail = window.history.state.email || 'user@credvault.com';
    this.store.dispatch(AuthActions.sendOTP({ email: simulatedEmail, purpose: 'Login' }));
    
    this.resendSuccess = true;
    setTimeout(() => this.resendSuccess = false, 3000);
  }
}
