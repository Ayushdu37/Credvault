import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

export interface Notification {
  id: string;
  type: 'payment' | 'security' | 'card' | 'billing' | 'system';
  title: string;
  message: string;
  timestamp: string;
  read: boolean;
  icon: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationsService {

  private mockNotifications: Notification[] = [
    {
      id: 'notif-001',
      type: 'payment',
      title: 'Payment Received',
      message: 'Your payment of ₹12,500.00 towards Platinum Credit Card has been received successfully.',
      timestamp: '2025-03-15T10:30:00Z',
      read: false,
      icon: 'check-circle',
    },
    {
      id: 'notif-002',
      type: 'security',
      title: 'Unusual Login Detected',
      message: 'A login was detected from a new device in Mumbai, Maharashtra. If this was not you, please secure your account immediately.',
      timestamp: '2025-03-14T22:15:00Z',
      read: false,
      icon: 'shield-alert',
    },
    {
      id: 'notif-003',
      type: 'card',
      title: 'Card Locked',
      message: 'Your Gold Rewards Card ending in 8832 has been locked as per your request.',
      timestamp: '2025-03-14T18:45:00Z',
      read: true,
      icon: 'lock',
    },
    {
      id: 'notif-004',
      type: 'billing',
      title: 'Statement Ready',
      message: 'Your March 2025 billing statement for Platinum Credit Card is now available. Total due: ₹24,750.00.',
      timestamp: '2025-03-12T09:00:00Z',
      read: true,
      icon: 'file-text',
    },
    {
      id: 'notif-005',
      type: 'system',
      title: 'Scheduled Maintenance',
      message: 'CredVault will undergo scheduled maintenance on March 20, 2025 from 2:00 AM to 4:00 AM IST.',
      timestamp: '2025-03-10T12:00:00Z',
      read: true,
      icon: 'info',
    },
    {
      id: 'notif-006',
      type: 'payment',
      title: 'Auto-Pay Scheduled',
      message: 'Auto-pay of ₹8,200.00 for Gold Rewards Card is scheduled for March 25, 2025.',
      timestamp: '2025-03-09T14:30:00Z',
      read: true,
      icon: 'clock',
    },
    {
      id: 'notif-007',
      type: 'security',
      title: 'Password Changed',
      message: 'Your account password was successfully changed. If you did not make this change, contact support.',
      timestamp: '2025-03-08T16:20:00Z',
      read: true,
      icon: 'shield-check',
    },
    {
      id: 'notif-008',
      type: 'card',
      title: 'Credit Limit Increased',
      message: 'Congratulations! Your Platinum Credit Card limit has been increased from ₹2,00,000 to ₹3,00,000.',
      timestamp: '2025-03-05T11:00:00Z',
      read: true,
      icon: 'trending-up',
    },
  ];

  getNotifications(): Observable<Notification[]> {
    return of(this.mockNotifications).pipe(delay(400));
  }

  markAsRead(id: string): Observable<Notification> {
    const notif = this.mockNotifications.find(n => n.id === id);
    if (notif) {
      notif.read = true;
    }
    return of(notif!).pipe(delay(200));
  }

  markAllAsRead(): Observable<Notification[]> {
    this.mockNotifications.forEach(n => n.read = true);
    return of(this.mockNotifications).pipe(delay(300));
  }
}
