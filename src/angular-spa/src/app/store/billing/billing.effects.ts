import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, of } from 'rxjs';
import { BillingActions } from './billing.actions';
import { BillingService } from '../../features/billing/services/billing.service';

export const loadBills$ = createEffect(
  (actions$ = inject(Actions), billingService = inject(BillingService)) =>
    actions$.pipe(
      ofType(BillingActions.loadBills),
      switchMap(() =>
        billingService.getBills().pipe(
          map(bills => BillingActions.loadBillsSuccess({ bills })),
          catchError(err => of(BillingActions.loadBillsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const loadBillDetail$ = createEffect(
  (actions$ = inject(Actions), billingService = inject(BillingService)) =>
    actions$.pipe(
      ofType(BillingActions.loadBillDetail),
      switchMap(({ id }) =>
        billingService.getBillById(id).pipe(
          map(bill => bill
            ? BillingActions.loadBillDetailSuccess({ bill })
            : BillingActions.loadBillDetailFailure({ error: 'Statement not found' })
          ),
          catchError(err => of(BillingActions.loadBillDetailFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
