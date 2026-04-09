import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, of } from 'rxjs';
import { CardsActions } from './cards.actions';
import { CardsService } from '../../features/cards/services/cards.service';

export const loadCards$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.loadCards),
      switchMap(() =>
        cardsService.getCards().pipe(
          map(cards => CardsActions.loadCardsSuccess({ cards })),
          catchError(err => of(CardsActions.loadCardsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const lockCard$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.lockCard),
      switchMap(({ id }) =>
        cardsService.lockCard(id).pipe(
          map(() => CardsActions.lockCardSuccess({ id })),
          catchError(err => of(CardsActions.lockCardFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const unlockCard$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.unlockCard),
      switchMap(({ id }) =>
        cardsService.unlockCard(id).pipe(
          map(() => CardsActions.unlockCardSuccess({ id })),
          catchError(err => of(CardsActions.unlockCardFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const requestCard$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.requestCard),
      switchMap(({ payload }) =>
        cardsService.requestNewCard(payload).pipe(
          map(res => CardsActions.requestCardSuccess({ message: res.message })),
          catchError(err => of(CardsActions.requestCardFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const setDefaultCard$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.setDefaultCard),
      switchMap(({ id }) =>
        cardsService.setDefault(id).pipe(
          map(() => CardsActions.setDefaultCardSuccess({ id })),
          catchError(err => of(CardsActions.setDefaultCardFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const verifyCard$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.verifyCard),
      switchMap(({ id }) =>
        cardsService.verifyCard(id).pipe(
          map(() => CardsActions.verifyCardSuccess({ id })),
          catchError(err => of(CardsActions.verifyCardFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const deleteCard$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.deleteCard),
      switchMap(({ id }) =>
        cardsService.deleteCard(id).pipe(
          map(() => CardsActions.deleteCardSuccess({ id })),
          catchError(err => of(CardsActions.deleteCardFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const addCard$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.addCard),
      switchMap(({ payload }) =>
        cardsService.addCard(payload).pipe(
          map(res => CardsActions.addCardSuccess({ message: res.message })),
          catchError(err => of(CardsActions.addCardFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
