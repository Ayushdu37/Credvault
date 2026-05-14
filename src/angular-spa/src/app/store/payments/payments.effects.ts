import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { catchError, map, switchMap, of, tap, delay } from 'rxjs';
import { PaymentsActions } from './payments.actions';
import { PaymentsService } from '../../features/payments/services/payments.service';
import { mapPaymentResponseToPayment, PaymentMethodLabel } from '../../core/models/payment.model';
import { DashboardActions } from '../dashboard/dashboard.actions';
import { CardsActions } from '../cards/cards.actions';

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

/** Map backend enum int → frontend label */
const paymentMethodIntToLabel: Record<number, PaymentMethodLabel> = {
  0: 'UPI',
  1: 'Bank Account',
  2: 'Debit Card',
  3: 'Bank Account', // NetBanking
  4: 'Credit Card',
};

export const submitPayment$ = createEffect(
  (actions$ = inject(Actions), paymentsService = inject(PaymentsService)) =>
    actions$.pipe(
      ofType(PaymentsActions.submitPayment),
      switchMap(({ payload }) =>
        paymentsService.makePayment(payload).pipe(
          map(paymentId => PaymentsActions.submitPaymentSuccess({ paymentId })),
          catchError(err => of(PaymentsActions.submitPaymentFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

/** After a successful payment, re-fetch the dashboard and cards so all widgets update together */
export const refreshDashboardAfterPayment$ = createEffect(
  (actions$ = inject(Actions)) =>
    actions$.pipe(
      ofType(PaymentsActions.submitPaymentSuccess),
      // Wait for backend RabbitMQ consumer to update DB
      delay(1500),
      switchMap(() => [
        DashboardActions.loadSummary(),
        CardsActions.loadCards({ page: 1, pageSize: 50 })
      ])
    ),
  { functional: true }
);
