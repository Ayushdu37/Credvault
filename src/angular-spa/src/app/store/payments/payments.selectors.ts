import { createFeatureSelector, createSelector } from '@ngrx/store';
import { PaymentsState, paymentsAdapter } from './payments.reducer';

export const selectPaymentsState = createFeatureSelector<PaymentsState>('payments');

const { selectAll } = paymentsAdapter.getSelectors();

export const selectAllPayments = createSelector(selectPaymentsState, selectAll);
export const selectPaymentsLoading = createSelector(selectPaymentsState, s => s.loading);
export const selectPaymentsSubmitting = createSelector(selectPaymentsState, s => s.submitting);
export const selectPaymentsError = createSelector(selectPaymentsState, s => s.error);
export const selectPaymentSuccessMessage = createSelector(selectPaymentsState, s => s.successMessage);
export const selectLastReferenceNumber = createSelector(selectPaymentsState, s => s.lastReferenceNumber);
