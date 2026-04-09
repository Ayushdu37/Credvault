import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { PaymentMethod } from '../../features/payment-methods/services/payment-methods.service';
import { PaymentMethodsActions } from './payment-methods.actions';

export interface PaymentMethodsState extends EntityState<PaymentMethod> {
  loading: boolean;
  error: string | null;
}

export const adapter: EntityAdapter<PaymentMethod> = createEntityAdapter<PaymentMethod>();

export const initialState: PaymentMethodsState = adapter.getInitialState({
  loading: false,
  error: null,
});

export const paymentMethodsReducer = createReducer(
  initialState,
  on(PaymentMethodsActions.loadMethods, (state) => ({ ...state, loading: true, error: null })),
  on(PaymentMethodsActions.loadMethodsSuccess, (state, { methods }) =>
    adapter.setAll(methods, { ...state, loading: false })
  ),
  on(PaymentMethodsActions.loadMethodsFailure, (state, { error }) => ({ ...state, loading: false, error })),
  on(PaymentMethodsActions.deleteMethodSuccess, (state, { id }) =>
    adapter.removeOne(id, state)
  ),
  on(PaymentMethodsActions.setDefaultSuccess, (state, { methods }) =>
    adapter.setAll(methods, state)
  ),
);
