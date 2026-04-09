import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { createReducer, on } from '@ngrx/store';
import { BillingStatement } from '../../core/models/billing.model';
import { BillingActions } from './billing.actions';

export interface BillingState extends EntityState<BillingStatement> {
  loading: boolean;
  selectedBillId: string | null;
  error: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const billingAdapter = createEntityAdapter<BillingStatement>();

export const initialState: BillingState = billingAdapter.getInitialState({
  loading: false,
  selectedBillId: null,
  error: null,
  page: 1,
  pageSize: 10,
  totalCount: 0,
});

export const billingReducer = createReducer(
  initialState,

  on(BillingActions.loadBills, (state, { page, pageSize }) => ({
    ...state, loading: true, error: null, page, pageSize,
  })),
  on(BillingActions.loadBillsSuccess, (state, { bills, totalCount }) =>
    billingAdapter.setAll(bills, { ...state, loading: false, totalCount })
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

  on(BillingActions.refreshBillAfterPayment, (state, { billId }) => ({
    ...state, selectedBillId: billId, loading: true,
  })),
  on(BillingActions.refreshBillSuccess, (state, { bill }) =>
    billingAdapter.upsertOne(bill, { ...state, loading: false })
  ),

  on(BillingActions.schedulePaymentSuccess, (state) => state),
  on(BillingActions.cancelScheduledPaymentSuccess, (state) => state),
);
