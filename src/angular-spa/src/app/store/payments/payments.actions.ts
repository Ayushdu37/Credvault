import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { 
  Payment, 
  MakePaymentRequest
} from '../../core/models/payment.model';

export const PaymentsActions = createActionGroup({
  source: 'Payments',
  events: {
    'Load Payment History': props<{ page: number; pageSize: number }>(),
    'Load Payment History Success': props<{ payments: Payment[]; totalCount: number }>(),
    'Load Payment History Failure': props<{ error: string }>(),

    'Submit Payment': props<{ payload: MakePaymentRequest }>(),
    'Submit Payment Success': props<{ paymentId: string }>(),
    'Submit Payment Failure': props<{ error: string }>(),

    'Clear Payment Result': emptyProps(),
  },
});
