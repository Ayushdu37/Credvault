import { createFeatureSelector, createSelector } from '@ngrx/store';
import { CardsState, cardsAdapter } from './cards.reducer';

export const selectCardsState = createFeatureSelector<CardsState>('cards');

const { selectAll, selectEntities, selectTotal } = cardsAdapter.getSelectors();

export const selectAllCards = createSelector(selectCardsState, selectAll);
export const selectCardEntities = createSelector(selectCardsState, selectEntities);
export const selectCardCount = createSelector(selectCardsState, selectTotal);
export const selectCardsLoading = createSelector(selectCardsState, s => s.loading);
export const selectCardsActionLoading = createSelector(selectCardsState, s => s.actionLoading);
export const selectCardsError = createSelector(selectCardsState, s => s.error);
export const selectCardsSuccessMessage = createSelector(selectCardsState, s => s.successMessage);

export const selectCardById = (id: string) => createSelector(
  selectCardEntities,
  (entities) => entities[id]
);
