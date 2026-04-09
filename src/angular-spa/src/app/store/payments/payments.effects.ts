import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, of } from 'rxjs';
import { PaymentsActions } from './payments.actions';
import { PaymentsService } from '../../features/payments/services/payments.service';
import { mapPaymentResponseToPayment } from '../../core/models/payment.model';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';

export const loadPaymentHistory$ = createEffect(
  (actions$ = inject(Actions), paymentsService = inject(PaymentsService)) =>
    actions$.pipe(
      ofType(PaymentsActions.loadPaymentHistory),
      switchMap(({ page, pageSize }) =>
        paymentsService.getPayments(page, pageSize).pipe(
          map(res => {
            const payments = res.items.map(mapPaymentResponseToPayment);
            return PaymentsActions.loadPaymentHistorySuccess({ payments, totalCount: res.totalCount });
          }),
          catchError(err => of(PaymentsActions.loadPaymentHistoryFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const submitPayment$ = createEffect(
  (actions$ = inject(Actions), paymentsService = inject(PaymentsService), router = inject(Router)) =>
    actions$.pipe(
      ofType(PaymentsActions.submitPayment),
      switchMap(({ payload }) =>
        paymentsService.makePayment(payload).pipe(
          map(payment => {
            const mapped = mapPaymentResponseToPayment(payment);
            return PaymentsActions.submitPaymentSuccess({ payment: mapped });
          }),
          tap(() => router.navigate(['/payments'])),
          catchError(err => of(PaymentsActions.submitPaymentFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
