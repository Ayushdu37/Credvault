import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, of } from 'rxjs';
import { BillingActions } from './billing.actions';
import { BillingService } from '../../features/billing/services/billing.service';
import { mapBillResponseToStatement } from '../../core/models/billing.model';

export const loadBills$ = createEffect(
  (actions$ = inject(Actions), billingService = inject(BillingService)) =>
    actions$.pipe(
      ofType(BillingActions.loadBills),
      switchMap(({ page, pageSize }) =>
        billingService.getBills(page, pageSize).pipe(
          map(res => {
            const bills = res.items.map(b => mapBillResponseToStatement(b));
            return BillingActions.loadBillsSuccess({ bills, totalCount: res.totalCount });
          }),
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
          map(bill => {
            const mapped = mapBillResponseToStatement(bill);
            return BillingActions.loadBillDetailSuccess({ bill: mapped });
          }),
          catchError(err => of(BillingActions.loadBillDetailFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const refreshBillAfterPayment$ = createEffect(
  (actions$ = inject(Actions), billingService = inject(BillingService)) =>
    actions$.pipe(
      ofType(BillingActions.refreshBillAfterPayment),
      switchMap(({ billId }) =>
        billingService.getBillById(billId).pipe(
          map(bill => {
            const mapped = mapBillResponseToStatement(bill);
            return BillingActions.refreshBillSuccess({ bill: mapped });
          }),
          catchError(() => of())
        )
      )
    ),
  { functional: true }
);

export const schedulePayment$ = createEffect(
  (actions$ = inject(Actions), billingService = inject(BillingService)) =>
    actions$.pipe(
      ofType(BillingActions.schedulePayment),
      switchMap(({ billId, payload }) =>
        billingService.schedulePayment(billId, payload).pipe(
          map(schedule => BillingActions.schedulePaymentSuccess({ schedule })),
          catchError(err => of(BillingActions.schedulePaymentFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const cancelScheduledPayment$ = createEffect(
  (actions$ = inject(Actions), billingService = inject(BillingService)) =>
    actions$.pipe(
      ofType(BillingActions.cancelScheduledPayment),
      switchMap(({ scheduleId }) =>
        billingService.cancelScheduledPayment(scheduleId).pipe(
          map(() => BillingActions.cancelScheduledPaymentSuccess({ scheduleId })),
          catchError(err => of(BillingActions.cancelScheduledPaymentFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
