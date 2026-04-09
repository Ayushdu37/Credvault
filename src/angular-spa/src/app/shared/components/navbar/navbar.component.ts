import { Component, Output, EventEmitter, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { selectUser } from '../../../store/auth/auth.selectors';
import { AuthActions } from '../../../store/auth/auth.actions';
import { LucideAngularModule, Menu, User, LogOut } from 'lucide-angular';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [LucideAngularModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  @Output() menuToggle = new EventEmitter<void>();

  private store = inject(Store);
  private router = inject(Router);

  user = toSignal(this.store.select(selectUser));
  dropdownOpen = false;

  get userInitials(): string {
    const name = this.user()?.fullName || '';
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }

  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }

  closeDropdown(): void {
    this.dropdownOpen = false;
  }

  logout(): void {
    this.dropdownOpen = false;
    this.store.dispatch(AuthActions.logout());
  }

  goToProfile(): void {
    this.dropdownOpen = false;
    this.router.navigateByUrl('/profile');
  }
}
