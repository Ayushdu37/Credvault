import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { PaymentMethodsService } from '../../features/payment-methods/services/payment-methods.service';
import { PaymentMethodsActions } from './payment-methods.actions';
import { map, exhaustMap, catchError, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { mapPaymentMethodResponseToPaymentMethod } from '../../core/models/payment.model';

export const loadMethods = createEffect(
  (actions$ = inject(Actions), svc = inject(PaymentMethodsService)) =>
    actions$.pipe(
      ofType(PaymentMethodsActions.loadMethods),
      switchMap(({ page, pageSize }) =>
        svc.getPaymentMethods(page, pageSize).pipe(
          map(res => {
            const methods = res.items.map(mapPaymentMethodResponseToPaymentMethod);
            return PaymentMethodsActions.loadMethodsSuccess({
              methods,
              totalCount: res.totalCount,
            });
          }),
          catchError((err) => of(PaymentMethodsActions.loadMethodsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const addMethod = createEffect(
  (actions$ = inject(Actions), svc = inject(PaymentMethodsService)) =>
    actions$.pipe(
      ofType(PaymentMethodsActions.addMethod),
      exhaustMap(({ payload }) =>
        svc.addPaymentMethod(payload).pipe(
          switchMap(() => [
            PaymentMethodsActions.addMethodSuccess(),
            PaymentMethodsActions.loadMethods({ page: 1, pageSize: 10 })
          ]),
          catchError((err) => of(PaymentMethodsActions.addMethodFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const deleteMethod = createEffect(
  (actions$ = inject(Actions), svc = inject(PaymentMethodsService)) =>
    actions$.pipe(
      ofType(PaymentMethodsActions.deleteMethod),
      exhaustMap(({ id }) =>
        svc.deletePaymentMethod(id).pipe(
          map(() => PaymentMethodsActions.deleteMethodSuccess({ id })),
          catchError((err) => of(PaymentMethodsActions.deleteMethodFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
