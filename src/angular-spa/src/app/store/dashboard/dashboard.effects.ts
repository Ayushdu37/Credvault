import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, of, tap } from 'rxjs';
import { DashboardActions } from './dashboard.actions';
import { DashboardService } from '../../features/dashboard/dashboard.service';

export const loadSummary$ = createEffect(
  (actions$ = inject(Actions), dashboardService = inject(DashboardService)) =>
    actions$.pipe(
      ofType(DashboardActions.loadSummary),
      tap(() => console.log('[Dashboard] Loading summary...')),
      switchMap(() =>
        dashboardService.getSummary().pipe(
          tap(summary => console.log('[Dashboard] Summary loaded:', summary)),
          map(summary => DashboardActions.loadSummarySuccess({ summary })),
          catchError(error => {
            console.error('[Dashboard] Failed to load summary:', error);
            return of(DashboardActions.loadSummaryFailure({ error: error.message }));
          })
        )
      )
    ),
  { functional: true }
);
