import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { BillingStatement, PaymentScheduleResponse, SchedulePaymentRequest } from '../../core/models/billing.model';

export const BillingActions = createActionGroup({
  source: 'Billing',
  events: {
    'Load Bills': props<{ page: number; pageSize: number }>(),
    'Load Bills Success': props<{ bills: BillingStatement[]; totalCount: number }>(),
    'Load Bills Failure': props<{ error: string }>(),

    'Load Bill Detail': props<{ id: string }>(),
    'Load Bill Detail Success': props<{ bill: BillingStatement }>(),
    'Load Bill Detail Failure': props<{ error: string }>(),

    'Schedule Payment': props<{ billId: string; payload: SchedulePaymentRequest }>(),
    'Schedule Payment Success': props<{ schedule: PaymentScheduleResponse }>(),
    'Schedule Payment Failure': props<{ error: string }>(),

    'Cancel Scheduled Payment': props<{ scheduleId: string }>(),
    'Cancel Scheduled Payment Success': props<{ scheduleId: string }>(),
    'Cancel Scheduled Payment Failure': props<{ error: string }>(),

    'Refresh Bill After Payment': props<{ billId: string }>(),
    'Refresh Bill Success': props<{ bill: BillingStatement }>(),
  },
});
