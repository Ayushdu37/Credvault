import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';

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
    private router: Router
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

    this.loading = true;
    this.error = null;

    // TODO: Wire to AuthStore / AuthService Backend Call: [HttpPost("verify-otp")]
    // Simulating API call
    setTimeout(() => {
      const code = this.mfaForm.value.code;
      if (code === '123456') { // Mock success
        this.loading = false;
        this.router.navigate(['/dashboard']);
      } else {
        this.error = 'Invalid authentication code. Please try again.';
        this.loading = false;
      }
    }, 1000);
  }

  resendCode() {
    // TODO: Wire to backend call: [HttpPost("send-otp")]
    this.resendSuccess = true;
    setTimeout(() => this.resendSuccess = false, 3000);
  }
}
