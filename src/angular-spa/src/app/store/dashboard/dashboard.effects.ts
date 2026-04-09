import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, of } from 'rxjs';
import { DashboardActions } from './dashboard.actions';
import { DashboardService } from '../../features/dashboard/dashboard.service';

export const loadSummary$ = createEffect(
  (actions$ = inject(Actions), dashboardService = inject(DashboardService)) =>
    actions$.pipe(
      ofType(DashboardActions.loadSummary),
      switchMap(() =>
        dashboardService.getSummary().pipe(
          map(summary => DashboardActions.loadSummarySuccess({ summary })),
          catchError(error => of(DashboardActions.loadSummaryFailure({ error: error.message })))
        )
      )
    ),
  { functional: true }
);
