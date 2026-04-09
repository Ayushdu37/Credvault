import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { PaymentMethod } from '../../core/models/payment.model';
import { PaymentMethodsActions } from './payment-methods.actions';

export interface PaymentMethodsState extends EntityState<PaymentMethod> {
  loading: boolean;
  error: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const adapter: EntityAdapter<PaymentMethod> = createEntityAdapter<PaymentMethod>();

export const initialState: PaymentMethodsState = adapter.getInitialState({
  loading: false,
  error: null,
  page: 1,
  pageSize: 10,
  totalCount: 0,
});

export const paymentMethodsReducer = createReducer(
  initialState,
  on(PaymentMethodsActions.loadMethods, (state, { page, pageSize }) =>
    ({ ...state, loading: true, error: null, page, pageSize })
  ),
  on(PaymentMethodsActions.loadMethodsSuccess, (state, { methods, totalCount }) =>
    adapter.setAll(methods, { ...state, loading: false, totalCount })
  ),
  on(PaymentMethodsActions.loadMethodsFailure, (state, { error }) =>
    ({ ...state, loading: false, error })
  ),
  on(PaymentMethodsActions.addMethodSuccess, (state) =>
    state
  ),
  on(PaymentMethodsActions.deleteMethodSuccess, (state, { id }) =>
    adapter.removeOne(id, state)
  ),
);
