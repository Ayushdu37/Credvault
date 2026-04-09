import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Notification } from '../../features/notifications/services/notifications.service';
import { NotificationsActions } from './notifications.actions';

export interface NotificationsState extends EntityState<Notification> {
  loading: boolean;
  error: string | null;
}

export const adapter: EntityAdapter<Notification> = createEntityAdapter<Notification>({
  sortComparer: (a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime(),
});

export const initialState: NotificationsState = adapter.getInitialState({
  loading: false,
  error: null,
});

export const notificationsReducer = createReducer(
  initialState,
  on(NotificationsActions.loadNotifications, (state) => ({ ...state, loading: true, error: null })),
  on(NotificationsActions.loadNotificationsSuccess, (state, { notifications }) =>
    adapter.setAll(notifications, { ...state, loading: false })
  ),
  on(NotificationsActions.loadNotificationsFailure, (state, { error }) => ({ ...state, loading: false, error })),
  on(NotificationsActions.markAsReadSuccess, (state, { notification }) =>
    adapter.updateOne({ id: notification.id, changes: { read: true } }, state)
  ),
  on(NotificationsActions.markAllAsReadSuccess, (state, { notifications }) =>
    adapter.setAll(notifications, state)
  ),
);
