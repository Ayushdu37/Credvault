import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { NotificationsService } from '../../features/notifications/services/notifications.service';
import { NotificationsActions } from './notifications.actions';
import { map, exhaustMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const loadNotifications = createEffect(
  (actions$ = inject(Actions), svc = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.loadNotifications),
      exhaustMap(() =>
        svc.getNotifications().pipe(
          map((notifications) => NotificationsActions.loadNotificationsSuccess({ notifications })),
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
          map((notification) => NotificationsActions.markAsReadSuccess({ notification }))
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
          map((notifications) => NotificationsActions.markAllAsReadSuccess({ notifications }))
        )
      )
    ),
  { functional: true }
);
