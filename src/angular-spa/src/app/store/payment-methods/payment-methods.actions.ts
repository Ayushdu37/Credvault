import { createActionGroup, props, emptyProps } from '@ngrx/store';
import { PaymentMethod, AddPaymentMethodRequest } from '../../core/models/payment.model';

export const PaymentMethodsActions = createActionGroup({
  source: 'PaymentMethods',
  events: {
    'Load Methods': props<{ page: number; pageSize: number }>(),
    'Load Methods Success': props<{ methods: PaymentMethod[]; totalCount: number }>(),
    'Load Methods Failure': props<{ error: string }>(),

    'Add Method': props<{ payload: AddPaymentMethodRequest }>(),
    'Add Method Success': emptyProps(),
    'Add Method Failure': props<{ error: string }>(),

    'Delete Method': props<{ id: string }>(),
    'Delete Method Success': props<{ id: string }>(),
    'Delete Method Failure': props<{ error: string }>(),
  },
});
