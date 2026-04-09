import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { createReducer, on } from '@ngrx/store';
import { CreditCard } from '../../features/cards/services/cards.service';
import { CardsActions } from './cards.actions';

export interface CardsState extends EntityState<CreditCard> {
  loading: boolean;
  actionLoading: boolean; // for lock/unlock/request operations
  error: string | null;
  successMessage: string | null;
}

export const cardsAdapter = createEntityAdapter<CreditCard>();

export const initialState: CardsState = cardsAdapter.getInitialState({
  loading: false,
  actionLoading: false,
  error: null,
  successMessage: null,
});

export const cardsReducer = createReducer(
  initialState,

  // Load Cards
  on(CardsActions.loadCards, (state) => ({
    ...state, loading: true, error: null,
  })),
  on(CardsActions.loadCardsSuccess, (state, { cards }) =>
    cardsAdapter.setAll(cards, { ...state, loading: false })
  ),
  on(CardsActions.loadCardsFailure, (state, { error }) => ({
    ...state, loading: false, error,
  })),

  // Lock Card
  on(CardsActions.lockCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.lockCardSuccess, (state, { id }) =>
    cardsAdapter.updateOne({ id, changes: { status: 'Locked' } }, { ...state, actionLoading: false })
  ),
  on(CardsActions.lockCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  // Unlock Card
  on(CardsActions.unlockCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.unlockCardSuccess, (state, { id }) =>
    cardsAdapter.updateOne({ id, changes: { status: 'Active' } }, { ...state, actionLoading: false })
  ),
  on(CardsActions.unlockCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  // Request Card
  on(CardsActions.requestCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.requestCardSuccess, (state, { message }) => ({
    ...state, actionLoading: false, successMessage: message,
  })),
  on(CardsActions.requestCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  // Set Default Card
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

  // Verify Card
  on(CardsActions.verifyCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.verifyCardSuccess, (state, { id }) =>
    cardsAdapter.updateOne({ id, changes: { isVerified: true } }, { ...state, actionLoading: false })
  ),
  on(CardsActions.verifyCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  // Delete Card
  on(CardsActions.deleteCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.deleteCardSuccess, (state, { id }) =>
    cardsAdapter.removeOne(id, { ...state, actionLoading: false, successMessage: 'Card removed successfully.' })
  ),
  on(CardsActions.deleteCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),

  // Add Card
  on(CardsActions.addCard, (state) => ({ ...state, actionLoading: true })),
  on(CardsActions.addCardSuccess, (state, { message }) => ({
    ...state, actionLoading: false, successMessage: message,
  })),
  on(CardsActions.addCardFailure, (state, { error }) => ({
    ...state, actionLoading: false, error,
  })),
);
