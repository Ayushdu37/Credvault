import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Payment, SubmitPaymentPayload } from '../../features/payments/services/payments.service';

export const PaymentsActions = createActionGroup({
  source: 'Payments',
  events: {
    'Load Payment History': emptyProps(),
    'Load Payment History Success': props<{ payments: Payment[] }>(),
    'Load Payment History Failure': props<{ error: string }>(),

    'Submit Payment': props<{ payload: SubmitPaymentPayload }>(),
    'Submit Payment Success': props<{ referenceNumber: string; message: string }>(),
    'Submit Payment Failure': props<{ error: string }>(),

    'Clear Payment Result': emptyProps(),
  },
});
