import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Notification } from '../../core/models/notification.model';
import { NotificationsActions } from './notifications.actions';

export interface NotificationsState extends EntityState<Notification> {
  loading: boolean;
  error: string | null;
  unreadCount: number;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const adapter: EntityAdapter<Notification> = createEntityAdapter<Notification>({
  sortComparer: (a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime(),
});

export const initialState: NotificationsState = adapter.getInitialState({
  loading: false,
  error: null,
  unreadCount: 0,
  page: 1,
  pageSize: 10,
  totalCount: 0,
});

export const notificationsReducer = createReducer(
  initialState,
  on(NotificationsActions.loadNotifications, (state, { page, pageSize }) =>
    ({ ...state, loading: true, error: null, page, pageSize })
  ),
  on(NotificationsActions.loadNotificationsSuccess, (state, { notifications, totalCount }) =>
    adapter.setAll(notifications, { ...state, loading: false, totalCount })
  ),
  on(NotificationsActions.loadNotificationsFailure, (state, { error }) =>
    ({ ...state, loading: false, error })
  ),
  on(NotificationsActions.markAsReadSuccess, (state, { id }) =>
    adapter.updateOne({ id, changes: { read: true } }, state)
  ),
  on(NotificationsActions.markAllAsReadSuccess, (state) => {
    const updates = (state.ids as string[]).map(nId => ({ id: nId, changes: { read: true } }));
    return adapter.updateMany(updates, { ...state, unreadCount: 0 });
  }),
  on(NotificationsActions.loadUnreadCountSuccess, (state, { count }) =>
    ({ ...state, unreadCount: count })
  ),
);
