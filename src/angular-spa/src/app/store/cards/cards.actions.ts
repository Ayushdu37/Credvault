import { createActionGroup, props } from '@ngrx/store';
import { CreditCard, AddCardRequest } from '../../core/models/card.model';

export const CardsActions = createActionGroup({
  source: 'Cards',
  events: {
    'Load Cards': props<{ page: number; pageSize: number }>(),
    'Load Cards Success': props<{ cards: CreditCard[]; totalCount: number }>(),
    'Load Cards Failure': props<{ error: string }>(),

    'Set Default Card': props<{ id: string }>(),
    'Set Default Card Success': props<{ id: string }>(),
    'Set Default Card Failure': props<{ error: string }>(),

    'Verify Card': props<{ id: string }>(),
    'Verify Card Success': props<{ id: string }>(),
    'Verify Card Failure': props<{ error: string }>(),

    'Delete Card': props<{ id: string }>(),
    'Delete Card Success': props<{ id: string }>(),
    'Delete Card Failure': props<{ error: string }>(),

    'Add Card': props<{ payload: AddCardRequest }>(),
    'Add Card Success': props<{ card: CreditCard }>(),
    'Add Card Failure': props<{ error: string }>(),

    'Update Card Limit': props<{ id: string; newLimit: number }>(),
    'Update Card Limit Success': props<{ id: string; newLimit: number }>(),
    'Update Card Limit Failure': props<{ error: string }>(),

    'Select Card': props<{ id: string }>(),
    'Load Card By Id': props<{ id: string }>(),
    'Load Card By Id Success': props<{ card: CreditCard }>(),
    'Load Card By Id Failure': props<{ error: string }>(),
  },
});
