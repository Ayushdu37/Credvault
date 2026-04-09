import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { CreditCard, RequestCardPayload, AddCardPayload } from '../../features/cards/services/cards.service';

export const CardsActions = createActionGroup({
  source: 'Cards',
  events: {
    'Load Cards': emptyProps(),
    'Load Cards Success': props<{ cards: CreditCard[] }>(),
    'Load Cards Failure': props<{ error: string }>(),

    'Lock Card': props<{ id: string }>(),
    'Lock Card Success': props<{ id: string }>(),
    'Lock Card Failure': props<{ error: string }>(),

    'Unlock Card': props<{ id: string }>(),
    'Unlock Card Success': props<{ id: string }>(),
    'Unlock Card Failure': props<{ error: string }>(),

    'Request Card': props<{ payload: RequestCardPayload }>(),
    'Request Card Success': props<{ message: string }>(),
    'Request Card Failure': props<{ error: string }>(),

    'Set Default Card': props<{ id: string }>(),
    'Set Default Card Success': props<{ id: string }>(),
    'Set Default Card Failure': props<{ error: string }>(),

    'Verify Card': props<{ id: string }>(),
    'Verify Card Success': props<{ id: string }>(),
    'Verify Card Failure': props<{ error: string }>(),

    'Delete Card': props<{ id: string }>(),
    'Delete Card Success': props<{ id: string }>(),
    'Delete Card Failure': props<{ error: string }>(),

    'Add Card': props<{ payload: AddCardPayload }>(),
    'Add Card Success': props<{ message: string }>(),
    'Add Card Failure': props<{ error: string }>(),
  },
});
