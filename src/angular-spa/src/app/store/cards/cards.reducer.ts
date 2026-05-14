import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { createReducer, on } from '@ngrx/store';
import { CreditCard } from '../../core/models/card.model';
import { CardsActions } from './cards.actions';

export interface CardsState extends EntityState<CreditCard> {
  loading: boolean;
  actionLoading: boolean;
  error: string | null;
  successMessage: string | null;
  selectedId: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const cardsAdapter = createEntityAdapter<CreditCard>();

export const initialState: CardsState = cardsAdapter.getInitialState({
  loading: false,
  actionLoading: false,
  error: null,
  successMessage: null,
  selectedId: null,
  page: 1,
  pageSize: 10,
  totalCount: 0,
});

export const cardsReducer = createReducer(
  initialState,

  on(CardsActions.loadCards, (state, { page, pageSize }) => ({
    ...state, loading: true, error: null, page, pageSize,
  })),
  on(CardsActions.loadCardsSuccess, (state, { cards, totalCount }) =>
    cardsAdapter.setAll(cards, { ...state, loading: false, totalCount })
  ),
  on(CardsActions.loadCardsFailure, (state, { error }) => ({
    ...state, loading: false, error,
  })),

  on(CardsActions.selectCard, (state, { id }) => ({
    ...state, selectedId: id,
  })),

  on(CardsActions.loadCardById, (state) => ({ ...state, loading: true })),
  on(CardsActions.loadCardByIdSuccess, (state, { card }) =>
    cardsAdapter.upsertOne(card, { ...state, loading: false })
  ),
  on(CardsActions.loadCardByIdFailure, (state, { error }) => ({
    ...state, loading: false, error,
  })),

  on(CardsActions.setDefaultCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.setDefaultCardSuccess, (state, { id }) => {
    const allIds = state.ids as string[];
    const updates = allIds.map(cid => ({
      id: cid,
      changes: { isDefault: cid === id },
    }));
    return cardsAdapter.updateMany(updates, { ...state, actionLoading: false });
  }),
  on(CardsActions.setDefaultCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  on(CardsActions.verifyCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.verifyCardSuccess, (state, { id }) =>
    cardsAdapter.updateOne({ id, changes: { isVerified: true } }, { ...state, actionLoading: false })
  ),
  on(CardsActions.verifyCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  on(CardsActions.deleteCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.deleteCardSuccess, (state, { id }) =>
    cardsAdapter.removeOne(id, { ...state, actionLoading: false, successMessage: 'Card removed successfully.' })
  ),
  on(CardsActions.deleteCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  on(CardsActions.addCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.addCardSuccess, (state, { card }) =>
    cardsAdapter.addOne(card, { 
      ...state, 
      actionLoading: false, 
      successMessage: 'Card added successfully.',
      totalCount: state.totalCount + 1
    })
  ),
  on(CardsActions.addCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  on(CardsActions.updateCardLimit, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.updateCardLimitSuccess, (state, { id, newLimit }) =>
    cardsAdapter.updateOne({ id, changes: { creditLimit: newLimit } }, { ...state, actionLoading: false })
  ),
  on(CardsActions.updateCardLimitFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),
);
