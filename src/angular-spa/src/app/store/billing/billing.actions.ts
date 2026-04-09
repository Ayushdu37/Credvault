import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { BillingStatement } from '../../features/billing/services/billing.service';

export const BillingActions = createActionGroup({
  source: 'Billing',
  events: {
    'Load Bills': emptyProps(),
    'Load Bills Success': props<{ bills: BillingStatement[] }>(),
    'Load Bills Failure': props<{ error: string }>(),

    'Load Bill Detail': props<{ id: string }>(),
    'Load Bill Detail Success': props<{ bill: BillingStatement }>(),
    'Load Bill Detail Failure': props<{ error: string }>(),
  },
});
