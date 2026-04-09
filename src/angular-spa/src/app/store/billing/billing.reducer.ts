import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { createReducer, on } from '@ngrx/store';
import { BillingStatement } from '../../features/billing/services/billing.service';
import { BillingActions } from './billing.actions';

export interface BillingState extends EntityState<BillingStatement> {
  loading: boolean;
  selectedBillId: string | null;
  error: string | null;
}

export const billingAdapter = createEntityAdapter<BillingStatement>();

export const initialState: BillingState = billingAdapter.getInitialState({
  loading: false,
  selectedBillId: null,
  error: null,
});

export const billingReducer = createReducer(
  initialState,

  on(BillingActions.loadBills, (state) => ({
    ...state, loading: true, error: null,
  })),
  on(BillingActions.loadBillsSuccess, (state, { bills }) =>
    billingAdapter.setAll(bills, { ...state, loading: false })
  ),
  on(BillingActions.loadBillsFailure, (state, { error }) => ({
    ...state, loading: false, error,
  })),

  on(BillingActions.loadBillDetail, (state, { id }) => ({
    ...state, loading: true, selectedBillId: id, error: null,
  })),
  on(BillingActions.loadBillDetailSuccess, (state, { bill }) =>
    billingAdapter.upsertOne(bill, { ...state, loading: false })
  ),
  on(BillingActions.loadBillDetailFailure, (state, { error }) => ({
    ...state, loading: false, error,
  })),
);
