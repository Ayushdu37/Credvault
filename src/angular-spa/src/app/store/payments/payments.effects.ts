import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, of } from 'rxjs';
import { PaymentsActions } from './payments.actions';
import { PaymentsService } from '../../features/payments/services/payments.service';

export const loadPaymentHistory$ = createEffect(
  (actions$ = inject(Actions), paymentsService = inject(PaymentsService)) =>
    actions$.pipe(
      ofType(PaymentsActions.loadPaymentHistory),
      switchMap(() =>
        paymentsService.getPaymentHistory().pipe(
          map(payments => PaymentsActions.loadPaymentHistorySuccess({ payments })),
          catchError(err => of(PaymentsActions.loadPaymentHistoryFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const submitPayment$ = createEffect(
  (actions$ = inject(Actions), paymentsService = inject(PaymentsService)) =>
    actions$.pipe(
      ofType(PaymentsActions.submitPayment),
      switchMap(({ payload }) =>
        paymentsService.submitPayment(payload).pipe(
          map(res => PaymentsActions.submitPaymentSuccess({
            referenceNumber: res.referenceNumber,
            message: res.message,
          })),
          catchError(err => of(PaymentsActions.submitPaymentFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
