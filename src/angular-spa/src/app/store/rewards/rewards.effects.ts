import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { RewardsService } from '../../features/rewards/services/rewards.service';
import { RewardsActions } from './rewards.actions';
import { map, exhaustMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const loadAccount = createEffect(
  (actions$ = inject(Actions), svc = inject(RewardsService)) =>
    actions$.pipe(
      ofType(RewardsActions.loadAccount),
      exhaustMap(() =>
        svc.getRewardAccount().pipe(
          map((account) => RewardsActions.loadAccountSuccess({ account })),
          catchError((err) => of(RewardsActions.loadAccountFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const loadTiers = createEffect(
  (actions$ = inject(Actions), svc = inject(RewardsService)) =>
    actions$.pipe(
      ofType(RewardsActions.loadTiers),
      exhaustMap(() =>
        svc.getTiers().pipe(
          map((tiers) => RewardsActions.loadTiersSuccess({ tiers }))
        )
      )
    ),
  { functional: true }
);

export const loadTransactions = createEffect(
  (actions$ = inject(Actions), svc = inject(RewardsService)) =>
    actions$.pipe(
      ofType(RewardsActions.loadTransactions),
      exhaustMap(() =>
        svc.getTransactions().pipe(
          map((transactions) => RewardsActions.loadTransactionsSuccess({ transactions })),
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
      exhaustMap(({ points }) =>
        svc.redeemPoints(points).pipe(
          map((account) => RewardsActions.redeemPointsSuccess({ account })),
          catchError((err) => of(RewardsActions.redeemPointsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
