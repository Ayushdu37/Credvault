import { createFeatureSelector, createSelector } from '@ngrx/store';
import { NotificationsState, adapter } from './notifications.reducer';

export const selectNotificationsState = createFeatureSelector<NotificationsState>('notifications');

const { selectAll } = adapter.getSelectors();

export const selectAllNotifications = createSelector(selectNotificationsState, selectAll);
export const selectNotificationsLoading = createSelector(selectNotificationsState, (s) => s.loading);
export const selectNotificationsError = createSelector(selectNotificationsState, (s) => s.error);
export const selectUnreadCount = createSelector(selectAllNotifications, (notifs) => notifs.filter(n => !n.read).length);
