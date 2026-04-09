import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { RewardsService } from '../../features/rewards/services/rewards.service';
import { RewardsActions } from './rewards.actions';
import { map, exhaustMap, catchError, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { mapRewardAccountResponse, mapRewardTransactionResponse } from '../../core/models/billing.model';

export const loadAccount = createEffect(
  (actions$ = inject(Actions), svc = inject(RewardsService)) =>
    actions$.pipe(
      ofType(RewardsActions.loadAccount),
      exhaustMap(() =>
        svc.getRewardAccount().pipe(
          map(account => {
            const mapped = mapRewardAccountResponse(account);
            return RewardsActions.loadAccountSuccess({ account: mapped });
          }),
          catchError((err) => of(RewardsActions.loadAccountFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const loadTransactions = createEffect(
  (actions$ = inject(Actions), svc = inject(RewardsService)) =>
    actions$.pipe(
      ofType(RewardsActions.loadTransactions),
      switchMap(({ page, pageSize }) =>
        svc.getTransactions(page, pageSize).pipe(
          map(res => {
            const transactions = res.items.map(mapRewardTransactionResponse);
            return RewardsActions.loadTransactionsSuccess({
              transactions,
              totalCount: res.totalCount,
            });
          }),
          catchError((err) => of(RewardsActions.loadTransactionsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const redeemPoints = createEffect(
  (actions$ = inject(Actions), svc = inject(RewardsService)) =>
    actions$.pipe(
      ofType(RewardsActions.redeemPoints),
      exhaustMap(({ payload }) =>
        svc.redeemPoints(payload).pipe(
          map(account => {
            const mapped = mapRewardAccountResponse(account);
            return RewardsActions.redeemPointsSuccess({ account: mapped });
          }),
          catchError((err) => of(RewardsActions.redeemPointsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
