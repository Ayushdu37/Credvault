import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { PaymentMethod } from '../../features/payment-methods/services/payment-methods.service';

export const PaymentMethodsActions = createActionGroup({
  source: 'PaymentMethods',
  events: {
    'Load Methods': emptyProps(),
    'Load Methods Success': props<{ methods: PaymentMethod[] }>(),
    'Load Methods Failure': props<{ error: string }>(),

    'Delete Method': props<{ id: string }>(),
    'Delete Method Success': props<{ id: string }>(),

    'Set Default': props<{ id: string }>(),
    'Set Default Success': props<{ methods: PaymentMethod[] }>(),
  },
});
