import { createFeatureSelector, createSelector } from '@ngrx/store';
import { PaymentsState, paymentsAdapter } from './payments.reducer';

export const selectPaymentsState = createFeatureSelector<PaymentsState>('payments');

const { selectAll } = paymentsAdapter.getSelectors();

export const selectAllPayments = createSelector(selectPaymentsState, selectAll);
export const selectPaymentsLoading = createSelector(selectPaymentsState, s => s.loading);
export const selectPaymentsSubmitting = createSelector(selectPaymentsState, s => s.submitting);
export const selectPaymentsError = createSelector(selectPaymentsState, s => s.error);
export const selectPaymentSuccessMessage = createSelector(selectPaymentsState, s => s.successMessage);

// ─── Stats Selectors ─────────────────────────────────────────
export const selectTotalPaymentsThisMonth = createSelector(selectAllPayments, payments => {
    const now = new Date();
    const thisMonth = payments.filter(p => {
        const date = new Date(p.date);
        return p.status === 'Completed' && date.getMonth() === now.getMonth() && date.getFullYear() === now.getFullYear();
    });
    return thisMonth.reduce((sum, p) => sum + p.amount, 0);
});

export const selectPendingPaymentsCount = createSelector(selectAllPayments, payments => {
    return payments.filter(p => p.status === 'Pending').length;
});

export const selectCompletedPaymentsCount = createSelector(selectAllPayments, payments => {
    return payments.filter(p => p.status === 'Completed').length;
});

export const selectFailedPaymentsCount = createSelector(selectAllPayments, payments => {
    return payments.filter(p => p.status === 'Failed').length;
});
