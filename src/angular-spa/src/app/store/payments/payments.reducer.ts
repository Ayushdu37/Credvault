import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { createReducer, on } from '@ngrx/store';
import { Payment } from '../../core/models/payment.model';
import { PaymentsActions } from './payments.actions';

export interface PaymentsState extends EntityState<Payment> {
  loading: boolean;
  submitting: boolean;
  error: string | null;
  successMessage: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const paymentsAdapter = createEntityAdapter<Payment>();

export const initialState: PaymentsState = paymentsAdapter.getInitialState({
  loading: false,
  submitting: false,
  error: null,
  successMessage: null,
  page: 1,
  pageSize: 10,
  totalCount: 0,
});

export const paymentsReducer = createReducer(
  initialState,

  on(PaymentsActions.loadPaymentHistory, (state, { page, pageSize }) => ({
    ...state, loading: true, error: null, page, pageSize,
  })),
  on(PaymentsActions.loadPaymentHistorySuccess, (state, { payments, totalCount }) =>
    paymentsAdapter.setAll(payments, { ...state, loading: false, totalCount })
  ),
  on(PaymentsActions.loadPaymentHistoryFailure, (state, { error }) => ({
    ...state, loading: false, error,
  })),

  on(PaymentsActions.submitPayment, (state) => ({
    ...state, submitting: true, error: null, successMessage: null,
  })),
  on(PaymentsActions.submitPaymentSuccess, (state, { payment }) =>
    paymentsAdapter.addOne(payment, {
      ...state,
      submitting: false,
      successMessage: `Payment of ₹${payment.amount} submitted successfully.`,
    })
  ),
  on(PaymentsActions.submitPaymentFailure, (state, { error }) => ({
    ...state, submitting: false, error,
  })),

  on(PaymentsActions.clearPaymentResult, (state) => ({
    ...state, successMessage: null, error: null,
  })),
);
