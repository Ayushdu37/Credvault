import { Component, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { toSignal } from '@angular/core/rxjs-interop';
import { DatePipe, CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { AuthActions } from '../../../store/auth/auth.actions';
import { selectUser } from '../../../store/auth/auth.selectors';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    DatePipe,
    CommonModule,
    LucideAngularModule,
    CardComponent,
    ButtonComponent,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  private store = inject(Store);

  user = toSignal(this.store.select(selectUser));

  // Mock profile details that extend the auth user
  profileDetails = {
    phone: '+91 98765 43210',
    address: '42 Finance Street, Bandra West, Mumbai 400050',
    dateOfBirth: '1992-06-15',
    panNumber: 'ABCDE1234F',
    aadhaar: 'XXXX XXXX 7890',
    memberSince: '2023-04-20T00:00:00Z',
    accountType: 'Premium',
    twoFactorEnabled: true,
    emailNotifications: true,
    smsAlerts: true,
  };

  get userInitials(): string {
    const name = this.user()?.fullName || '';
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }

  // Match backend schema for notifications
  notificationPrefs = {
    emailEnabled: true,
    paymentAlerts: true,
    billReminders: true,
    rewardUpdates: false
  };

  // Mock active sessions matching GET /api/users/sessions
  activeSessions = [
    { id: 'sess-001', deviceName: 'MacBook Pro 16"', deviceType: 'laptop', browser: 'Chrome 122.0', ipAddress: '192.168.1.45', lastActive: '2026-04-06T10:15:00Z', isCurrent: true },
    { id: 'sess-002', deviceName: 'iPhone 15 Pro', deviceType: 'smartphone', browser: 'Safari Mobile', ipAddress: '100.22.45.12', lastActive: '2026-04-05T18:30:00Z', isCurrent: false },
    { id: 'sess-003', deviceName: 'Windows PC', deviceType: 'desktop', browser: 'Edge 121.0', ipAddress: '204.14.33.11', lastActive: '2026-04-02T09:00:00Z', isCurrent: false },
  ];

  ngOnInit(): void {
    this.store.dispatch(AuthActions.loadProfile());
  }

  togglePref(key: keyof typeof this.notificationPrefs): void {
    this.notificationPrefs[key] = !this.notificationPrefs[key];
    // In real app, dispatch to store here to PUT /api/notifications/preferences
  }

  revokeSession(id: string): void {
    const session = this.activeSessions.find(s => s.id === id);
    if (!session) return;
    
    // In real app, call ModalService to confirm, then DELETE /api/users/sessions/{id}
    this.activeSessions = this.activeSessions.filter(s => s.id !== id);
  }
}
