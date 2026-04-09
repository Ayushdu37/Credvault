import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, of } from 'rxjs';
import { CardsActions } from './cards.actions';
import { CardsService } from '../../features/cards/services/cards.service';
import { mapCardResponseToCreditCard, AddCardRequest } from '../../core/models/card.model';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';

export const loadCards$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.loadCards),
      switchMap(({ page, pageSize }) =>
        cardsService.getCards(page, pageSize).pipe(
          map(res => {
            const cards = res.items.map(mapCardResponseToCreditCard);
            return CardsActions.loadCardsSuccess({ cards, totalCount: res.totalCount });
          }),
          catchError(err => of(CardsActions.loadCardsFailure({ error: err.message })))
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
        cardsService.setDefaultCard(id).pipe(
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
  (actions$ = inject(Actions), cardsService = inject(CardsService), router = inject(Router)) =>
    actions$.pipe(
      ofType(CardsActions.addCard),
      switchMap(({ payload }) =>
        cardsService.addCard(payload).pipe(
          switchMap((idOrCard: any) => {
            // If API returned only an ID (string), fetch full details
            const id = typeof idOrCard === 'string' ? idOrCard : idOrCard.id;
            return cardsService.getCardById(id).pipe(
              map(card => {
                const mapped = mapCardResponseToCreditCard(card);
                return CardsActions.addCardSuccess({ card: mapped });
              })
            );
          }),
          tap(() => router.navigate(['/cards'])),
          catchError(err => of(CardsActions.addCardFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const updateCardLimit$ = createEffect(
  (actions$ = inject(Actions), cardsService = inject(CardsService)) =>
    actions$.pipe(
      ofType(CardsActions.updateCardLimit),
      switchMap(({ id, newLimit }) =>
        cardsService.updateCardLimit(id, { newLimit }).pipe(
          map(() => CardsActions.updateCardLimitSuccess({ id, newLimit })),
          catchError(err => of(CardsActions.updateCardLimitFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);
