import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { NotificationsService } from '../../features/notifications/services/notifications.service';
import { NotificationsActions } from './notifications.actions';
import { map, exhaustMap, catchError, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { mapNotificationResponseToNotification } from '../../core/models/notification.model';

export const loadNotifications = createEffect(
  (actions$ = inject(Actions), svc = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.loadNotifications),
      switchMap(({ page, pageSize }) =>
        svc.getNotifications(page, pageSize).pipe(
          map(res => {
            const notifications = res.items.map(mapNotificationResponseToNotification);
            return NotificationsActions.loadNotificationsSuccess({
              notifications,
              totalCount: res.totalCount,
            });
          }),
          catchError((err) => of(NotificationsActions.loadNotificationsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const markAsRead = createEffect(
  (actions$ = inject(Actions), svc = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.markAsRead),
      exhaustMap(({ id }) =>
        svc.markAsRead(id).pipe(
          map(() => NotificationsActions.markAsReadSuccess({ id })),
          catchError((err) => of(NotificationsActions.loadNotificationsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const markAllAsRead = createEffect(
  (actions$ = inject(Actions), svc = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.markAllAsRead),
      exhaustMap(() =>
        svc.markAllAsRead().pipe(
          map(() => NotificationsActions.markAllAsReadSuccess()),
          catchError((err) => of(NotificationsActions.loadNotificationsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const loadUnreadCount = createEffect(
  (actions$ = inject(Actions), svc = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.loadUnreadCount),
      exhaustMap(() =>
        svc.getUnreadCount().pipe(
          map(count => NotificationsActions.loadUnreadCountSuccess({ count })),
          catchError((err) => of(NotificationsActions.loadNotificationsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const loadPreferences = createEffect(
  (actions$ = inject(Actions), svc = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.loadPreferences),
      exhaustMap(() =>
        svc.getPreferences().pipe(
          map(preferences => NotificationsActions.loadPreferencesSuccess({ preferences })),
          catchError((err) => of(NotificationsActions.loadNotificationsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const updatePreferences = createEffect(
  (actions$ = inject(Actions), svc = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.updatePreferences),
      exhaustMap(({ payload }) =>
        svc.updatePreferences(payload).pipe(
          map(preferences => NotificationsActions.updatePreferencesSuccess({ preferences })),
          catchError((err) => of(NotificationsActions.loadNotificationsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
