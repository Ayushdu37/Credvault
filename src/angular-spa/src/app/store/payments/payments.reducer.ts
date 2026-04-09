import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { createReducer, on } from '@ngrx/store';
import { Payment } from '../../features/payments/services/payments.service';
import { PaymentsActions } from './payments.actions';

export interface PaymentsState extends EntityState<Payment> {
  loading: boolean;
  submitting: boolean;
  error: string | null;
  successMessage: string | null;
  lastReferenceNumber: string | null;
}

export const paymentsAdapter = createEntityAdapter<Payment>();

export const initialState: PaymentsState = paymentsAdapter.getInitialState({
  loading: false,
  submitting: false,
  error: null,
  successMessage: null,
  lastReferenceNumber: null,
});

export const paymentsReducer = createReducer(
  initialState,

  on(PaymentsActions.loadPaymentHistory, (state) => ({
    ...state, loading: true, error: null,
  })),
  on(PaymentsActions.loadPaymentHistorySuccess, (state, { payments }) =>
    paymentsAdapter.setAll(payments, { ...state, loading: false })
  ),
  on(PaymentsActions.loadPaymentHistoryFailure, (state, { error }) => ({
    ...state, loading: false, error,
  })),

  on(PaymentsActions.submitPayment, (state) => ({
    ...state, submitting: true, error: null, successMessage: null, lastReferenceNumber: null,
  })),
  on(PaymentsActions.submitPaymentSuccess, (state, { referenceNumber, message }) => ({
    ...state, submitting: false, successMessage: message, lastReferenceNumber: referenceNumber,
  })),
  on(PaymentsActions.submitPaymentFailure, (state, { error }) => ({
    ...state, submitting: false, error,
  })),

  on(PaymentsActions.clearPaymentResult, (state) => ({
    ...state, successMessage: null, error: null, lastReferenceNumber: null,
  })),
);
