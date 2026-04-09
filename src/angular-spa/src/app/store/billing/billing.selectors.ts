import { createFeatureSelector, createSelector } from '@ngrx/store';
import { BillingState, billingAdapter } from './billing.reducer';

export const selectBillingState = createFeatureSelector<BillingState>('billing');

const { selectAll, selectEntities } = billingAdapter.getSelectors();

export const selectAllBills = createSelector(selectBillingState, selectAll);
export const selectBillEntities = createSelector(selectBillingState, selectEntities);
export const selectBillingLoading = createSelector(selectBillingState, s => s.loading);
export const selectBillingError = createSelector(selectBillingState, s => s.error);
export const selectSelectedBillId = createSelector(selectBillingState, s => s.selectedBillId);

export const selectBillById = (id: string) => createSelector(
  selectBillEntities,
  (entities) => entities[id]
);

export const selectSelectedBill = createSelector(
  selectBillEntities,
  selectSelectedBillId,
  (entities, id) => id ? entities[id] : undefined
);
