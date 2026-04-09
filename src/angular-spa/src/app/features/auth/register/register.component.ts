import { Component, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { FormsModule } from '@angular/forms';
import { AsyncPipe, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { AuthActions } from '../../../store/auth/auth.actions';
import {
  selectAuthLoading,
  selectAuthError,
} from '../../../store/auth/auth.selectors';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [FormsModule, AsyncPipe, RouterLink, LucideAngularModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private store = inject(Store);
  
  fullName = '';
  email = '';
  phoneNumber = '';
  password = '';
  confirmPassword = '';
  
  loading$ = this.store.select(selectAuthLoading);
  error$ = this.store.select(selectAuthError);

  passwordVisible = false;

  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  get passwordStrength(): string {
    if (!this.password) return '';
    if (this.password.length < 6) return 'Weak';
    if (this.password.match(/[0-9]/) && this.password.match(/[a-zA-Z]/) && this.password.length >= 8) {
      if (this.password.match(/[^a-zA-Z0-9]/)) return 'Strong';
      return 'Medium';
    }
    return 'Weak';
  }

  get strengthColor(): string {
    const strength = this.passwordStrength;
    if (strength === 'Strong') return 'var(--success)';
    if (strength === 'Medium') return 'var(--warning)';
    return 'var(--danger)';
  }

  onRegister(): void {
    if (this.password !== this.confirmPassword) {
      // In a real app we'd dispatch an error or set a local error state, but let's assume basic validation
      this.store.dispatch(AuthActions.registerFailure({ error: 'Passwords do not match' }));
      return;
    }

    if (this.email && this.password && this.fullName && this.phoneNumber) {
      this.store.dispatch(
        AuthActions.register({ 
          email: this.email, 
          password: this.password,
          fullName: this.fullName,
          phoneNumber: this.phoneNumber
        })
      );
    }
  }
}
