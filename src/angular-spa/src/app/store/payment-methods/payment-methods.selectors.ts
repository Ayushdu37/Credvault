import { createFeatureSelector, createSelector } from '@ngrx/store';
import { PaymentMethodsState, adapter } from './payment-methods.reducer';

export const selectPaymentMethodsState = createFeatureSelector<PaymentMethodsState>('paymentMethods');

const { selectAll } = adapter.getSelectors();

export const selectAllPaymentMethods = createSelector(selectPaymentMethodsState, selectAll);
export const selectPaymentMethodsLoading = createSelector(selectPaymentMethodsState, (s) => s.loading);
export const selectPaymentMethodsError = createSelector(selectPaymentMethodsState, (s) => s.error);
export const selectDefaultMethod = createSelector(selectAllPaymentMethods, (methods) => methods.find(m => m.isDefault));
