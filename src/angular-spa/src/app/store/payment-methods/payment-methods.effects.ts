import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { PaymentMethodsService } from '../../features/payment-methods/services/payment-methods.service';
import { PaymentMethodsActions } from './payment-methods.actions';
import { map, exhaustMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const loadMethods = createEffect(
  (actions$ = inject(Actions), svc = inject(PaymentMethodsService)) =>
    actions$.pipe(
      ofType(PaymentMethodsActions.loadMethods),
      exhaustMap(() =>
        svc.getPaymentMethods().pipe(
          map((methods) => PaymentMethodsActions.loadMethodsSuccess({ methods })),
          catchError((err) => of(PaymentMethodsActions.loadMethodsFailure({ error: err.message })))
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
          map((deletedId) => PaymentMethodsActions.deleteMethodSuccess({ id: deletedId }))
        )
      )
    ),
  { functional: true }
);

export const setDefault = createEffect(
  (actions$ = inject(Actions), svc = inject(PaymentMethodsService)) =>
    actions$.pipe(
      ofType(PaymentMethodsActions.setDefault),
      exhaustMap(({ id }) =>
        svc.setDefault(id).pipe(
          map((methods) => PaymentMethodsActions.setDefaultSuccess({ methods }))
        )
      )
    ),
  { functional: true }
);
