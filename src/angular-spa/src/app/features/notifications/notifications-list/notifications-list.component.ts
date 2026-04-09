import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { toSignal } from '@angular/core/rxjs-interop';
import { DatePipe, NgClass } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { NotificationsActions } from '../../../store/notifications/notifications.actions';
import {
  selectAllNotifications,
  selectNotificationsLoading,
  selectUnreadCount,
} from '../../../store/notifications/notifications.selectors';

@Component({
  selector: 'app-notifications-list',
  standalone: true,
  imports: [
    DatePipe,
    NgClass,
    LucideAngularModule,
    CardComponent,
    ButtonComponent,
    SpinnerComponent,
    EmptyStateComponent,
  ],
  templateUrl: './notifications-list.component.html',
  styleUrl: './notifications-list.component.css',
})
export class NotificationsListComponent implements OnInit {
  private store = inject(Store);

  notifications = toSignal(this.store.select(selectAllNotifications), { initialValue: [] });
  loading = toSignal(this.store.select(selectNotificationsLoading), { initialValue: false });
  unreadCount = toSignal(this.store.select(selectUnreadCount), { initialValue: 0 });

  ngOnInit(): void {
    this.store.dispatch(NotificationsActions.loadNotifications());
  }

  markAsRead(id: string): void {
    this.store.dispatch(NotificationsActions.markAsRead({ id }));
  }

  markAllAsRead(): void {
    this.store.dispatch(NotificationsActions.markAllAsRead());
  }

  getTypeClass(type: string): string {
    const map: Record<string, string> = {
      payment: 'type-payment',
      security: 'type-security',
      card: 'type-card',
      billing: 'type-billing',
      system: 'type-system',
    };
    return map[type] || 'type-system';
  }

  getTypeLabel(type: string): string {
    const map: Record<string, string> = {
      payment: 'Payment',
      security: 'Security',
      card: 'Card',
      billing: 'Billing',
      system: 'System',
    };
    return map[type] || 'System';
  }
}
